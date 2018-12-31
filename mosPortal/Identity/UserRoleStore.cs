using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mosPortal.Identity
{
    using System.Threading;
    using System.Threading.Tasks;
    using Data;
    using Microsoft.AspNetCore.Identity;
    using mosPortal.Models;
    public class UserRoleStore : IUserRoleStore<User>
    {
        private readonly dbbuergerContext db;
        public UserRoleStore(dbbuergerContext db)
        {
            this.db = db;
        }

        public async Task AddToRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            Role role = db.Role.Where(r => r.Name == roleName).SingleOrDefault();
            UserRole userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            db.Add(userRole);
            await db.SaveChangesAsync(cancellationToken);
            return;
            //return Task.FromResult((object)null);
            //throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        { }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            List<UserRole> userRoles = db.UserRole.Where(ur => ur.UserId == user.Id).ToList();
            List<string> roleNames = new List<string>();
            foreach (UserRole userRole in userRoles)
            {
                Role role = db.Role.Where(r => r.Id == userRole.Id).SingleOrDefault();
                roleNames.Add(role.Name);
            }
            return await Task.FromResult(roleNames);

            //throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<User>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            Role role = db.Role.Where(r => r.Name == roleName).SingleOrDefault();
            List<UserRole> userRoles = db.UserRole.Where(ur => ur.RoleId == role.Id).ToList();
            List<User> users = new List<User>();
            foreach (UserRole userRole in userRoles)
            {
                users.Add(db.User.Where(u => u.Id == userRole.UserId).SingleOrDefault());
            }

            return await Task.FromResult(users);
        }

        public Task<bool> IsInRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            Role role = db.Role.Where(r => r.Name == roleName).SingleOrDefault();
            UserRole userRole = null;
            userRole = db.UserRole.Where(ur => ur.UserId == user.Id && ur.RoleId == role.Id).Single();
            if(userRole !=null)
            {
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public async Task RemoveFromRoleAsync(User user, string roleName, CancellationToken cancellationToken)
        {
            Role role = db.Role.Where(r => r.Name == roleName).SingleOrDefault();
            UserRole userRole = null;
            userRole = db.UserRole.Where(ur => ur.UserId == user.Id && ur.RoleId == role.Id).Single();
            if (userRole != null)
            {
                db.Remove(userRole);
                int i = await db.SaveChangesAsync(cancellationToken);
                return;
            }
            else
            {
                return;
            }
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
