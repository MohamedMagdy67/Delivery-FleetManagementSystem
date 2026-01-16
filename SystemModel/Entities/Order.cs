using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemModel.Entities
{
    #region ENUMS
    public enum OrderStatus
    {
        Pending,
        Accepted,
        Assigned,
        PickedUp,
        Delivered,
        Cancelled
    }
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded
    }
    #endregion
    public class Order
    {
        public int ID { get; set; }
        public int CustomerID { get; set; }
        public int RestaurantID { get; set; }
        public int? DriverID { get; set; }
        public int? VehicleID { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string PackageType { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DeliveryFee { get; set; }
        public OrderStatus Status { get;  set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EstimatedPickupAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        #region Navigation Properties
        public virtual Restaurant Restaurant { get; set; }
        public virtual Driver Driver { get; set; }
        public virtual User Customer { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }  = new HashSet<Review>();
        public virtual Payment Payment { get; set; }
        public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new HashSet<OrderStatusHistory>();
        public virtual Vehicle Vehicle { get; set; }
        #endregion
        #region Domain Rules
        public void AcceptOrder()
        {
            if (Status == OrderStatus.Pending) {  Status = OrderStatus.Accepted; }
            else 
            {
                throw new Exception("\"Cannot accept order unless it is Created\"");

            }
        }
        public void AssignDriver(int driverID)
        {
            if (Status == OrderStatus.Accepted && DriverID == null) { Status = OrderStatus.Assigned; DriverID = driverID; }
            else 
            {
                throw new Exception("Cannot assign driver: either status is not Accepted or driver is already assigned");
            }
        }
        public void PickupOrder()
        {
            if (Status == OrderStatus.Assigned) { Status = OrderStatus.PickedUp; }
            else
            {
                throw new Exception("Cannot pick up order unless it is AssignedToDriver");
            }
        }
        public void DeliverOrder()
        {
            if (Status == OrderStatus.PickedUp) { Status = OrderStatus.Delivered; }
            else
            {
                throw new Exception("Cannot deliver order unless it is PickedUp");
            }
        }
        public void CancelOrder()
        {
            if (Status == OrderStatus.Pending || Status == OrderStatus.Accepted) { Status = OrderStatus.Cancelled; }
            else
            {
                throw new Exception("Cannot cancel order after it is Delivered or invalid status");
            }
        }
        #endregion
    }
}
