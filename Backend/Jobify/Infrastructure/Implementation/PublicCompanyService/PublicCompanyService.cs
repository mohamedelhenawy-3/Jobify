using Application.DTOs.Comapny.RefactorCompany;
using Application.Interfaces.PublicCompany;
using Infrastructure.Implementation.AuthService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.PublicCompanyService
{
    public class PublicCompanyService : IPublicCompanyService
    {
        private readonly AppDBContext _context;

        public PublicCompanyService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<List<PublicCompanyDto>> SearchAsync(string? q, string? industry, string? location)
        {
            var query = _context.Companies.Where(c => c.IsVerified);

            if (!string.IsNullOrEmpty(q))
                query = query.Where(c => c.CompanyName.Contains(q) || c.Description.Contains(q));

            if (!string.IsNullOrEmpty(industry))
                query = query.Where(c => c.Industry == industry);

            if (!string.IsNullOrEmpty(location))
                query = query.Where(c => c.Location == location);

            return await query
                .OrderByDescending(c => c.IsVerified)
                .ThenBy(c => c.CompanyName)
                .Select(c => new PublicCompanyDto
                {
                    Id = c.Id,
                    CompanyName = c.CompanyName,
                    Description = c.Description,
                   CompanySize=c.CompanySize,
                    Industry = c.Industry,
                    Founded = c.Founded,
                    Location = c.Location,
                    CompanyWebsite = c.CompanyWebsite,
                    Logo = c.Logo.ImageUrl,
                    IsVerified = c.IsVerified
                }).ToListAsync
                ();
        }

        public async Task<PublicCompanyDto> GetByIdAsync(int id)
        {
            return await _context.Companies
                .Where(c => c.Id == id && c.IsVerified)
                .Select(c => new PublicCompanyDto
                {
                    Id = c.Id,
                    CompanyName = c.CompanyName,
                    Description = c.Description,

                    Industry = c.Industry,
                    Founded = c.Founded,
                    Location = c.Location,
                    CompanyWebsite = c.CompanyWebsite,
                    Logo = c.Logo.ImageUrl,
                    IsVerified = c.IsVerified
                })
                .FirstOrDefaultAsync();
        }
         
    }
}
