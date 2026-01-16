using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.LocationDTOS;
using SystemModel.Entities;

namespace ServiceLayer.LocationServices
{
    public class LocationService
    {
        private readonly DelivryDB _context;
        private const double MinDistanceMeters = 50; 
        public LocationService(DelivryDB context)
        {
            _context = context;
        }

        public async Task<double> UpdateLocationAsync(LocationDTO dto)
        {
            var oldLocation = await _context.DriverLocations
                .FirstOrDefaultAsync(l => l.DriverID == dto.DriverID);

            if (oldLocation == null)
            {
                var newLocation = new DriverLocation
                {
                    DriverID = dto.DriverID,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    Timestamp = DateTime.UtcNow
                };
                _context.DriverLocations.Add(newLocation);
                await _context.SaveChangesAsync();
                return 0;
            }
            else
            {
                var distance = GetDistanceInMeters(oldLocation.Latitude, oldLocation.Longitude, dto.Latitude, dto.Longitude);

                if (distance >= MinDistanceMeters)
                {
                    oldLocation.Latitude = dto.Latitude;
                    oldLocation.Longitude = dto.Longitude;
                    oldLocation.Timestamp = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
                return distance;
            }
            
        }

        public async Task<LocationDTO> GetDriverLocationByIDAsync(int driverID)
        {
            var driver = await _context.Drivers.Include(d => d.DriverLocation)
                .FirstOrDefaultAsync(d => d.ID == driverID);

            if (driver == null)
                throw new Exception("Driver Not Found");

            if (driver.Status == DriverStatus.NotActive)
                throw new Exception("Driver Is Not Active");

            if (driver.DriverLocation == null)
                throw new Exception("Driver Has No Location");

            return new LocationDTO
            {
                DriverID = driver.ID,
                Latitude = driver.DriverLocation.Latitude,
                Longitude = driver.DriverLocation.Longitude,
                TimeStamp = driver.DriverLocation.Timestamp
            };
        }

        public async Task<List<LocationDTO>> GetAvailableDriversLocationAsync()
        {
            var drivers = await _context.Drivers.Include(d => d.DriverLocation)
                .Where(d => d.Status == DriverStatus.Active && d.DriverLocation != null)
                .ToListAsync();

            var list = new List<LocationDTO>();
            foreach (var d in drivers)
            {
                list.Add(new LocationDTO
                {
                    DriverID = d.ID,
                    Latitude = d.DriverLocation.Latitude,
                    Longitude = d.DriverLocation.Longitude,
                    TimeStamp = d.DriverLocation.Timestamp
                });
            }
            return list;
        }

        public async Task<List<LocationDTO>> GetBusyDriversLocationAsync()
        {
            var drivers = await _context.Drivers.Include(d => d.DriverLocation)
                .Where(d => d.Status == DriverStatus.Busy && d.DriverLocation != null)
                .ToListAsync();

            var list = new List<LocationDTO>();
            foreach (var d in drivers)
            {
                list.Add(new LocationDTO
                {
                    DriverID = d.ID,
                    Latitude = d.DriverLocation.Latitude,
                    Longitude = d.DriverLocation.Longitude,
                    TimeStamp = d.DriverLocation.Timestamp
                });
            }
            return list;
        }

        public double GetDistanceInMeters(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            double R = 6371000; // نصف قطر الأرض بالمتر
            double dLat = ToRadians((double)(lat2 - lat1));
            double dLon = ToRadians((double)(lon2 - lon1));

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        public double ToRadians(double angle) => angle * Math.PI / 180;
    }
}
