using System;
using System.Collections.Generic;

namespace School.Model
{
    public partial class Ordine
    {
        public Ordine()
        {
            OrdineProdotto = new HashSet<OrdineProdotto>();
        }

        public int CdOrdine { get; set; }
        public int CdUtente { get; set; }
        public DateTime DtInserimento { get; set; }
        public double Totale { get; set; }
        public string Stato { get; set; }

        public virtual ICollection<OrdineProdotto> OrdineProdotto { get; set; }
        public virtual Utente CdUtenteNavigation { get; set; }
    }
}
