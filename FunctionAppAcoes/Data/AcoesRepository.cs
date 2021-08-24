using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Dapper;
using Dapper.Contrib.Extensions;
using FunctionAppAcoes.Models;

namespace FunctionAppAcoes.Data
{
    public class AcoesRepository
    {
        public void Save(DadosAcao dadosAcao)
        {
            using var conexao = new SqlConnection(
                Environment.GetEnvironmentVariable("BaseAcoesStaging"));
            conexao.Insert<HistoricoAcao>(new ()
            {
                Codigo = dadosAcao.Codigo,
                DataReferencia = DateTime.UtcNow.AddHours(-3),
                Valor = dadosAcao.Valor
            });
        }

        public IEnumerable<HistoricoAcao> GetAll()
        {
            using var conexao = new SqlConnection(
                Environment.GetEnvironmentVariable("BaseAcoesStaging"));
            
            return conexao.Query<HistoricoAcao>(
                "SELECT * FROM Acoes ORDER BY Id DESC");
        }
    }
}