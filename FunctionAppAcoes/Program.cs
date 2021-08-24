using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FunctionAppAcoes.Data;
using FunctionAppAcoes.Messaging;

namespace FunctionAppAcoes
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(worker => worker.UseNewtonsoftJson())
                .ConfigureServices(services =>
                {
                    services.AddScoped<AcoesRepository>();
                    services.AddScoped<AcoesTopicSender>();
                })
                .ConfigureOpenApi()
                .Build();

            host.Run();
        }
    }
}