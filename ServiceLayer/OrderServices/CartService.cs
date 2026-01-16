using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.OrderDTOS;

namespace ServiceLayer.OrderServices
{
    public class CartService
    {
        private readonly DelivryDB _context;
        public CartService(DelivryDB context)
        {
            _context = context;                
        }
        public CartResponseDTO CreateCart(List<CartDTO> dto)
        {
            List<int> ItemIDs = new List<int>();
            List<int> Quantity = new List<int>();
            foreach(var i in dto)
            {
                ItemIDs.Add(i.ItemID);
                Quantity.Add(i.Quantity);
            }
            var items = _context.MenuItems.Where(i => ItemIDs.Contains(i.ID)).ToList();
            if (items.Count <= 0)
            {
                throw new Exception("Items Not Found");
            }
            decimal totalprice = 0;
            string PackageType = "";
            int n = 0;
            foreach(var i in items)
            {
                totalprice += i.Price * Quantity[n];

                if (PackageType == "")
                {
                    PackageType += $"{Quantity[n]} {i.Name}";
                }
                else
                {
                    PackageType += $",{Quantity[n]} {i.Name}";
                }

                n++;
            }


            return new CartResponseDTO
            {
                TotalPrice = totalprice,
                PackageType = PackageType,
            };
        }
    }
}
