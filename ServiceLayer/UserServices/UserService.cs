using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.UserDTOS;
using SystemModel.Entities;
namespace ServiceLayer.UserServices
{
    public class UserService
    {
        public readonly DelivryDB _context;
        public UserService(DelivryDB context)
        {
            _context = context;  
        }
        public List<UserResponseDTO> GetUsers()
        {
            var users = _context.Users.Where(u => u.Role != UserRole.Admin).ToList();
          
            List<UserResponseDTO> res = new List<UserResponseDTO>();
            foreach(var user in users)
            {
                res.Add( new UserResponseDTO()
                {
                    UserName = user.Name,
                    UserID = user.ID,
                    UserEmail = user.Email,
                    userRole = user.Role.ToString(),
                    UserPhone = user.Phone,
                    IsActive = user.IsActive

                });
            }
            return res;
        }
        public UserResponseDTO ChangeUserRole(ChangeRoleDTO dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == dto.UserID);
            if(user == null)
            {
                throw new Exception("User not found");
            }
            if(user.Role == UserRole.Admin)
            {
                throw new Exception("Cannot Change Admin Role");
            }
            if(dto.NewRole == UserRole.Admin)
            {
                throw new Exception("Cannot Change Role To Admin Role");
            }
            if(user.Role == dto.NewRole)
            {
                throw new Exception("User Already Has Same Role");
            }
            user.Role = dto.NewRole;
            _context.SaveChanges();
            
            return new UserResponseDTO()
            {
                UserName = user.Name , 
                UserID = user.ID , 
                UserEmail = user.Email , 
                UserPhone = user.Phone,
                userRole = user.Role.ToString(),
                IsActive = user.IsActive

            };

        }
        public UserResponseDTO GetUserByID(int UserID)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == UserID);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            return new UserResponseDTO()
            {
                UserID = user.ID,
                UserEmail = user.Email,
                UserPhone = user.Phone,
                userRole = user.Role.ToString(),
                UserName = user.Name,
                IsActive = user.IsActive
            };

        }
        public UserResponseDTO ActivateUser(int UserID)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == UserID);
            if(user == null)
            {
                throw new Exception("User Not Found");
            }
            if(user.Role == UserRole.Admin)
            {
                throw new Exception("Cannot Change Status Admin User");
            }
            if (user.IsActive == true)
            {
                throw new Exception("User Is Already Non Active");
            }
            user.IsActive = true;
            _context.SaveChanges();
            return new UserResponseDTO()
            {
                UserEmail = user.Email,
                UserID = user.ID,
                UserName = user.Name,
                UserPhone = user.Phone,
                userRole = user.Role.ToString(),
                IsActive = user.IsActive
            };


        }
        public UserResponseDTO DeActivateUser(int UserID)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == UserID);
            if (user == null)
            {
                throw new Exception("User Not Found");
            }
            if (user.Role == UserRole.Admin)
            {
                throw new Exception("Cannot Change Status Admin User");
            }
            if (user.IsActive == false)
            {
                throw new Exception("User Is Already Non Active");
            }
            user.IsActive = false;
            _context.SaveChanges();
            return new UserResponseDTO()
            {
                UserEmail = user.Email,
                UserID = user.ID,
                UserName = user.Name,
                UserPhone = user.Phone,
                userRole = user.Role.ToString(),
                IsActive = user.IsActive
            };


        }



    }
}
