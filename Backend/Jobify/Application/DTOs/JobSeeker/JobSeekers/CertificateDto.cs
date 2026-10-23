using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.JobSeeker.JobSeekers
{
    public class CertificateDto
    {
        public int? Id { get; set; }
        public string Name { get; set; } = "";
        public string Issuer { get; set; } = "";
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CredentialUrl { get; set; }
    }
}
