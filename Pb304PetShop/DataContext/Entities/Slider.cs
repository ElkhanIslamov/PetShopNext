using Microsoft.EntityFrameworkCore;

namespace Pb304PetShop.DataContext.Entities
{
    public class Slider
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? Description { get; set; }
    }
}
