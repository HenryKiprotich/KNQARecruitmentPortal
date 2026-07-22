using System.Linq;
using System.Threading.Tasks;
using KNQARecruitmentPortal.Data;
using Microsoft.EntityFrameworkCore;

namespace KNQARecruitmentPortal.Services
{
    public class PermissionService(AppDbContext db) : IPermissionService
    {
        public async Task<bool> UserHasPermissionAsync(string userName, string permissionName)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(permissionName)) return false;

            var q = from u in db.Users
                    join r in db.Roles on u.RoleId equals r.Id
                    join rp in db.RolePermissions on r.Id equals rp.RoleId
                    join p in db.Permissions on rp.PermissionId equals p.Id
                    where u.UserName == userName && p.PermissionName == permissionName && u.Status == 1
                    select p;

            return await q.AnyAsync();
        }
    }
}
