using System;

namespace Upo.Data
{
    public class OrdiniJoinDataSource
    {
        public string Username { get; set; }

        public int CdOrdine { get; set; }

        public string Stato { get; set; }

        public DateTime DtInserimento { get; set; }

        public int Quantita { get; set; }

        public double Totale { get; set; }

        public string Titolo { get; set; }

        public int CdProdotto { get; set; }
    }
}
