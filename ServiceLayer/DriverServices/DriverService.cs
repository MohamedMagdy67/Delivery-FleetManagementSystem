using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.OrderDTOS;
using SystemModel.Entities;
using SystemDTOS.DriverDTOS;
using System.Diagnostics.Contracts;
namespace ServiceLayer.DriverServices
{
    public class DriverService
    {
        private readonly DelivryDB _context;
        public DriverService(DelivryDB context)
        {
            _context = context; 
        }

        public DriverResponseDTO CreateDriver(CreateDriverDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LicenseNumber))
            {
                throw new Exception("Invalid Driver Data");
            }
            var user = _context.Users.FirstOrDefault(u => u.ID == dto.UserID);
            if (user == null)
            {
                throw new Exception("This User Is Not Exist");
            }
            if(user.Role != UserRole.Driver)
            {
                throw new Exception("User Role Must Be Driver");
            }
            if(!user.IsActive)
            {
                throw new Exception("User Must Be Active");
            }
            var D = _context.Drivers.FirstOrDefault(d => d.UserID == dto.UserID);
            if(D != null)
            {
                throw new Exception("This User Already Has Driver Account");
            }
            var DD = _context.Drivers.FirstOrDefault(d => d.LicenseNumber == dto.LicenseNumber);
            if (DD != null)
            {
                throw new Exception("This LicenseNumber Is For Another Diver");
            }
            var Driver = new Driver()
            {
                LicenseNumber = dto.LicenseNumber,
                UserID = dto.UserID,
                Rating = 0,
                Status = DriverStatus.NotActive
            };
            _context.Drivers.Add(Driver);
            _context.SaveChanges();
            return new DriverResponseDTO()
            {
                DriverID = Driver.ID,
                UserID = Driver.UserID,
                LicenseNumber = Driver.LicenseNumber,
                rating = Driver.Rating,
                Status = DriverStatus.NotActive.ToString()

            };
        }
        public DriverResponseDTO ActivateDriver(int DriverID)
        {
            var Driver = _context.Drivers.Find(DriverID);
            if(Driver == null)
            {
                throw new Exception("This Diver Is Not Exist");
            }
            if(Driver.Status == DriverStatus.Busy)
            {
                throw new Exception("Driver Must Finish The Order First");
            }
            if(Driver.Status == DriverStatus.Active)
            {
                throw new Exception("Driver Is Already Active");
            }
            var User = _context.Users.Find(Driver.UserID);
            if (User == null || !User.IsActive ) 
            {
                throw new Exception("User Must Be Active");
            }
            Driver.Status = DriverStatus.Active;
            _context.SaveChanges();
            return new DriverResponseDTO()
            {
                DriverID = Driver.ID,
                UserID = Driver.UserID,
                LicenseNumber = Driver.LicenseNumber,
                rating = Driver.Rating,
                Status = DriverStatus.Active.ToString()
            };
        }
        public DriverResponseDTO DeActivateDriver(int DriverID)
        {
            var Driver = _context.Drivers.Find(DriverID);
            if (Driver == null)
            {
                throw new Exception("This Diver Is Not Exist");
            }
            if(Driver.Status == DriverStatus.Busy)
            {
                throw new Exception("Driver Must Finish The Order First");
            }
            if(Driver.Status == DriverStatus.NotActive)
            {
                throw new Exception("Driver Is Already NotActive");
            }

            Driver.Status = DriverStatus.NotActive;
            _context.SaveChanges();
            return new DriverResponseDTO()
            {
                DriverID = Driver.ID,
                UserID = Driver.UserID,
                LicenseNumber = Driver.LicenseNumber,
                rating = Driver.Rating,
                Status = DriverStatus.NotActive.ToString()
            };
        }
        public List<DriverResponseDTO> GetDrivers()
        {
            var Drivers = _context.Drivers.ToList();
            List<DriverResponseDTO> drivers = new List<DriverResponseDTO>();
            foreach(var d in Drivers)
            {
                drivers.Add(new DriverResponseDTO()
                {
                    DriverID = d.ID,
                    UserID = d.UserID,
                    LicenseNumber = d.LicenseNumber,
                    rating = d.Rating,
                    Status = d.Status.ToString()
                });
            }
            return drivers;
        }
        public List<DriverResponseDTO> GetActiveDrivers()
        {
            var Drivers = _context.Drivers.Where(d => d.Status == DriverStatus.Active).ToList();
            List<DriverResponseDTO> drivers = new List<DriverResponseDTO>();
            foreach (var d in Drivers)
            {
                drivers.Add(new DriverResponseDTO()
                {
                    DriverID = d.ID,
                    UserID = d.UserID,
                    LicenseNumber = d.LicenseNumber,
                    rating = d.Rating,
                    Status = d.Status.ToString()
                });
            }
            return drivers;
        }
        public List<DriverResponseDTO> GetBusyDrivers()
        {
            var Drivers = _context.Drivers.Where(d => d.Status == DriverStatus.Busy).ToList();
            List<DriverResponseDTO> drivers = new List<DriverResponseDTO>();
            foreach (var d in Drivers)
            {
                drivers.Add(new DriverResponseDTO()
                {
                    DriverID = d.ID,
                    UserID = d.UserID,
                    LicenseNumber = d.LicenseNumber,
                    rating = d.Rating,
                    Status = d.Status.ToString()
                });
            }
            return drivers;
        }
        public List<DriverResponseDTO> GetNotActiveDrivers()
        {
            var Drivers = _context.Drivers.Where(d => d.Status == DriverStatus.NotActive).ToList();
            List<DriverResponseDTO> drivers = new List<DriverResponseDTO>();
            foreach (var d in Drivers)
            {
                drivers.Add(new DriverResponseDTO()
                {
                    DriverID = d.ID,
                    UserID = d.UserID,
                    LicenseNumber = d.LicenseNumber,
                    rating = d.Rating,
                    Status = d.Status.ToString()
                });
            }
            return drivers;
        }
        public DriverResponseDTO GetDriverByID(int DriverID)
        {
            var Driver = _context.Drivers.Find(DriverID);
            if(Driver == null)
            {
                throw new Exception("Driver Not Found");
            }
            return new DriverResponseDTO()
            {
                DriverID = Driver.ID,
                UserID = Driver.UserID,
                LicenseNumber= Driver.LicenseNumber,
                rating = Driver.Rating,
                Status = Driver.Status.ToString()
            };

        }
        public DriverResponseDTO UpdateDriver(UpdateDriverDTO dto, int DriverID)
        {
            var Driver = _context.Drivers.Find(DriverID);
            if (Driver == null)
            {
                throw new Exception("Driver Not Found");
            }
            if (string.IsNullOrWhiteSpace(dto.LicenseNumber))
            {
                throw new Exception("Invalid LicenseNumber");
            }
            var DriverTest = _context.Drivers.FirstOrDefault(d => d.LicenseNumber == dto.LicenseNumber && d.ID != DriverID);
            if (DriverTest != null)
            {
                throw new Exception("LicenseNumber Is For Another Driver");
            }
            Driver.LicenseNumber = dto.LicenseNumber;
            Driver.Rating = dto.Rating;
            _context.SaveChanges();
            return new DriverResponseDTO()
            {
                DriverID = Driver.ID,
                LicenseNumber = Driver.LicenseNumber,
                rating = Driver.Rating,
                Status = Driver.Status.ToString(),
                UserID = Driver.UserID
            };

        }
        public string DeleteDriver(int DriverID)
        {
            var Driver = _context.Drivers.FirstOrDefault(d => d.ID == DriverID);
            if(Driver == null)
            {
                throw new Exception("Driver Not Found");
            }
            if(Driver.Status == DriverStatus.Busy)
            {
                throw new Exception("Driver Must Finish Order First");
            }
            Driver.Status = DriverStatus.NotActive;
            var user = _context.Users.Find(Driver.UserID);
            user.IsActive = false;
            _context.SaveChanges();
            return "Driver Deleted Succesfully";
        }
    }
}
