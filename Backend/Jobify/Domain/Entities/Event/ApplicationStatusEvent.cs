using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Event
{
    public class ApplicationStatusEvent
    {
        public JobApplication Application { get; set; }
        public Interview? Interview { get; set; }
    }
}
