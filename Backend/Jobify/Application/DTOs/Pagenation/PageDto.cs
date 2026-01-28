using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Pagenation
{
    public class PageDto <T>
    {
        public int pagesize { get; set; }
        public int currentpage { get; set; }
        public int totaljobs { get; set; }
        public int totalPages => (int)Math.Ceiling((double)totaljobs / pagesize);
        public IEnumerable<T> items { get; set; } = new List<T>();

    }
}
