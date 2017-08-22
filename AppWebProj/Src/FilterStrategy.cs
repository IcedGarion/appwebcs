using System;
using System.Linq;

namespace UpoECommerce.Src
{
    /*
     * PATTERN STRATEGY usato per il filtraggio: ogni classe diversa che vuole filtrare (Utenti, Ordini e Prodotti)
     * definisce i suoi metodi per filtrare (non tutti i campi si sovrappongono e quindi ci sono alcune property che rimangono vuote)
     * e poi si applica un solo filtro che, a seconda della strategy, chiama i metodi giusti
     */
    public class FilterStrategy<T>
    {
        //Delegates che accettano una Query in T e altri parametri, e restituiscono la query Filtrata (Where)

        //Ordine
        public virtual Func<IQueryable<T>, DateTime, DateTime, IQueryable<T>> FilterDate { get; set; }

        public virtual Func<IQueryable<T>, string, IQueryable<T>> FilterTitoloOrd { get; set; }


        public virtual Func<IQueryable<T>, string, IQueryable<T>> FilterState { get; set; }


        //Ordine + Prodotto
        public virtual Func<IQueryable<T>, double, IQueryable<T>> FilterMinQty { get; set; }


        public virtual Func<IQueryable<T>, double, IQueryable<T>> FilterMinEqQty { get; set; }


        public virtual Func<IQueryable<T>, double, IQueryable<T>> FilterMagQty { get; set; }


        public virtual Func<IQueryable<T>, double, IQueryable<T>> FilterMagEqQty { get; set; }


        public virtual Func<IQueryable<T>, double, IQueryable<T>> FilterEqualQty { get; set; }


        //Prodotto
        public virtual Func<IQueryable<T>, string, IQueryable<T>> FilterTitoloProd { get; set; }


        public virtual Func<IQueryable<T>, IQueryable<T>> FilterScontoPos { get; set; }


        public virtual Func<IQueryable<T>, IQueryable<T>> FilterScontoZero { get; set; }


        public virtual Func<IQueryable<T>, string, IQueryable<T>> FilterDisp { get; set; }


        //Utente
        public virtual Func<IQueryable<T>, string, IQueryable<T>> FilterUsername { get; set; }


        public virtual Func<IQueryable<T>, string, IQueryable<T>> FilterRole{ get; set; }


        
    }
}
