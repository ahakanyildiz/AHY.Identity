using Microsoft.AspNetCore.Identity;
using System;

namespace AHY.Identity.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public DateTime CreateTime { get; set; }
    }
}
