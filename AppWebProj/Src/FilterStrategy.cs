using System;
using System.Linq;

namespace UpoECommerce.Src
{
    public class FilterStrategy<T>
    {
        //Delegates che accettano una Query in T e altri parametri, e restituiscono la query Filtrata (Where)

        //Ordine
        public Func<IQueryable<T>, DateTime, DateTime, IQueryable<T>> FilterDate;

        public Func<IQueryable<T>, string, IQueryable<T>> FilterTitoloOrd;

        public Func<IQueryable<T>, string, IQueryable<T>> FilterState;

        //Ordine + Prodotto
        public Func<IQueryable<T>, double, IQueryable<T>> FilterMinQty;

        public Func<IQueryable<T>, double, IQueryable<T>> FilterMinEqQty;

        public Func<IQueryable<T>, double, IQueryable<T>> FilterMagQty;

        public Func<IQueryable<T>, double, IQueryable<T>> FilterMagEqQty;

        public Func<IQueryable<T>, double, IQueryable<T>> FilterEqualQty;

        //Prodotto
        public Func<IQueryable<T>, string, IQueryable<T>> FilterTitoloProd;

        public Func<IQueryable<T>, IQueryable<T>> FilterScontoPos;

        public Func<IQueryable<T>, IQueryable<T>> FilterScontoZero;

        public Func<IQueryable<T>, string, IQueryable<T>> FilterDisp;

        //Utente
        public Func<IQueryable<T>, string, IQueryable<T>> FilterUsername;

        public Func<IQueryable<T>, string, IQueryable<T>> FilterRole;

        
    }
}
