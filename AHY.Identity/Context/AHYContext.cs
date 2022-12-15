using AHY.Identity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AHY.Identity.Context
{
    public class AHYContext : IdentityDbContext<AppUser,AppRole,int>
    {
        public AHYContext(DbContextOptions options) : base(options)
        {

        }
    }
}
