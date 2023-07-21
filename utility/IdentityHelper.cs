using Microsoft.IdentityModel.Tokens;
using PeyulErp.Models;
using PeyulErp.Settings;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PeyulErp.Utility
{
    public class IdentityHelper
    {
        public const string AdminUserPolicyName = "Admin";
        public const string AdminUserClaimName = "admin";
    }
}