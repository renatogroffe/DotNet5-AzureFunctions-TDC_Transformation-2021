using MongoDB.Bson;

namespace FunctionAppProcessarAcoes.Documents
{
    public class AcaoDocument
    {
        public ObjectId _id { get; set; }
        public string Codigo { get; set; }
        public double Valor { get; set; }
        public string DataReferencia { get; set; }
        public Corretora CorretoraResponsavel { get; set; }
    }

    public class Corretora
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
    }
}