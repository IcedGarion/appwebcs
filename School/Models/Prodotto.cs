using System;
using System.Collections.Generic;

namespace Upo.Model
{
    public partial class Prodotto
    {
        public Prodotto()
        {
            OrdineProdotto = new HashSet<OrdineProdotto>();
        }

        public int CdProdotto { get; set; }
        public string Titolo { get; set; }
        public string Descrizione { get; set; }
        public double Prezzo { get; set; }
        public double Sconto { get; set; }
        public byte[] Immagine { get; set; }
        public string Disponibile { get; set; }

        public virtual ICollection<OrdineProdotto> OrdineProdotto { get; set; }
    }
}
