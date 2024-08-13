using CouponAPI.Models;

namespace CouponAPI.Data
{
    public static class CouponStore
    {
        public static List<Coupon> couponList = new List<Coupon>
        {
            new Coupon { Id = 1, Name = "100FF", Percent = 10, IsActive = true},
            new Coupon { Id = 2, Name = "200FF", Percent = 20, IsActive = false}
        };
    }
}
