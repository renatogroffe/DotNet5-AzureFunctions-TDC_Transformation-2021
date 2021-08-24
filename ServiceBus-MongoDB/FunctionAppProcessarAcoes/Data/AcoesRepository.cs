using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using FunctionAppProcessarAcoes.Documents;
using FunctionAppProcessarAcoes.Models;

namespace FunctionAppProcessarAcoes.Data
{
    public class AcoesRepository
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<AcaoDocument> _historico;

        public AcoesRepository()
        {
            _client = new MongoClient(
                Environment.GetEnvironmentVariable("MongoDB_Connection"));
            _db = _client.GetDatabase(
                Environment.GetEnvironmentVariable("MongoDB_Database"));
            _historico = _db.GetCollection<AcaoDocument>(
                Environment.GetEnvironmentVariable("MongoDB_Collection"));
        }

        public void Save(DadosAcao dadosAcao)
        {
            var document = new AcaoDocument();
            document.Codigo = dadosAcao.Codigo;
            document.Valor = dadosAcao.Valor.Value;
            document.DataReferencia = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            document.CorretoraResponsavel = new ()
            {
                Codigo = dadosAcao.CodCorretora,
                Nome = dadosAcao.NomeCorretora
            };

            _historico.InsertOne(document);
        }

        public IEnumerable<AcaoDocument> GetAll()
        {
            return _historico.Find(all => true).ToList()
                .OrderByDescending(a => a.DataReferencia);
        }
    }
}