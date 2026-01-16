using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.MenuItemDTOS;
using SystemModel.Entities;

namespace ServiceLayer.MenuItemServices
{
    public class MenuItemService
    {
        private readonly DelivryDB _context;
        public MenuItemService(DelivryDB context)
        {
            _context = context;
        }

        public MenuItemResponseDTO CreateMenuItem(CreateMenuItemDTO dto,int CurrentUserID)
        {
            if(string.IsNullOrWhiteSpace(dto.Name) || dto.Price<=0 )
            {
                throw new Exception("Invalid MenuItem Data");
            }
            var User = _context.Users.FirstOrDefault(u => u.ID == CurrentUserID);
            if(User == null)
            {
                throw new Exception("User Not Found");
            }
            if(User.Role == UserRole.RestaurantOwner || User.Role == UserRole.RestaurantStaff)
            {
                var restaurantUser = _context.RestaurantUsers.FirstOrDefault(r => r.RestaurantID == dto.RestaurantID && r.UserID == CurrentUserID);
                if(restaurantUser == null)
                {
                    throw new Exception("You Are Not Able To Add MenuItem For Another Restaurant");
                }
            }
            var i = _context.MenuItems.FirstOrDefault(m => m.RestaurantID == dto.RestaurantID && m.Name == dto.Name);
            if(i != null)
            {
                throw new Exception("This ItemName Is For Another Item");
            }
            var MenuItem = new MenuItem()
            {
                Name = dto.Name,
                Price = dto.Price,
                Description = dto.Description,
                RestaurantID = dto.RestaurantID,
                IsAvailable = dto.IsAvailable,
            };
            _context.MenuItems.Add(MenuItem);
            _context.SaveChanges();
            return new MenuItemResponseDTO()
            {
                ID = MenuItem.ID,
                RestaurantID = MenuItem.RestaurantID,
                Name = MenuItem.Name,
                Description = MenuItem.Description,
                Price = MenuItem.Price,
                IsAvailable = MenuItem.IsAvailable,
            };
        }
        public List<MenuItemResponseDTO> GetMenuItemsByRestaurantID(int RestaurantID)
        {   
            List<MenuItemResponseDTO> res = new List<MenuItemResponseDTO>();
            var Menu = _context.MenuItems.Where(m => m.RestaurantID == RestaurantID).ToList();
            foreach(var m in Menu)
            {
                res.Add(new MenuItemResponseDTO()
                {   
                    ID = m.ID,
                    Name = m.Name,
                    Price = m.Price,
                    Description = m.Description,
                    IsAvailable = m.IsAvailable,
                    RestaurantID = m.RestaurantID,
                });
            }
            return res;
        }
        public List<MenuItemResponseDTO> GetAvailableMenuItemsByRestaurantID(int RestaurantID)
        {
            List<MenuItemResponseDTO> res = new List<MenuItemResponseDTO>();
            var Menu = _context.MenuItems.Where(m => m.RestaurantID == RestaurantID && m.IsAvailable == true).ToList();
            foreach (var m in Menu)
            {
                res.Add(new MenuItemResponseDTO()
                {
                    ID = m.ID,
                    Name = m.Name,
                    Price = m.Price,
                    Description = m.Description,
                    IsAvailable = m.IsAvailable,
                    RestaurantID = m.RestaurantID,
                });
            }
            return res;
        }
        public List<MenuItemResponseDTO> GetNotAvailableMenuItemsByRestaurantID(int RestaurantID)
        {
            List<MenuItemResponseDTO> res = new List<MenuItemResponseDTO>();
            var Menu = _context.MenuItems.Where(m => m.RestaurantID == RestaurantID && m.IsAvailable == false).ToList();
            foreach (var m in Menu)
            {
                res.Add(new MenuItemResponseDTO()
                {
                    ID = m.ID,
                    Name = m.Name,
                    Price = m.Price,
                    Description = m.Description,
                    IsAvailable = m.IsAvailable,
                    RestaurantID = m.RestaurantID,
                });
            }
            return res;
        }
        public MenuItemResponseDTO GetMenuItemByID(int ItemID)
        {
            var item = _context.MenuItems.FirstOrDefault(m => m.ID == ItemID);
            if (item == null)
            {
                throw new Exception("MenuItem Not Found");
            }
            return new MenuItemResponseDTO()
            {
                ID = item.ID,
                Name = item.Name,
                Price = item.Price,
                Description = item.Description,
                IsAvailable = item.IsAvailable,
                RestaurantID = item.RestaurantID,
            };
        }
        public MenuItemResponseDTO UpdateMenuItem(int ItemID,UpdateMenuItemDTO dto,int CurrentUserID)
        {
            if(string.IsNullOrWhiteSpace(dto.Name) || dto.Price<=0)
            {
                throw new Exception("Invalid MenuItem Data");
            }
            var User = _context.Users.Find(CurrentUserID);
            if (User == null)
            {
                throw new Exception("User Not Found");
            }
          
            var item = _context.MenuItems.Find(ItemID);
            if(item == null)
            {
                throw new Exception("Item Not Found");
            }
            if (User.Role == UserRole.RestaurantOwner || User.Role == UserRole.RestaurantStaff)
            {
                var restaurantuser = _context.RestaurantUsers.FirstOrDefault(r => r.RestaurantID == item.RestaurantID && r.UserID == CurrentUserID);
                if(restaurantuser == null)
                {
                    throw new Exception("You Not Able To Update MenuItem For Another Restaurant");
                }
            }
            var itemtest = _context.MenuItems.FirstOrDefault(m => m.Name == dto.Name && m.RestaurantID == item.RestaurantID && m.ID != ItemID);
            if (itemtest != null) 
            {
                throw new Exception("This New Name Is For Another Item");
            }
            item.Name = dto.Name;
            item.Description = dto.Description;
            item.Price = dto.Price;
            item.IsAvailable = dto.IsAvailable;
            _context.SaveChanges();

            return new MenuItemResponseDTO()
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                RestaurantID = item.RestaurantID,
                ID = item.ID,
            };

        }
        public MenuItemResponseDTO ChangeAvailability(int ItemID,bool Avialable,int CurrentUserID)
        {
            var User = _context.Users.Find(CurrentUserID);
            if (User == null)
            {
                throw new Exception("User Not Found");
            }
            var item = _context.MenuItems.Find(ItemID);
            if (item == null)
            {
                throw new Exception("Item Not Found");
            }
            if (User.Role == UserRole.RestaurantOwner || User.Role == UserRole.RestaurantStaff)
            {
                var restaurantuser = _context.RestaurantUsers.FirstOrDefault(r => r.RestaurantID == item.RestaurantID && r.UserID == CurrentUserID);
                if (restaurantuser == null)
                {
                    throw new Exception("You Not Able To Update MenuItem For Another Restaurant");
                }
            }
            if(item.IsAvailable == Avialable)
            {
                if (Avialable == false)
                {
                    throw new Exception("Item Is Already Not Available");
                }

                if (Avialable == true)
                {
                    throw new Exception("Item Is Already Available");
                }
                
            }
            item.IsAvailable = Avialable;
            _context.SaveChanges();

            return new MenuItemResponseDTO()
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                IsAvailable = item.IsAvailable,
                RestaurantID = item.RestaurantID,
                ID = item.ID,
            };

        }
        public string DeleteMenuItem(int ItemID,int CurrentUserID)
        {
            var User = _context.Users.Find(CurrentUserID);
            if (User == null)
            {
                throw new Exception("User Not Found");
            }
            var item = _context.MenuItems.Find(ItemID);
            if (item == null)
            {
                throw new Exception("Item Not Found");
            }
            if (User.Role == UserRole.RestaurantOwner || User.Role == UserRole.RestaurantStaff)
            {
                var restaurantuser = _context.RestaurantUsers.FirstOrDefault(r => r.RestaurantID == item.RestaurantID && r.UserID == CurrentUserID);
                if (restaurantuser == null)
                {
                    throw new Exception("You Not Able To Update MenuItem For Another Restaurant");
                }
            }
            _context.MenuItems.Remove(item);
            _context.SaveChanges();
            return "MenuItem Deleted Succesfully";
        }
        
    }
}
