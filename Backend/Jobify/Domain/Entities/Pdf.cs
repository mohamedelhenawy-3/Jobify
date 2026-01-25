using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Pdf
    {
        public int Id { get; set; }

        public string PdfUrl { get; set; }
        public string PublicId { get; set; } // for Cloudinary / Azure Blob (future)

        public DateTime CreatedAt { get; set; }


        public int? JobSeekerId { get; set; }
        public JobSeeker JobSeeker { get; set; }

     

    }
}
