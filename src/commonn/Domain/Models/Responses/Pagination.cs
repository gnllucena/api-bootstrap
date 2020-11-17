using System.Collections.Generic;

namespace Common.Domain.Models.Responses
{
    public class Pagination<T> where T : class
    {
        public Pagination(IEnumerable<T> itens, int offset, int limit, int total)
        {
            Itens = itens;
            Offset = offset;
            Limit = limit;
            Total = total;
        }

        public IEnumerable<T> Itens { get; private set; }
        public int Offset { get; private set; }
        public int Limit { get; private set; }
        public int Total { get; private set; }
    }
}
