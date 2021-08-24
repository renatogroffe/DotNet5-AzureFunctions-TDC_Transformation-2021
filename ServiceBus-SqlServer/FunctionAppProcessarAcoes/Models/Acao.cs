using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace FunctionAppProcessarAcoes.Models
{
    public class Acao
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public DateTime DataReferencia { get; set; }
        [Column(TypeName = "numeric(10,4)")]
        public decimal? Valor { get; set; }
        public string CodCorretora { get; set; }
        public string NomeCorretora { get; set; }
    }
}