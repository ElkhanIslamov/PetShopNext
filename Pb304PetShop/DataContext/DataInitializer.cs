using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pb304PetShop.Data;
using Pb304PetShop.DataContext.Entities;

namespace Pb304PetShop.DataContext
{
    public class DataInitializer
    {
        private readonly AppDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        //private readonly IConfiguration _configuration;
        private readonly SuperAdmin _superAdmin;

        public DataInitializer(AppDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IOptions<SuperAdmin> options)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _superAdmin = options.Value;
        }

        public async Task SeedData()
        {
            await _dbContext.Database.MigrateAsync();

            await CreateRoles();

            await CreateSuperAdmin();
        }

        public async Task CreateRoles()
        {
            foreach (var role in RoleConstants.AllRoles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public async Task CreateSuperAdmin()
        {
            if (await _userManager.FindByNameAsync(_superAdmin.UserName) != null)
                return;

            var superAdmin = new AppUser
            {
                FullName = _superAdmin.FullName,
                UserName = _superAdmin.UserName,
                Email = _superAdmin.Email
            };

            var result = await _userManager.CreateAsync(superAdmin, _superAdmin.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(superAdmin, RoleConstants.SuperAdmin);
            }
            else
            {
                //logging
            }
        }  
    }
}
