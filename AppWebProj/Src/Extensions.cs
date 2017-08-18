using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using Upo.Data;
using Upo.Model;
using UpoECommerce.Src;

namespace Upo
{
    public static class Extensions
    {
        private static readonly string MIN_DATE = "1/1/1754";
        private static readonly string MAX_DATE = "12/31/9998";


        //serializza e deserializza oggetti in array di byte per salvarli in session
        public static void SetObjectAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value);
        }

        /*
         * FITRI: 
         * Ordine, Utente e Prodotti devono essere filtrati. Filtrano per nome di un parametro (se != null, filtra le righe che contengono il parametro)
         *     e filtrano per operatore: se != null, applica operatore su un certo valore (es.: qtaOp = '<', val = 10 => query.Where(x => x.proprieta' < 10)
         *     (2 metodi diversi FILTRO generici per ottenere il risultato).
         * Ognuno dei 3 diversi Oggetti che vogliono filtrare istanziano una FilterStrategy che contiene i delegates che dicono
         *     come filtrare (delle Where), cosi' lo stesso metodo Filter analizza i parametri che si vogliono filtrare
         *     e chiama le funzioni giuste.
         */

        //accetta operatore e valore: a seconda dell'operatore, chiama la funzione giusta nella strategy passandogli il valore
        //la Func<> della strategy e' un filtro per la query (Where)
        //riusato 3 volte
        private static IQueryable<T> FilterOperator<T>(ref IQueryable<T> Query, ref bool filtered, string clear, string Operator, string Value,
            FilterStrategy<T> Strategy)
        {
            if(clear == null)
            {
                if (Operator != null && Value != null)
                {
                    double.TryParse(Value, out double Val);

                    switch (Operator)
                    {
                        case "<":
                            Query = Strategy.FilterMinQty(Query, Val);
                            break;
                        case "<=":
                            Query = Strategy.FilterMinEqQty(Query, Val);
                            break;
                        case ">":
                            Query = Strategy.FilterMagQty(Query, Val);
                            break;
                        case ">=":
                            Query = Strategy.FilterMagEqQty(Query, Val);
                            break;
                        case "=":
                            Query = Strategy.FilterEqualQty(Query, Val);
                            break;
                        default:
                            break;
                    }

                    filtered = true;
                }
            }

            return Query;
        }

        //Metodo Generico che accetta delegates
        //filtra tutti i nomi: se trova un argomento != null, chiama il corrispondente Func<> definito nella strategy, il quale filtra.
        //Ordini, Utenti e Prodotti chiamano con parametri diversi e con Strategy diverse
        private static IQueryable<T> GeneralFilter<T>(ref IQueryable<T> Query, ref bool filtered, string clear, 
            string start, string end, string titolo, string stato,
            string username, string ruolo,
            string titoloProd, string disp, string sconto,
            FilterStrategy<T> Strategy)
        {
            //limiti DateTime
            DateTime.TryParse(MIN_DATE, out DateTime MIN);
            DateTime.TryParse(MAX_DATE, out DateTime MAX);

            filtered = false;

            //se c'e' clear, non fa niente
            if (clear == null)
            {
                //ORDINE 
                if (start != null && end != null)
                {
                    try
                    {
                        DateTime Start = DateTime.Parse(start);
                        DateTime End = DateTime.Parse(end);

                        if ((Start.CompareTo(MIN) > 0 && Start.CompareTo(MAX) < 0)
                            && (End.CompareTo(MIN) > 0 && End.CompareTo(MAX) < 0))
                        {
                            //esempio: Query = Query.Where(ordine => ordine.DtInserimento >= Start && ordine.DtInserimento <= End);
                            Query = Strategy.FilterDate(Query, Start, End);
                        }
                    }
                    catch (Exception)
                    { }

                    filtered = true;
                }

                if (titolo != null && !titolo.Equals(""))
                {
                    //esempio: Query = Query.Where(ordine => ordine.Titolo.Contains(titolo));
                    Query = Strategy.FilterTitoloOrd(Query, titolo);
                    filtered = true;
                }

                if (stato != null && !stato.Equals(""))
                {
                    //Query = Query.Where(ordine => ordine.Stato.Equals(stato));
                    Query = Strategy.FilterState(Query, stato);
                    filtered = true;
                }

                //UTENTE
                if (username != null && !username.Equals(""))
                {
                    //Query = Query.Where(u => u.Username.Contains(username));
                    Query = Strategy.FilterUsername(Query, username);
                    filtered = true;
                }

                if (ruolo != null && !ruolo.Equals(""))
                {
                    //Query = Query.Where(u => u.Ruolo.Equals(ruolo));
                    Query = Strategy.FilterRole(Query, ruolo);
                    filtered = true;
                }

                //PRODOTTO
                if (titoloProd != null && !titoloProd.Equals(""))
                {
                    //Query = Query.Where(ordine => ordine.Titolo.Contains(titolo));
                    Query = Strategy.FilterTitoloProd(Query, titoloProd);
                    filtered = true;
                }

                if (sconto != null && !sconto.Equals(""))
                {

                    if (sconto.Equals("si"))
                    {
                        //Query = Query.Where(prod => prod.Sconto > 0);
                        Query = Strategy.FilterScontoPos(Query);
                    }
                    else
                    {
                        //Query = Query.Where(prod => prod.Sconto == 0);
                        Query = Strategy.FilterScontoZero(Query);
                    }

                    filtered = true;
                }

                if (disp != null && !disp.Equals(""))
                {
                    //Query = Query.Where(ordine => ordine.Disponibile.Equals(disp));
                    Query = Strategy.FilterDisp(Query, disp);
                    filtered = true;
                }
            }

            return Query;
        }

        //FILTRI Specifici
        public static IQueryable<OrdiniJoinDataSource> FilterOrder(this IQueryable<OrdiniJoinDataSource> Query, ref bool filtered, string clear, string start, string end,
            string titolo, string qtaoperator, string qta, string totoperator, string tot, string stato)
        {
            //filtra i nomi (contains)
            Query = GeneralFilter(ref Query, ref filtered, clear, start, end, titolo, stato, null, null, null, null, null,
                new FilterStrategy<OrdiniJoinDataSource>()
                {
                    FilterDate = (query, Start, End) => query.Where(ordine => ordine.DtInserimento >= Start && ordine.DtInserimento <= End),
                    FilterTitoloOrd = (query, tit) => query.Where(ordine => ordine.Titolo.Contains(tit)),
                    FilterState = (query, state) => query.Where(ordine => ordine.Stato.Equals(state))
                });

            //filtra operatore su quantita' prodotti nell'ordine
            Query = FilterOperator(ref Query, ref filtered, clear, qtaoperator, qta,
                new FilterStrategy<OrdiniJoinDataSource>()
                {
                    FilterMinQty = (query, qty) => query.Where(ordine => ordine.Quantita < qty),
                    FilterMinEqQty = (query, qty) => query.Where(ordine => ordine.Quantita <= qty),
                    FilterMagQty = (query, qty) => query.Where(ordine => ordine.Quantita > qty),
                    FilterMagEqQty = (query, qty) => query.Where(ordine => ordine.Quantita >= qty),
                    FilterEqualQty = (query, qty) => query.Where(ordine => ordine.Quantita == qty)
                });

            //filtra operatore su totale costo ordine
            Query = FilterOperator(ref Query, ref filtered, clear, totoperator, tot,
                new FilterStrategy<OrdiniJoinDataSource>()
                {
                    FilterMinQty = (query, total) => query.Where(ordine => ordine.Totale < total),
                    FilterMinEqQty = (query, total) => query.Where(ordine => ordine.Totale <= total),
                    FilterMagQty = (query, total) => query.Where(ordine => ordine.Totale > total),
                    FilterMagEqQty = (query, total) => query.Where(ordine => ordine.Totale >= total),
                    FilterEqualQty = (query, total) => query.Where(ordine => ordine.Totale == total)
                });

            return Query;
        }

        public static IQueryable<Utente> FilterUser(this IQueryable<Utente> Query, ref bool filtered, string clear, string username, string ruolo)
        {
            filtered = false;

            Query = GeneralFilter(ref Query, ref filtered, clear, null, null, null, null, username, ruolo, null, null, null,
                new FilterStrategy<Utente>()
                {
                    FilterUsername = (query, usrname) => query.Where(utente => utente.Username.Contains(usrname)),
                    FilterRole = (query, role) => query.Where(utente => utente.Ruolo.Equals(role))
                });

            return Query;
        }

        public static IQueryable<Prodotto> FilterProd(this IQueryable<Prodotto> Query, ref bool filtered, string clear, string titolo, string disp,
            string prezzooperator, string prezzo, string sconto)
        {
            filtered = false;

            Query = GeneralFilter(ref Query, ref filtered, clear, null, null, null, null, null, null, titolo, disp, sconto,
                new FilterStrategy<Prodotto>()
                {
                    FilterTitoloProd = (query, title) => query.Where(prodotto => prodotto.Titolo.Contains(title)),
                    FilterDisp = (query, Disp) => query.Where(prodotto => prodotto.Disponibile.Equals(Disp)),
                    FilterScontoPos = (query) => query.Where(prodotto => prodotto.Sconto > 0),
                    FilterScontoZero = (query) => query.Where(prodotto => prodotto.Sconto == 0)
                });

            Query = FilterOperator(ref Query, ref filtered, clear, prezzooperator, prezzo,
                new FilterStrategy<Prodotto>()
                {
                    FilterMinQty = (query, price) => query.Where(prodotto => prodotto.Prezzo < price),
                    FilterMinEqQty = (query, price) => query.Where(prodotto => prodotto.Prezzo <= price),
                    FilterMagQty = (query, price) => query.Where(prodotto => prodotto.Prezzo > price),
                    FilterMagEqQty = (query, price) => query.Where(prodotto => prodotto.Prezzo >= price),
                    FilterEqualQty = (query, price) => query.Where(prodotto => prodotto.Prezzo == price),
                });

            return Query;
        }
    }
}
