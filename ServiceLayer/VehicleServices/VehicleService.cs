using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemModel.Entities;
using SystemContext.SystemDbContext;
using SystemDTOS;
using SystemDTOS.VehicleDTOS;
using System.Security.Claims;
namespace ServiceLayer.VehicleServices
{
    public class VehicleService
    {
        private readonly DelivryDB _context;
        public VehicleService(DelivryDB context)
        {
            _context = context;
        }

        public VehicleResponseDTO CreateVehicle(CreateVehicleDTO dto,int UserID)
        {
            var Driver = _context.Drivers.FirstOrDefault(d => d.ID == dto.DriverID);
            if (Driver == null)
            {
                throw new Exception("Driver Not Found");
            }
            if (Driver.Status == DriverStatus.NotActive)
            {
                throw new Exception("This Driver Is Not Active");
            }
            var user = _context.Users.Find(UserID);
            if (user.Role == UserRole.Driver)
            {
                if (UserID != Driver.UserID)
                {
                    throw new Exception("You Can Not Create A Vehicle For Another User");
                }
            }
            if (string.IsNullOrWhiteSpace(dto.PlateNumber) || string.IsNullOrWhiteSpace(dto.Type.ToString()))
            {
                throw new Exception("Invalid Vehicle Details");
            }
           
            var DV = _context.Vehicles.FirstOrDefault(v => v.DriverID == dto.DriverID);
            if (DV != null)
            {
                throw new Exception("This Driver Is Already Has Vehicle");
            }
            var Vehicle = new Vehicle()
            {
                PlateNumber = dto.PlateNumber,
                DriverID = dto.DriverID,
                Type = dto.Type.ToString()
            };
            _context.Vehicles.Add(Vehicle);
            _context.SaveChanges();
            return new VehicleResponseDTO()
            {
                PlateNumber = Vehicle.PlateNumber,
                Type = Vehicle.Type,
                DriverID = Vehicle.DriverID,
                VehicleID = Vehicle.ID
            };

        }
        public VehicleResponseDTO UpdateVehicle(UpdateVehicleDTO dto, int VehicleID)
        {
            if (string.IsNullOrWhiteSpace(dto.PlateNumber) || string.IsNullOrWhiteSpace(dto.Type.ToString()))
            {
                throw new Exception("Invalid Vehicle Details");
            }
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.ID == VehicleID);
            if (vehicle == null)
            {
                throw new Exception("Vehicle Not Found");
            }
            
            var Driver = _context.Drivers.FirstOrDefault(d => d.ID == vehicle.DriverID);
            if (Driver.Status == DriverStatus.NotActive)
            {
                throw new Exception("This Driver Is Not Active");
           
            }
            var NewVehicleTest = _context.Vehicles.FirstOrDefault(v => v.PlateNumber == dto.PlateNumber && v.ID != VehicleID);
            if (NewVehicleTest != null)
            {
                throw new Exception("This PlateNumber Is For Another Vehicle");
            }
            vehicle.PlateNumber = dto.PlateNumber;
            vehicle.Type = dto.Type.ToString();
            _context.SaveChanges();
            return new VehicleResponseDTO()
            {
                PlateNumber = vehicle.PlateNumber,
                Type = vehicle.Type,
                VehicleID = vehicle.ID,
                DriverID = vehicle.DriverID
            };

        }
        public List<VehicleResponseDTO> GetVehicles()
        {
            var vehicles = _context.Vehicles.ToList();
            List<VehicleResponseDTO> res = new List<VehicleResponseDTO>();
            foreach (var vehicle in vehicles)
            {
                res.Add(new VehicleResponseDTO()
                {
                    PlateNumber = vehicle.PlateNumber,
                    Type = vehicle.Type,
                    VehicleID = vehicle.ID,
                    DriverID = vehicle.DriverID

                });
            }
            return res;
        }
        public VehicleResponseDTO GetVehicleByID(int VehicleID)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.ID == VehicleID);
            if (vehicle == null)
            {
                throw new Exception("Vehicle Not Found");
            }
            return new VehicleResponseDTO()
            {
                PlateNumber = vehicle.PlateNumber,
                Type = vehicle.Type,
                VehicleID = vehicle.ID,
                DriverID = vehicle.DriverID
            };
        } 
        public VehicleResponseDTO GetVehicleByDriverID(int DriverID)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.DriverID == DriverID);
            if (vehicle == null)
            {
                throw new Exception("Vehicle Not Found");
            }
            return new VehicleResponseDTO()
            {
                PlateNumber = vehicle.PlateNumber,
                Type = vehicle.Type,
                VehicleID = vehicle.ID,
                DriverID = vehicle.DriverID
            };
        }
        public string DeleteVehicle(int VehicleID)
        {
            var vehicle = _context.Vehicles.FirstOrDefault(v => v.ID == VehicleID);
            if(vehicle == null)
            {
                throw new Exception("Vehicle Not Found");
            }
            var Driver = _context.Drivers.FirstOrDefault(d => d.ID == vehicle.DriverID);
            if(Driver.Status == DriverStatus.Busy)
            {
                throw new Exception("Driver Must Finish The Order First");
            }
            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();

            return "Vehicle Deleted Succesfully";

        }
        
    }
}
