using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using FunctionAppAcoes.Models;

namespace FunctionAppAcoes.Messaging
{
    public class AcoesTopicSender
    {
        private readonly TopicClient _client;
        private readonly JsonSerializerOptions _serializerOptions;

        public AcoesTopicSender()
        {
            _client = new TopicClient(
                Environment.GetEnvironmentVariable("AzureServiceBus_Connection"),
                Environment.GetEnvironmentVariable("AzureServiceBus_Topic"));
            
            _serializerOptions = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task SendAsync(DadosAcao acao)
        {
            await _client.SendAsync(
                new Message(Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(new Acao()
                    {
                        Codigo = acao.Codigo,
                        Valor = acao.Valor,
                        CodCorretora = Environment.GetEnvironmentVariable("CodCorretora"),
                        NomeCorretora = Environment.GetEnvironmentVariable("NomeCorretora")
                    },
                    _serializerOptions))));
        }
    }
}