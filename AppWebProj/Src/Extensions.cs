using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using Upo.Data;
using Upo.Model;

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


        //FILTRI TABELLE
        public static bool FilterOrder(this IQueryable<OrdiniJoinDataSource> Query, string clear, string start, string end,
            string titolo, string qtaoperator, string qta, string totoperator, string tot, string stato)
        {
            DateTime.TryParse(MIN_DATE, out DateTime MIN);
            DateTime.TryParse(MAX_DATE, out DateTime MAX);
            bool filtered = false;

            //se c'e' clear, non fa niente
            if (clear == null)
            {
                if (start != null && end != null)
                {
                    try
                    {
                        DateTime Start = DateTime.Parse(start);
                        DateTime End = DateTime.Parse(end);

                        if ((Start.CompareTo(MIN) > 0 && Start.CompareTo(MAX) < 0)
                            && (End.CompareTo(MIN) > 0 && End.CompareTo(MAX) < 0))
                        {
                            Query = Query.Where(ordine => ordine.DtInserimento >= Start && ordine.DtInserimento <= End);
                        }
                    }
                    catch (Exception)
                    { }

                    filtered = true;
                }

                if (titolo != null && !titolo.Equals(""))
                {
                    Query = Query.Where(ordine => ordine.Titolo.Contains(titolo));
                    filtered = true;
                }

                if (qtaoperator != null && qta != null)
                {
                    double.TryParse(qta, out double Qta);

                    switch (qtaoperator)
                    {
                        case "<":
                            Query = Query.Where(ordine => ordine.Quantita < Qta);
                            break;
                        case "<=":
                            Query = Query.Where(ordine => ordine.Quantita <= Qta);
                            break;
                        case ">":
                            Query = Query.Where(ordine => ordine.Quantita > Qta);
                            break;
                        case ">=":
                            Query = Query.Where(ordine => ordine.Quantita >= Qta);
                            break;
                        case "=":
                            Query = Query.Where(ordine => ordine.Quantita == Qta);
                            break;
                        default:
                            break;
                    }
                    filtered = true;
                }

                if (totoperator != null && tot != null)
                {
                    double.TryParse(tot, out double Tot);

                    switch (totoperator)
                    {
                        case "<":
                            Query = Query.Where(ordine => ordine.Totale < Tot);
                            break;
                        case "<=":
                            Query = Query.Where(ordine => ordine.Totale <= Tot);
                            break;
                        case ">":
                            Query = Query.Where(ordine => ordine.Totale > Tot);
                            break;
                        case ">=":
                            Query = Query.Where(ordine => ordine.Totale >= Tot);
                            break;
                        case "=":
                            Query = Query.Where(ordine => ordine.Totale == Tot);
                            break;
                        default:
                            break;
                    }
                    filtered = true;
                }

                if (stato != null && !stato.Equals(""))
                {
                    Query = Query.Where(ordine => ordine.Stato.Equals(stato));
                    filtered = true;
                }
            }

            return filtered;
        }

        public static bool FilterUser(this IQueryable<Utente> Query, string clear, string username, string ruolo)
        {
            bool filtered = false;

            if (clear == null)
            {
                if (username != null && !username.Equals(""))
                {
                    Query = Query.Where(u => u.Username.Contains(username));
                    filtered = true;
                }

                if (ruolo != null && !ruolo.Equals(""))
                {
                    Query = Query.Where(u => u.Ruolo.Equals(ruolo));
                    filtered = true;
                }
            }

            return filtered;
        }

        public static bool FilterProd(this IQueryable<Prodotto> Query, string clear, string titolo, string disp,
            string prezzooperator, string prezzo, string sconto)
        {
            bool filtered = false;

            if (clear == null)
            {
                if (titolo != null && !titolo.Equals(""))
                {
                    Query = Query.Where(prod => prod.Titolo.Contains(titolo));
                    filtered = true;
                }

                if (prezzooperator != null && prezzo != null)
                {
                    double.TryParse(prezzo, out double Prezzo);

                    switch (prezzooperator)
                    {
                        case "<":
                            Query = Query.Where(prod => prod.Prezzo < Prezzo);
                            break;
                        case "<=":
                            Query = Query.Where(prod => prod.Prezzo <= Prezzo);
                            break;
                        case ">":
                            Query = Query.Where(prod => prod.Prezzo > Prezzo);
                            break;
                        case ">=":
                            Query = Query.Where(prod => prod.Prezzo >= Prezzo);
                            break;
                        case "=":
                            Query = Query.Where(prod => prod.Prezzo == Prezzo);
                            break;
                        default:
                            break;
                    }

                    filtered = true;
                }

                if (sconto != null && !sconto.Equals(""))
                {

                    if (sconto.Equals("si"))
                    {
                        Query = Query.Where(prod => prod.Sconto > 0);
                    }
                    else
                    {
                        Query = Query.Where(prod => prod.Sconto == 0);
                    }

                    filtered = true;
                }

                if (disp != null && !disp.Equals(""))
                {
                    Query = Query.Where(ordine => ordine.Disponibile.Equals(disp));
                    filtered = true;
                }
            }

            return filtered;
        }
    }
}
