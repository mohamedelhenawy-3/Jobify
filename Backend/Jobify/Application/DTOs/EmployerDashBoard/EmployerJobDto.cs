using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.EmployerDashBoard
{
    public class EmployerJobDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int Applications { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
