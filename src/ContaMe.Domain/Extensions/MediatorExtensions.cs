using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ContaMe.Domain.Extensions
{
    public static class MediatorExtensions
    {
        public static void AddCustomMediator(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }
    }
}
