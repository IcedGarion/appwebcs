using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace appwebProgetto.Models
{
    public class Ordine
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_ORDINE { get; set; }

        public int CD_UTENTE { get; set; }

        public DateTime dt_inserimento { get; set; }

        public float totale { get; set; }

        public string stato { get; set; }


        public Utente Utente { get; set; }

        public ICollection<OrdineProdotto> OrdiniProdotti { get; set; }
    }
}
