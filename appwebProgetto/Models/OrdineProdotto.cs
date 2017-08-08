using System.ComponentModel.DataAnnotations.Schema;

namespace appwebProgetto.Models
{
    public class OrdineProdotto
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CD_ORDINE { get; set; }

        public int CD_PRODOTTO { get; set; }

        public int quantita { get; set; }

        public Ordine Ordine { get; set; }

        public Prodotto Prodotto { get; set; }

    }
}
