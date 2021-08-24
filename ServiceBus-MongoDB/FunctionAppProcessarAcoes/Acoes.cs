using System.Linq;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using FunctionAppProcessarAcoes.Data;

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
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
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