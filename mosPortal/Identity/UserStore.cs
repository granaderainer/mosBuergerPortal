using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mosPortal.Data;
using mosPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Extensions.Internal;

namespace mosPortal.Identity
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>, IUserRoleStore<User>
    {
        private readonly dbbuergerContext db;

        public UserStore(dbbuergerContext db)
        {
            this.db = db;
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.Password = passwordHash;

            return Task.FromResult((object) null);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Password);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrWhiteSpace(user.Password));
        }


        //UserRoleStore
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
            //return Task.FromResult((object)null);
            //throw new NotImplementedException();
        }

        public async Task<IList<string>> GetRolesAsync(User user, CancellationToken cancellationToken)
        {
            List<UserRole> userRoles = db.UserRole.Where(ur => ur.UserId == user.Id).ToList();
            List<string> roleNames = new List<string>();
            foreach (UserRole userRole in userRoles)
            {
                Role role = db.Role.Where(r => r.Id == userRole.RoleId).SingleOrDefault();
                roleNames.Add(role.Name);
            }

            return await Task.FromResult(roleNames);

            //throw new NotImplementedException();
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
            try
            {
                userRole = db.UserRole.Where(ur => ur.UserId == user.Id && ur.RoleId == role.Id).Single();
                //if (userRole == null)
                //{
                //    userRole.RoleId = role.Id;
                //    userRole.UserId = user.Id;
                //    db.UserRole.Add(userRole);
                //    db.SaveChanges();
                //}
                return Task.FromResult(true);
            }
            catch (Exception e)
            {
                return Task.FromResult(false);
            }

            //if (userRole != null)
            //{
            //    return Task.FromResult(true);
            //}
            //else
            //{
            //    return Task.FromResult(false);
            //}
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
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(nameof(SetUserNameAsync));
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(nameof(GetNormalizedUserNameAsync));
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            return Task.FromResult((object) null);
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            db.Add(user);

            await db.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException(nameof(UpdateAsync));
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            db.Remove(user);

            int i = await db.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(i == 1 ? IdentityResult.Success : IdentityResult.Failed());
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (int.TryParse(userId, out int id))
            {
                return await db.User.FindAsync(id);
            }

            return await Task.FromResult((User) null);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await db.User
                .AsAsyncEnumerable()
                .SingleOrDefault(p => p.UserName.Equals(normalizedUserName, StringComparison.OrdinalIgnoreCase),
                    cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
        }
    }
}