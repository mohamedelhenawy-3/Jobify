using Application.DTOs.Comapny.RefactorCompany;
using Application.Interfaces.PublicCompany;
using Domain.Entities;
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

        // industryId replaces the old industry string (the dropdown sends the id)
    

        public async Task<PublicCompanyDto> GetByIdAsync(int id)
        {
            return await _context.Companies
                .AsNoTracking()
                .Where(c => c.Id == id && c.IsVerified)
                .Select(c => new PublicCompanyDto
                {
                    Id = c.Id,
                    CompanyName = c.CompanyName,
                    Description = c.Description,
                    CompanySize = c.CompanySize,
                    IndustryId = c.IndustryId,
                    Industry = c.Industry != null ? c.Industry.Name : null,
                    Founded = c.Founded,
                    Location = c.Location,
                    CompanyWebsite = c.CompanyWebsite,
                    Logo = c.Logo != null ? c.Logo.ImageUrl : null,
                    IsVerified = c.IsVerified
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<PublicCompanyDto>> SearchAsync(string? q, int? industryId, string? location)
        {
            var query = _context.Companies
               .AsNoTracking()
               .Where(c => c.IsVerified);

            if (!string.IsNullOrEmpty(q))
                query = query.Where(c => c.CompanyName.Contains(q) ||
                                         (c.Description != null && c.Description.Contains(q)));

            if (industryId.HasValue)
                query = query.Where(c => c.IndustryId == industryId);

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
                    CompanySize = c.CompanySize,
                    IndustryId = c.IndustryId,
                    Industry = c.Industry != null ? c.Industry.Name : null,
                    Founded = c.Founded,
                    Location = c.Location,
                    CompanyWebsite = c.CompanyWebsite,
                    Logo = c.Logo != null ? c.Logo.ImageUrl : null,
                    IsVerified = c.IsVerified
                })
                .ToListAsync();
        }
    }
}