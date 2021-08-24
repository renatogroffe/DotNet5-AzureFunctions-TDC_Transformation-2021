using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using FunctionAppAcoes.Data;
using FunctionAppAcoes.Messaging;
using FunctionAppAcoes.Models;
using FunctionAppAcoes.Validators;

namespace FunctionAppAcoes
{
    public class Acoes
    {
        private const string MSG_ERRO_DESERIALIZACAO = "Erro durante a deserializacao dos dados da Acao...";
        private const string MSG_ERROS_VALIDACAO = "Dados invalidos para a Acao";
        private const string MSG_ACAO_REGISTRADA = "Acao cadastrada com sucesso!";
        private readonly AcoesRepository _repository;
        private readonly AcoesTopicSender _acoesTopicSender;

        public Acoes(AcoesRepository repository,
            AcoesTopicSender acoesTopicSender)
        {
            _repository = repository;
            _acoesTopicSender = acoesTopicSender;
        }

        [Function(nameof(Acoes.RegistrarAcao))]
        [OpenApiOperation(operationId: "Acoes", tags: new[] { "Acoes" }, Summary = "Acao", Description = "Cadastrar a cotação de uma ação.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(DadosAcao), Required = true, Description = "Objeto contendo os dados da cotação de uma ação")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Resultado), Summary = "Resultado da inclusão da cotação de uma ação", Description = "Resultado da inclusão da cotação de uma ação")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(Resultado), Summary = "Falha na inclusão da cotação de uma ação", Description = "Falha na inclusão da cotação de uma ação")]
        public async Task<HttpResponseData> RegistrarAcao([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger(nameof(Acoes.RegistrarAcao));
            logger.LogInformation("Iniciando o cadastramento dos dados da Acao");

            Resultado resultado = new (); // new expression = novidade do C# 9 + .NET 5

            DadosAcao dadosAcao = null;
            try
            {
                dadosAcao = await req.ReadFromJsonAsync<DadosAcao>();
            }
            catch
            {
                resultado.Erro = true;
                resultado.Mensagem = MSG_ERRO_DESERIALIZACAO;
                logger.LogError(MSG_ERRO_DESERIALIZACAO);
            }

            if (dadosAcao is not null) // is not = novidade do C# 9 + .NET 5
            {
                var validationResult = new DadosAcaoValidator().Validate(dadosAcao);
                if (validationResult.IsValid)
                {
                    await _acoesTopicSender.SendAsync(dadosAcao);
                    logger.LogInformation("Mensagem enviada para o Azure Service Bus");

                    _repository.Save(dadosAcao);
                    logger.LogInformation(MSG_ACAO_REGISTRADA);
                    resultado.Mensagem = MSG_ACAO_REGISTRADA;
                }
                else
                {
                    logger.LogError(MSG_ERROS_VALIDACAO);
                    resultado.Erro = true;

                    var descricaoErros = new StringBuilder($"{MSG_ERROS_VALIDACAO} : |");
                    foreach (var error in validationResult.Errors)
                    {
                        logger.LogError($" ## {error.ErrorMessage}");
                        descricaoErros.Append($" {error.ErrorMessage} |");
                    }
                    resultado.Mensagem = descricaoErros.ToString();
                }
            }            

            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(resultado);
            response.StatusCode =
                resultado.Erro ? HttpStatusCode.BadRequest : HttpStatusCode.OK;
            return response;            
        }

        [Function(nameof(Acoes.ListarAcoes))]
        [OpenApiOperation(operationId: "Acoes", tags: new[] { "Acoes" }, Summary = "Acao", Description = "Consultar as cotações de Ações.", Visibility = OpenApiVisibilityType.Important)]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(HistoricoAcao[]), Summary = "Histórico de cotações de Ações.", Description = "Histórico de cotações de Ações.")]
        public async Task<HttpResponseData> ListarAcoes([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger(nameof(Acoes.ListarAcoes));

            var historicoAcoes = _repository.GetAll();
            logger.LogInformation(
                $"No. de registros encontrados: {historicoAcoes.Count()}");

            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(historicoAcoes);
            return response;
        }
    }
}