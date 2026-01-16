using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.AreaDTOS;
using SystemModel.Entities;
namespace ServiceLayer.AreaServices
{
    public class AreaService
    {
        public readonly DelivryDB _context;
        public AreaService(DelivryDB context)
        {
            _context = context;            
        }

        public AreaResponseDTO CreateArea(CreateAreaDTO dto)
        { 
            if(string.IsNullOrWhiteSpace(dto.AreaName) || string.IsNullOrWhiteSpace(dto.AreaCity))
            {
                throw new Exception("Invalid Area Details");
            }
            var area = _context.Areas.FirstOrDefault(a => a.Name == dto.AreaName && a.City == dto.AreaCity);
            if(area != null)
            {
                throw new Exception("This Area Is Already Exist");
            }

            Area areaa = new Area()
            {
                Name = dto.AreaName,
                City = dto.AreaCity,
                Latitude = dto.AreaLatitude,
                Longitude = dto.AreaLongitude
            };
            _context.Areas.Add(areaa);
            _context.SaveChanges();
          
            return new AreaResponseDTO()
            {
                AreaName = areaa.Name,
                AreaCity = areaa.City,
                AreaLatitude = areaa.Latitude,
                AreaLongitude = areaa.Longitude,
                AreaID = areaa.ID
            };

        }
        public List<AreaResponseDTO> GetAreas()
        {
            var Areas = _context.Areas.ToList();
            
            List<AreaResponseDTO> areas = new List<AreaResponseDTO>();
            
            foreach(var Area in Areas)
            {
                areas.Add(new AreaResponseDTO()
                {
                    AreaID = Area.ID,
                    AreaName = Area.Name,
                    AreaCity = Area.City,
                    AreaLatitude = Area.Latitude,
                    AreaLongitude = Area.Longitude
                });
            }
            return areas;
        }
        public AreaResponseDTO GetAreaByID(int AreaID)
        {
            var area = _context.Areas.Find(AreaID);
            if(area == null)
            {
                throw new Exception("Area Not Found");
            }
           
            return new AreaResponseDTO()
            {
                AreaName = area.Name,
                AreaCity = area.City,
                AreaLongitude = area.Longitude,
                AreaLatitude = area.Latitude,
                AreaID = area.ID
            };
        }
        public AreaResponseDTO UpdateArea(int AreaID , UpdateAreaDTO dto)
        {
            if(string.IsNullOrWhiteSpace(dto.AreaName)||string.IsNullOrWhiteSpace(dto.AreaCity))
            {
                throw new Exception("Invalid Area Details");
            }
            
            Area? a = _context.Areas.Find(AreaID);
            
            if(a == null)
            {
                throw new Exception("Area Not Found");
            }

            Area? aa = _context.Areas.FirstOrDefault(a => a.Name == dto.AreaName && a.City == dto.AreaCity && a.ID != AreaID);
            if (aa != null)
            {
                throw new Exception("Invalid Area Details");
            }
            
            a.Name = dto.AreaName;
            a.City = dto.AreaCity;
            a.Latitude = dto.AreaLatitude;
            a.Longitude = dto.AreaLongitude;
            _context.SaveChanges();

            return new AreaResponseDTO()
            {
                AreaName = a.Name,
                AreaCity = a.City,
                AreaLongitude = a.Longitude,
                AreaLatitude = a.Latitude,
                AreaID = a.ID

            };
        }
        public string DeleteArea(int AreaID)
        {
            var area = _context.Areas.Include(a => a.Restaurants).FirstOrDefault(a => a.ID == AreaID);
            if(area == null)
            {
                throw new Exception("Area Not Found");
            }
            if(area.Restaurants.Any())
            {
                throw new Exception("Cannot Delete Area Has Related Restaurants");
            }
            _context.Areas.Remove(area);
            _context.SaveChanges();
            return "Area Deleted Succefully";
        }

    }
}
