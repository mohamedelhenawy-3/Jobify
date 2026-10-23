using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Comapny.RefactorCompany
{
    public class PublicCompanyJobDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Location { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
