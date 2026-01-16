using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.RestaurantUsersDTOS;
using SystemModel.Entities;

namespace ServiceLayer.RestaurantServices
{
    public class RestaurantUsersService
    {
        public readonly DelivryDB _context;
        public RestaurantUsersService(DelivryDB context)
        {
            _context = context;
        }

        public ResponseRestaurantUser AddRestaurantUser(AddRestaurantUserDTO dto,int CurrentUserID)
        {
            User CurrentUser = _context.Users.Find(CurrentUserID);
            if (dto.RoleInRestaurant != RoleInRestaurant.RestaurantOwner && dto.RoleInRestaurant != RoleInRestaurant.RestaurantStaff)
            {
                throw new Exception("Invalid Role");
            }

            if(dto.RoleInRestaurant == RoleInRestaurant.RestaurantOwner)
            {
                if(CurrentUser.Role != UserRole.Admin)
                {
                    throw new Exception("only Admin Can Change Role To Owner");
                }
            }

            if(CurrentUser.Role == UserRole.RestaurantOwner)
            {
                var RU = _context.RestaurantUsers.FirstOrDefault(r => r.UserID == CurrentUser.ID);
                if(RU == null)
                {
                    throw new Exception("You Are Not Owner On This Restaurant");

                }

                if (RU.RestaurantID != dto.RestaurantID )
                {
                    throw new Exception("You Are Not Owner On This Restaurant");
                }
            }

            var user = _context.Users.Find(dto.UserID);
            if(user == null)
            {
                throw new Exception("User Not Found");
            }
           
            var Restaurant = _context.Restaurants.Find(dto.RestaurantID);
            if(Restaurant == null)
            {
                throw new Exception("Restaurant Not Found");
            }
           
            var RestaurantUsertest = _context.RestaurantUsers.FirstOrDefault(r => r.UserID == dto.UserID && r.RestaurantID == dto.RestaurantID);
            if(RestaurantUsertest != null)
            {
                throw new Exception("This User Already Related To This Restaurant");
            }

            if (user.Role.ToString() != dto.RoleInRestaurant.ToString() || user.Role == UserRole.Admin || user.Role == UserRole.Customer || user.Role == UserRole.Driver)
            {
                throw new Exception("User Has Another Role");
            }
            RestaurantUsers RestaurantUser = new RestaurantUsers()
            {
                RestaurantID = dto.RestaurantID,
                UserID = dto.UserID,
                RoleInRestaurant = dto.RoleInRestaurant.ToString(),
                CreatedAt = DateTime.UtcNow
            };
            _context.RestaurantUsers.Add(RestaurantUser);
            _context.SaveChanges();
            return new ResponseRestaurantUser
            {
                ID = RestaurantUser.ID,
                RestaurantID = RestaurantUser.RestaurantID,
                UserID = RestaurantUser.UserID,
                RoleInRestaurant = RestaurantUser.RoleInRestaurant.ToString(),
                CreatedAt = DateTime.UtcNow
            };

        }
        public List<ResponseRestaurantUser> GetRestaurantUsers(int RestaurantID)
        {
            var Restaurant = _context.Restaurants.FirstOrDefault(r => r.ID == RestaurantID);
            if(Restaurant == null)
            {
                throw new Exception("Restaurant Not Found");
            }
            var Users = _context.RestaurantUsers.Where(r => r.RestaurantID == RestaurantID).ToList();
           
            List<ResponseRestaurantUser> res = new List<ResponseRestaurantUser>();
            foreach (var user in Users)
            {
                res.Add(new ResponseRestaurantUser()
                {   ID = user.ID,
                    RestaurantID = user.RestaurantID,
                    UserID = user.UserID,
                    RoleInRestaurant = user.RoleInRestaurant,
                    CreatedAt = user.CreatedAt
                });
            }
            return res;

        }
        public string RemoveRestaurantUser(int UserID , int RestaurantID,int CurrentUserID)
        {
            User CurrentUser = _context.Users.Find(CurrentUserID);
            if (CurrentUser == null)
            {
                throw new Exception("Invalid User");
            }
            var RestaurantUser = _context.RestaurantUsers.FirstOrDefault(r => r.UserID == UserID && r.RestaurantID == RestaurantID);
            if(RestaurantUser == null)
            {
                throw new Exception("RestaurantUser Not Found");
            }
            if(RestaurantUser.RoleInRestaurant == RoleInRestaurant.RestaurantOwner.ToString())
            {
                if(CurrentUser.Role != UserRole.Admin)
                {
                    throw new Exception("Admin Only Can Remove Owner");
                }
            }
            if(CurrentUser.Role == UserRole.RestaurantOwner)
            {
                var RestaurantUserr = _context.RestaurantUsers.FirstOrDefault(r => r.UserID == CurrentUser.ID && r.RestaurantID == RestaurantID);
                if(RestaurantUserr.RoleInRestaurant != RoleInRestaurant.RestaurantOwner.ToString())
                {
                    throw new Exception("You Are Not Owner On This Restaurant");
                }
            }
            _context.RestaurantUsers.Remove(RestaurantUser);
            _context.SaveChanges();
            return "RestaurantUser Deleted Succesfully";

        }

    }
}
