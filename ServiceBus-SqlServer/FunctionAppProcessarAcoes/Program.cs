using System;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using FunctionAppProcessarAcoes.Data;

namespace FunctionAppProcessarAcoes
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
                .ConfigureServices(services =>
                {
                    services.AddDbContext<AcoesContext>(
                        options => options.UseSqlServer(
                            Environment.GetEnvironmentVariable("BaseAcoes")));
                    services.AddScoped<AcoesRepository>();
                })
                .ConfigureOpenApi()
                .Build();

            host.Run();
        }
    }
}