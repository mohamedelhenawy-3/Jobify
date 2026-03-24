using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Events
{
    public class ApplicationStatusNotifier
    {
        public event Func<JobApplication, Task> OnStatusChnaged;
        public async Task Notify(JobApplication app) 
        {
            if(OnStatusChnaged != null)
            {
                await OnStatusChnaged.Invoke(app);
            }

        }
    }
}
