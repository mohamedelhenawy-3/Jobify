using Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class JobApplicationNote
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public JobApplication Application { get; set; }

        public string NoteText { get; set; }
        public DateTime CreatedAt { get; set; }

        public string CreatedByUserId { get; set; }
        public AppUser CreatedByUser { get; set; }
    }
}
