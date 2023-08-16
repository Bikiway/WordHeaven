using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using System.Threading.Tasks;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Helpers;

namespace WordHeaven_Web.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IWebHostEnvironment _environment;

        public SeedDb(DataContext context, IUserHelper userHelper, IWebHostEnvironment environment)
        {
            _context = context;
            _userHelper = userHelper;
            _environment = environment;
        }

        public UserManager<User> UserManager { get; }

        public async Task SeedAsync()
        {
            await _context.Database.MigrateAsync();

            await _userHelper.CheckRoleAsync("Admin");
            await _userHelper.CheckRoleAsync("employee");

            var user = await _userHelper.GetUserByEmailAsync("test@gmail.com");
            string completePath = Path.Combine(_environment.WebRootPath, "assets", "images", "profile-default-image.jpg");
            byte[] imagemBytes = System.IO.File.ReadAllBytes(completePath);

            if (user == null)
            {
                user = new User
                {
                    FirstName = "User",
                    LastName = "Test1",
                    Email = "test@gmail.com",
                    UserName = "test@gmail.com",
                    PhoneNumber = "123456789",
                    Address = "Rua A",
                    PictureSource = imagemBytes
                };

                var result = await _userHelper.AddUserAsync(user, "Aa1234567890");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                var token = await _userHelper.GenerateConfirmEmailTokenAsync(user);
                await _userHelper.EmailConfirmAsync(user, token);

                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

            var isInRole = await _userHelper.IsUserInRoleAsync(user, "Admin");

            if (!isInRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Admin");
            }

        }
    }
}