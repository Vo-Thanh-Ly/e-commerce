using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Models;

namespace Sell___cleaning_services_e_commerce.Data;

public class Sell___cleaning_services_e_commerceContext : IdentityDbContext<User>
{
    public Sell___cleaning_services_e_commerceContext(DbContextOptions<Sell___cleaning_services_e_commerceContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
