using Domain.Entities;
using Domain.Entities.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Events
{
    public class ApplicationStatusNotifier
    {
        public event Func<ApplicationStatusEvent, Task> OnStatusChnaged;
        public async Task Notify(ApplicationStatusEvent data) 
        {
            if(OnStatusChnaged != null)
            {
                await OnStatusChnaged.Invoke(data);
            }

        }
    }
}
