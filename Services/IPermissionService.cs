using System.Threading.Tasks;

namespace KNQARecruitmentPortal.Services
{
    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(string userName, string permissionName);
    }
}
