using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comapny.RefactorCompany
{
    public class HiringSnapshotDto

    {
        public int OpenJobs { get; set; }
        public int NewApplicantsThisWeek { get; set; }
        public int InterviewsScheduled { get; set; }
        public int HiresThisMonth { get; set; }
    }
}
