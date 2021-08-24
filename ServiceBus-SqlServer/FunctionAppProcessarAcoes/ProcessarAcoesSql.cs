using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FunctionAppProcessarAcoes.Data;
using FunctionAppProcessarAcoes.Models;
using FunctionAppProcessarAcoes.Validators;

namespace FunctionAppProcessarAcoes
{
    public class ProcessarAcoesSql
    {
        private readonly AcoesRepository _repository;

        public ProcessarAcoesSql(AcoesRepository repository)
        {
            _repository = repository;
        }

        [Function("ProcessarAcoesSql")]
        public void Run([ServiceBusTrigger("topic-acoes", "efcore", Connection = "AzureServiceBus_Connection")] string mySbMsg, FunctionContext context)
        {
            var logger = context.GetLogger("ProcessarAcoes");
            logger.LogInformation($"Dados recebidos: {mySbMsg}");

            DadosAcao dadosAcao = null;
            try
            {
                dadosAcao = JsonSerializer.Deserialize<DadosAcao>(mySbMsg,
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                logger.LogError("Erro durante a deserializacao!");
            }

            if (dadosAcao != null)
            {
                var validationResult = new DadosAcaoValidator().Validate(dadosAcao);
                if (validationResult.IsValid)
                {
                    _repository.Save(dadosAcao);
                    logger.LogInformation("Acao registrada com sucesso!");
                }
                else
                {
                    logger.LogError("Dados invalidos para a Acao");
                    foreach (var error in validationResult.Errors)
                        logger.LogError($" ## {error.ErrorMessage}");
                }
            }
        }
    }
}