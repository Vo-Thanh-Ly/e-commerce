using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    public class UserClaims : IdentityUserClaim<string>
    {
    }
}
