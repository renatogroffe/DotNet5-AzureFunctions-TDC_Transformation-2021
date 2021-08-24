using System;
using System.Collections.Generic;
using System.Linq;
using FunctionAppProcessarAcoes.Models;

namespace FunctionAppProcessarAcoes.Data
{
    public class AcoesRepository
    {
        private readonly AcoesContext _context;

        public AcoesRepository(AcoesContext context)
        {
            _context = context;
        }

        public void Save(DadosAcao dadosAcao)
        {
            _context.Acoes.Add(new ()
            {
                Codigo = dadosAcao.Codigo,
                DataReferencia = DateTime.Now,
                Valor = dadosAcao.Valor.Value,
                CodCorretora = dadosAcao.CodCorretora,
                NomeCorretora = dadosAcao.NomeCorretora
            });
            _context.SaveChanges();
        }

        public IEnumerable<Acao> GetAll()
        {
            return _context.Acoes.OrderByDescending(a => a.Id).ToArray();
        }
    }
}