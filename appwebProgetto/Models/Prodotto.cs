using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace appwebProgetto.Models
{
    public class Prodotto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CD_PRODOTTO { get; set; }

        public string titolo { get; set; }

        public string descrizione { get; set; }

        public float prezzo { get; set; }

        public float sconto { get; set; }

        public byte[] immagine { get; set; }

        public string disponibile { get; set; }

        public ICollection<OrdineProdotto> OrdiniProdotti { get; set; }
    }
}
