using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.LooksUp
{
    public class JobMatchEventArgs
    {
        public string UserEmail { get; set; }
        public string Keyword { get; set; }
        public List<string> Jobs { get; set; }
    }
}
