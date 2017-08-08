using System;
using System.Collections.Generic;

namespace School.Model
{
    public partial class OrdineProdotto
    {
        public int CdOrdine { get; set; }
        public int CdProdotto { get; set; }
        public int Quantita { get; set; }

        public virtual Ordine CdOrdineNavigation { get; set; }
        public virtual Prodotto CdProdottoNavigation { get; set; }
    }
}
