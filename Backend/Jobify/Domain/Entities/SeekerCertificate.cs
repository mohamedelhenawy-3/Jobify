using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SeekerCertificate
    {
        public int Id { get; set; }
        public int JobSeekerProfileId { get; set; }
        public string Name { get; set; } = "";
        public string Issuer { get; set; } = "";
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? CredentialUrl { get; set; }
    }
}
