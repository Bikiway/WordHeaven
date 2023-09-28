using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using System.Threading.Tasks;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Helpers;
using System.Linq;

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
            await _userHelper.CheckRoleAsync("Employee");

            #region
            var user = await _userHelper.GetUserByEmailAsync("test@gmail.com");
            string completePath = Path.Combine(_environment.WebRootPath, "assets", "images", "profile-default-image.jpg");
            byte[] imagemBytes = System.IO.File.ReadAllBytes(completePath);

            if (user == null)
            {
                user = new User
                {
                    FirstName = "User",
                    LastName = "Admin",
                    Email = "test@gmail.com",
                    UserName = "test@gmail.com",
                    PhoneNumber = "123456789",
                    Address = "Rua A",
                    PostalCode = "2600-172",
                    Location = "Lisboa",
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

            if (!_context.Stores.Any())
            {
                AddStore("Chiado","Rua A", "Lisboa", "2600-111", "217207007", "WordHeaven.Chiado@mail.com");
                AddStore("Amadora","Rua AA", "Lisboa", "2600-121", "217207117", "WordHeaven.Amadora@mail.com");
                AddStore("Vila Nova de Gaia", "Rua Porto", "Vila Nova de Gaia", "2200-111", "217207303", "WordHeaven.VilaNovaDeGaia@mail.com");

                await _context.SaveChangesAsync();
            }


            #region

            var userEmployee = await _userHelper.GetUserByEmailAsync("testEmployee@gmail.com");
            string completePaths = Path.Combine(_environment.WebRootPath, "assets", "images", "profile-default-image.jpg");
            byte[] imagemByte = System.IO.File.ReadAllBytes(completePath);

            if (user == null)
            {
                user = new User
                {
                    FirstName = "Users",
                    LastName = "Employee",
                    Email = "testEmployee@gmail.com",
                    UserName = "testEmployee@gmail.com",
                    PhoneNumber = "123456789",
                    Address = "Rua B",
                    PostalCode = "2900-172",
                    Location = "Lisboa",
                    PictureSource = imagemBytes
                };

                var result = await _userHelper.AddUserAsync(user, "Aa1234567890");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }

                var token = await _userHelper.GenerateConfirmEmailTokenAsync(user);
                await _userHelper.EmailConfirmAsync(user, token);

                await _userHelper.AddUserToRoleAsync(user, "Employee");
            }

            var isRole = await _userHelper.IsUserInRoleAsync(user, "Employee");

            if (!isRole)
            {
                await _userHelper.AddUserToRoleAsync(user, "Employee");
            }

            if (!_context.Stores.Any())
            {
                AddStore("Chiado", "Rua A", "Lisboa", "2600-111", "217207007", "WordHeaven.Chiado@mail.com");
                AddStore("Amadora", "Rua AA", "Lisboa", "2600-121", "217207117", "WordHeaven.Amadora@mail.com");

                await _context.SaveChangesAsync();
            }

            #endregion
        }
        #endregion
        private void AddStore(string name, string address, string location, string postalCode, string phone, string email)
        {
            _context.Stores.Add(new Store
            {

                Name = name,
                Address = address,
                Location = location,
                PostalCode = postalCode,
                Phone = phone,
                Email = email,
                

            });
        }

       

    }
}