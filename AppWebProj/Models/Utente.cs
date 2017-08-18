using System;
using System.Collections.Generic;

namespace Upo.Model
{
    public partial class Utente
    {
        public Utente()
        {
            Ordine = new HashSet<Ordine>();
        }

        public int CdUtente { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Ruolo { get; set; }

        public virtual ICollection<Ordine> Ordine { get; set; }
    }
}
