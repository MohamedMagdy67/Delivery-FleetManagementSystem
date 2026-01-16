using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SystemContext.SystemDbContext;
using SystemDTOS.DriverDTOS;
using SystemDTOS.ReviewDTOS;
using SystemModel.Entities;
namespace ServiceLayer.ReviewServices
{
    public class ReviewService
    {
        private readonly DelivryDB _context;
        public ReviewService(DelivryDB context)
        {
            _context = context;

        }
        public ReviewResponseForRestaurantDTO CreateReviewForRestaurant(CreateReviewForRestaurantDTO dto , int FromUserID)
        {
            if (dto.Rating < 1 || dto.Rating > 5 || dto.ToRestaurantID <= 0 || dto.OrderID <= 0)
            {
                throw new Exception("Invalid Review Details");
            }
            var Order = _context.Orders.Find(dto.OrderID);
            if (Order == null)
            {
                throw new Exception("Order Not Found");
            }
            if (Order.RestaurantID != dto.ToRestaurantID)
            {
                throw new Exception("This Order Is Not For This Restaurant");
            }
            if(Order.CustomerID != FromUserID)
            {
                throw new Exception("This Order Not For This Customer");
            }
            var Review = _context.Reviews.FirstOrDefault(r => r.ToRestaurantID == dto.ToRestaurantID && r.OrderID == dto.OrderID && r.FromUserID == FromUserID);
            if (Review != null)
            {
                throw new Exception("You Can Not Review Again");
            }
            var review = new Review
            {
                OrderID = dto.OrderID,
                ToRestaurantID = dto.ToRestaurantID,
                FromUserID = FromUserID,
                Rating = dto.Rating,
                Comment = dto.Comment,
            };
            _context.Reviews.Add(review);
            _context.SaveChanges();
            var resReviews = GetRestaurantReviews(Order.RestaurantID);
            decimal TotalRating = 0;
            foreach(var r in resReviews)
            {
                TotalRating += r.Rating;
            }
            TotalRating /= (resReviews.Count);
            var Restaurant = _context.Restaurants.FirstOrDefault(r => r.ID == Order.RestaurantID);
            Restaurant.Rating = TotalRating;
            _context.SaveChanges();
            return new ReviewResponseForRestaurantDTO
            {
                ReviewID = review.ID,
                OrderID = review.OrderID,
                ToRestaurantID = (int)review.ToRestaurantID,
                FromUserID = review.FromUserID,
                Rating = review.Rating,
                Comment = review.Comment,
                
            };

        }
        public ReviewResponseForDriverDTO CreateReviewForDriver(CreateReviewForDriverDTO dto,int FromUserID)
        {
            if (dto.Rating < 1 || dto.Rating > 5 || dto.ToDriverID <= 0 || dto.OrderID <= 0)
            {
                throw new Exception("Invalid Review Details");
            }
            var Order = _context.Orders.Find(dto.OrderID);
            if (Order == null)
            {
                throw new Exception("Order Not Found");
            }
            if (Order.DriverID != dto.ToDriverID)
            {
                throw new Exception("This Order Is Not For This Restaurant");
            }
            if (Order.CustomerID != FromUserID)
            {
                throw new Exception("This Order Not For This Customer");
            }
            var Review = _context.Reviews.FirstOrDefault(r => r.ToUserID == dto.ToDriverID && r.OrderID == dto.OrderID && r.FromUserID == FromUserID);
            if (Review != null)
            {
                throw new Exception("You Can Not Review Again");
            }
            var review = new Review
            {
                OrderID = dto.OrderID,
                ToUserID = dto.ToDriverID,
                FromUserID = FromUserID,
                Rating = dto.Rating,
                Comment = dto.Comment,
            };
            _context.Reviews.Add(review);
            _context.SaveChanges();
            var DriverReviews = GetDriverReviews((int)Order.DriverID);
            decimal TotalRating = 0;
            foreach (var D in DriverReviews)
            {
                TotalRating += D.Rating;
            }
            TotalRating /= (DriverReviews.Count);
            var Driver = _context.Drivers.FirstOrDefault(r => r.ID == Order.DriverID);
            if(Driver == null)
            {
                throw new Exception("Driver Not Found");
            }
            Driver.Rating = TotalRating;
            _context.SaveChanges();
            return new ReviewResponseForDriverDTO
            {
                ReviewID = review.ID,
                OrderID = review.OrderID,
                ToDriverID = (int)review.ToUserID,
                FromUserID = review.FromUserID,
                Rating = review.Rating,
                Comment = review.Comment,

            };

        }
        public List<ReviewResponseForDriverDTO> GetDriverReviews(int DriverID)
        {   
            if(DriverID <= 0)
            {
                throw new Exception("Driver Not Found");
            }
            var Driver = _context.Drivers.Find(DriverID);
            if(Driver == null)
            {
                throw new Exception("Driver Not Found");
            }
            var DriverReviews = _context.Reviews.Where(r => r.ToUserID == DriverID).ToList();
            List<ReviewResponseForDriverDTO> reviews = new List<ReviewResponseForDriverDTO>();
            foreach(var r in DriverReviews)
            {
                reviews.Add(new ReviewResponseForDriverDTO
                {
                    ReviewID = r.ID,
                    FromUserID = r.FromUserID,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ToDriverID = (int)r.ToUserID,
                    OrderID = r.OrderID
                });
            }
            return reviews;

        }
        public List<ReviewResponseForRestaurantDTO> GetRestaurantReviews(int RestaurantID)
        {   
            if(RestaurantID <= 0)
            {
                throw new Exception("Restaurant Not Found");
            }
            var Restaurant = _context.Restaurants.Find(RestaurantID);
            if(Restaurant == null)
            {
                throw new Exception("Restaurant Not Found");
            }
            var RestaurantReviews = _context.Reviews.Where(r => r.ToRestaurantID == RestaurantID).ToList();
            List<ReviewResponseForRestaurantDTO> reviews = new List<ReviewResponseForRestaurantDTO>();
            foreach(var r in RestaurantReviews)
            {
                reviews.Add(new ReviewResponseForRestaurantDTO
                {
                    ReviewID = r.ID,
                    FromUserID = r.FromUserID,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    ToRestaurantID = (int)r.ToRestaurantID,
                    OrderID = r.OrderID
                });
            }
            return reviews;

        }
        


    }
}
