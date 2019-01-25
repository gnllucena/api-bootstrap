using System;
using System.Collections.Generic;

namespace API.Domains.Models
{
    public class Pagination<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int Offset { get; set; }
        public int Limit { get; set; }
        public int Total { get; set; }
    }
}
