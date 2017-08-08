using System.Collections;

namespace School.Data
{
    public class DataSourceResult
    {
        public int Total { get; set; }

        public IEnumerable Data { get; set; }
    }
}
