using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.LooksUp
{
    public  enum ApplicationStatus
    {
    
        Applied = 1,        // User just applied
        UnderReview = 2,    // Employer is reviewing
        Shortlisted = 3,    // Passed first screening
        InterviewScheduled = 4,
        Interviewed = 5,
        Offered = 6,        // Offer sent
        Hired = 7,          // Accepted offer
        Rejected = 8,       // Employer rejected
        Withdrawn = 9       // Applicant withdrew
    
    
}
}
