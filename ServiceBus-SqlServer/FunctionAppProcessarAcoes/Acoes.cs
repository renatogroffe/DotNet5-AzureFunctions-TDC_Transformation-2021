using System.Linq;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using FunctionAppProcessarAcoes.Data;
using FunctionAppProcessarAcoes.Models;

namespace FunctionAppProcessarAcoes
{
    public class Acoes
    {
        private readonly AcoesRepository _repository;

        public Acoes(AcoesRepository repository)
        {
            _repository = repository;
        }

        [Function("Acoes")]
        [OpenApiOperation(operationId: "Acoes", tags: new[] { "Acoes" }, Summary = "Acao", Description = "Consultar as cotações de Ações.", Visibility = OpenApiVisibilityType.Important)]
        //[OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Acao[]), Summary = "Consulta de cotações de Ações.", Description = "Consulta de cotações de Ações.")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("Acoes");
            
            var historicoAcoes = _repository.GetAll();
            logger.LogInformation($"No. de documentos encontrados: {historicoAcoes.Count()}");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteAsJsonAsync(historicoAcoes);
            return response;
        }
    }
}