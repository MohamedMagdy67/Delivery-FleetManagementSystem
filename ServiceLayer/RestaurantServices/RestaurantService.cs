using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.AreaDTOS;
using SystemDTOS.RestaurantDTOS;
using SystemModel.Entities;

namespace ServiceLayer.RestaurantServices
{
    public class RestaurantService
    {
        public readonly DelivryDB _context;
        public RestaurantService(DelivryDB context)
        {
            _context = context;
        }

        public RestaurantResponseDTO CreateRestaurant(CreateRestaurantDTO dto)
        {
            if(string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Address))
            {
                throw new Exception("Invalid Details");
            }

            var restaurant = _context.Restaurants.FirstOrDefault(r => r.Name == dto.Name);
            
            if(restaurant != null)
            {
                throw new BadHttpRequestException("Invalid Restaurant Name");
            }
            
            var Area = _context.Areas.FirstOrDefault(a => a.ID == dto.AreaID);
            
            if (Area == null)
            {
                throw new BadHttpRequestException("Area Not Found");
            }
            
            var r = new Restaurant()
            {
                Name = dto.Name,
                Address = dto.Address,
                Phone = dto.Phone,
                AreaID = dto.AreaID,
                OpeningHours = dto.OpeningHours,
                IsActive = false
            };
            
            _context.Restaurants.Add(r);
            _context.SaveChanges();
            
            return new RestaurantResponseDTO()
            {
                Name = r.Name,
                Address = r.Address,
                Phone = r.Phone,
                AreaID = r.AreaID,
                OpeningHours = r.OpeningHours,
                ID = r.ID,
                IsActive = r.IsActive
            };
        }
        public List<RestaurantResponseDTO> GetRestaurants()
        {
            var Restaurants = _context.Restaurants.Where(r => r.IsActive == true).ToList();
            List<RestaurantResponseDTO> res = new List<RestaurantResponseDTO>();
            
            foreach(var r in Restaurants)
            {
                res.Add(new RestaurantResponseDTO()
                {
                    Name = r.Name,
                    Address = r.Address,
                    Phone = r.Phone,
                    AreaID = r.AreaID,
                    OpeningHours = r.OpeningHours,
                    ID = r.ID,
                    IsActive = r.IsActive
                });
            }
            return res;
        }
        public RestaurantResponseDTO GetRestaurantByID(int RestaurantID)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.ID == RestaurantID);
            if(restaurant == null)
            {
                throw new Exception("Not Found Restaurant");
            }
            RestaurantResponseDTO res = new RestaurantResponseDTO()
            { 
               Name = restaurant.Name,
               Address = restaurant.Address,
               Phone = restaurant.Phone,
               ID = restaurant.ID,
               IsActive = restaurant.IsActive,
               OpeningHours = restaurant.OpeningHours,
               AreaID = restaurant.AreaID,
            
            };
            return res;
        }
        public RestaurantResponseDTO ActivateRestaurant(int RestaurantID)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.ID == RestaurantID);
            if (restaurant == null)
            {
                throw new Exception("Not Found Restaurant");
            }
            if (restaurant.IsActive == true)
            {
                throw new Exception("Restaurant Is Already Active");
            }
            restaurant.IsActive = true;
            _context.SaveChanges();
            RestaurantResponseDTO res = new RestaurantResponseDTO()
            {
                Name = restaurant.Name,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                ID = restaurant.ID,
                IsActive = restaurant.IsActive,
                OpeningHours = restaurant.OpeningHours,
                AreaID = restaurant.AreaID,

            };
            return res;
        }
        public RestaurantResponseDTO DeActivateRestaurant(int RestaurantID)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.ID == RestaurantID);
            if (restaurant == null)
            {
                throw new Exception("Not Found Restaurant");
            }
            if(restaurant.IsActive == false)
            {
                throw new Exception("Restaurant Is Already Non Active");
            }
            restaurant.IsActive = false;
            _context.SaveChanges();
            RestaurantResponseDTO res = new RestaurantResponseDTO()
            {
                Name = restaurant.Name,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                ID = restaurant.ID,
                IsActive = restaurant.IsActive,
                OpeningHours = restaurant.OpeningHours,
                AreaID = restaurant.AreaID,

            };
            return res;
        }
        public RestaurantResponseDTO UpdateRestaurant(int RestaurantID,UpdateRestaurantDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Address))
            {
                throw new Exception("Invalid Details");
            }

            var restaurant = _context.Restaurants.FirstOrDefault(r => r.ID == RestaurantID);
            
            if(restaurant == null)
            {
                throw new Exception("Restaurant Not Found");
            }

            if(restaurant.AreaID != dto.AreaID)
            {
                var Area = _context.Areas.FirstOrDefault(a => a.ID == dto.AreaID);
                if(Area == null)
                {
                    throw new Exception("This New Area Is Not Exist");
                }
            }
            restaurant.Name = dto.Name;
            restaurant.Address = dto.Address;
            restaurant.Phone = dto.Phone;
            restaurant.OpeningHours = dto.OpeningHours;
            restaurant.AreaID = dto.AreaID;
            _context.SaveChanges();

            return new RestaurantResponseDTO()
            {
                Name = restaurant.Name,
                Address = restaurant.Address,
                Phone = restaurant.Phone,
                OpeningHours = restaurant.OpeningHours,
                AreaID = restaurant.AreaID,
                ID = restaurant.ID,
                IsActive = restaurant.IsActive
            };
                       
        }
        public string DeleteRestaurant(int RestaurantID)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(r => r.ID == RestaurantID);
            if (restaurant == null)
            {
                throw new Exception("Not Found Restaurant");
            }
            if(restaurant.IsActive == false)
            {
                throw new Exception("Restaurant Is Already Non Active");
            }
            restaurant.IsActive = false;
            _context.SaveChanges();
           
            return "Acount Deleted Succefully";
        }

    }
}
