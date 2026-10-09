using Domain.Entities.LooksUp.EmployerPermisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CompanyMembers
{
    public class TeamFilterDto
    {
        public string? Search { get; set; }
        public EmployerCompanyRole? Role { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; }      // name | joined | jobs
        public bool Desc { get; set; }

        private int _pageNumber = 1;
        private int _pageSize = 10;

        public int PageNumber { get => _pageNumber; set => _pageNumber = value < 1 ? 1 : value; }
        public int PageSize { get => _pageSize; set => _pageSize = value < 1 ? 10 : Math.Min(value, 50); }
    }
}
