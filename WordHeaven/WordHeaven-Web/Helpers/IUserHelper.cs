using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Models.Users;

namespace WordHeaven_Web.Helpers
{
    public interface IUserHelper
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByIdAsync(string userId);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(LoginViewModel model);

        Task LogoutAsync();

        Task<bool> IsEmailConfirmedAsync(User user);

        Task<IdentityResult> EmailConfirmAsync(User user, string token);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> DeleteUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword);

        Task CheckRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<SignInResult> ValidatePasswordAsync(User user, string password);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);

        Task<string> GenerateConfirmEmailTokenAsync(User user);
    }
}