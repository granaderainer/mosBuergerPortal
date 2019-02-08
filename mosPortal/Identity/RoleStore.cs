using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mosPortal.Data;
using mosPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Extensions.Internal;

namespace mosPortal.Identity
{
    public class RoleStore : IRoleStore<Role>
    {
        private dbbuergerContext db;
        private Role roles;

        public RoleStore(dbbuergerContext db)
        {
            this.db = db;
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            db.Add(role);
            await db.SaveChangesAsync(cancellationToken);

            return await Task.FromResult(IdentityResult.Success);
            //throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            db.Remove(role);
            int i = await db.SaveChangesAsync(cancellationToken);
            return await Task.FromResult(i == 1 ? IdentityResult.Success : IdentityResult.Failed());
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<Role> IRoleStore<Role>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (int.TryParse(roleId, out int id))
            {
                return await db.Role.FindAsync(id);
            }

            return await Task.FromResult((Role) null);
        }

        async Task<Role> IRoleStore<Role>.FindByNameAsync(string normalizedRoleName,
            CancellationToken cancellationToken)
        {
            return await db.Role.AsAsyncEnumerable()
                .SingleOrDefault(r => r.Name.Equals(normalizedRoleName, StringComparison.OrdinalIgnoreCase),
                    cancellationToken);
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
                db = null;
            }
        }
    }
}