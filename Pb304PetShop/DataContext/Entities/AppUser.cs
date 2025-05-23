using Microsoft.AspNetCore.Identity;

namespace Pb304PetShop.DataContext.Entities
{
    public class AppUser : IdentityUser
    {
        public required string FullName { get; set; }
    }
}
