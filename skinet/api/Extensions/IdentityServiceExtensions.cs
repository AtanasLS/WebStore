using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
         IConfiguration config)
         {
            services.AddDbContext<AppIdentityDbContex>(opt =>
            {
                opt.UseSqlite(config.GetConnectionString("IdentityConnection"));
            });

            return services;
         }
    }
}