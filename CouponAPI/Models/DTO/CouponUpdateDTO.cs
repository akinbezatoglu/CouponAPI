using Microsoft.AspNetCore.Http.HttpResults;

namespace CouponAPI.Models.DTO
{
    public class CouponUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Percent { get; set; }
        public bool IsActive { get; set; }
    }
}
