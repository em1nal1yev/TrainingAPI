using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Services;
using TelimAPI.Infrastructure.Services;

namespace TelimAPI.Infrastructure
{
    public static class ServiceRegistration
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IEmailService, SmtpEmailService>();

            return services;
        }
    }
}
