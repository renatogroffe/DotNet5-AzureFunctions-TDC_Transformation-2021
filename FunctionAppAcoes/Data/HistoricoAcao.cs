using System;
using Dapper.Contrib.Extensions;

namespace FunctionAppAcoes.Data
{
    [Table("Acoes")]
    public class HistoricoAcao
    {
        [Key]
        public int Id { get; set; }
        public string Codigo { get; set; }
        public DateTime DataReferencia { get; set; }
        public double? Valor { get; set; }
    }
}