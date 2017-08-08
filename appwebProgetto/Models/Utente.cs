using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace appwebProgetto.Models
{
    public class Utente
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CD_UTENTE { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string stato { get; set; }

        public ICollection<Ordine> Ordini { get; set; }
    }
}
