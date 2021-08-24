namespace FunctionAppProcessarAcoes.Models
{
    public class DadosAcao
    {
        private string _codigo;
        public string Codigo
        {
            get => _codigo;
            set => _codigo = value?.Trim().ToUpper();
        }

        public decimal? Valor { get; set; }

        private string _codigoCorretora;
        public string CodCorretora
        {
            get => _codigoCorretora;
            set => _codigoCorretora = value?.Trim().ToUpper();
        }

        public string NomeCorretora { get; set; }
    }
}