namespace Upo.Data
{
    /*
     * Oggetto usato per visualizzare i dati del carrello nelle Views
     */
    public class CarrelloDataSource
    {
        public int CdProdotto { get; set; }

        public string Titolo { get; set; }

        public string Descrizione { get; set; }

        public double Prezzo { get; set; }

        public double Sconto { get; set; }

        public byte[] Immagine { get; set; }

        public int Quantita { get; set; }
    }
}
