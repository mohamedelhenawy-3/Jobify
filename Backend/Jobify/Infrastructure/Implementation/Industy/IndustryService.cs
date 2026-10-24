using Application.DTOs.Industries;
using Application.Interfaces.Industry;
using Domain.Entities;
using Infrastructure.Implementation.AuthService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.Industy
{
    public class IndustryService : IIndustryService
    {
        private readonly AppDBContext _db;

        public IndustryService(AppDBContext db) => _db = db;

        public async Task<List<IndustryDto>> GetAllAsync(bool includeInactive = false, CancellationToken ct = default)
        {
            var query = _db.Industries.AsNoTracking();

            if (!includeInactive)
                query = query.Where(i => i.IsActive);

            return await query
                .OrderBy(i => i.Name)
                .Select(i => new IndustryDto(i.Id, i.Name, i.Slug, i.IsActive))
                .ToListAsync(ct);
        }

        public async Task<IndustryDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Industries
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new IndustryDto(i.Id, i.Name, i.Slug, i.IsActive))
                .FirstOrDefaultAsync(ct)
                ?? throw new KeyNotFoundException("Industry not found.");
        }

        public async Task<IndustryDto> CreateAsync(CreateIndustryDto dto, CancellationToken ct = default)
        {
            var name = NormalizeName(dto.Name);
            if (name.Length == 0)
                throw new ArgumentException("Name is required.");

            var lower = name.ToLower();
            if (await _db.Industries.AnyAsync(i => i.Name.ToLower() == lower, ct))
                throw new InvalidOperationException("This industry already exists.");

            var slug = await UniqueSlugAsync(name, null, ct);

            var industry = new Industry
            {
                Name = name,
                Slug = slug,
                IsActive = true
            };

            _db.Industries.Add(industry);
            await _db.SaveChangesAsync(ct);

            return new IndustryDto(industry.Id, industry.Name, industry.Slug, industry.IsActive);
        }

        public async Task UpdateAsync(int id, UpdateIndustryDto dto, CancellationToken ct = default)
        {
            var industry = await _db.Industries.FirstOrDefaultAsync(i => i.Id == id, ct)
                ?? throw new KeyNotFoundException("Industry not found.");

            var name = NormalizeName(dto.Name);
            if (name.Length == 0)
                throw new ArgumentException("Name is required.");

            var lower = name.ToLower();
            if (await _db.Industries.AnyAsync(i => i.Id != id && i.Name.ToLower() == lower, ct))
                throw new InvalidOperationException("Another industry with this name already exists.");

            if (!string.Equals(industry.Name, name, StringComparison.Ordinal))
            {
                industry.Name = name;
                industry.Slug = await UniqueSlugAsync(name, id, ct);
            }

            industry.IsActive = dto.IsActive;
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var industry = await _db.Industries.FirstOrDefaultAsync(i => i.Id == id, ct)
                ?? throw new KeyNotFoundException("Industry not found.");

            if (await _db.Companies.AnyAsync(c => c.IndustryId == id, ct))
                throw new InvalidOperationException(
                    "This industry is used by companies. Deactivate it instead of deleting.");

            _db.Industries.Remove(industry);
            await _db.SaveChangesAsync(ct);
        }
        public async Task<List<PopularIndustryDto>> GetPopularAsync(int take = 6)
        {
            var rows = await _db.Industries
                .Where(i => i.IsActive)
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    i.Slug,
                    Count = _db.JobPosts.Count(j => j.IsActive && j.Company.IndustryId == i.Id)
                })
                .Where(x => x.Count > 0)
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Name)
                .Take(take)
                .ToListAsync();

            return rows.Select(x => new PopularIndustryDto(x.Id, x.Name, x.Slug, x.Count)).ToList();
        }
        // ---------- helpers ----------

        // trims and collapses repeated spaces: "  Information   Technology " -> "Information Technology"
        private static string NormalizeName(string? name) =>
            Regex.Replace((name ?? string.Empty).Trim(), @"\s+", " ");

        private static string Slugify(string name)
        {
            var slug = Regex.Replace(name.ToLowerInvariant(), @"[^\p{L}\p{N}]+", "-").Trim('-');
            return slug.Length == 0 ? "industry" : slug;
        }

        private async Task<string> UniqueSlugAsync(string name, int? ignoreId, CancellationToken ct)
        {
            var baseSlug = Slugify(name);
            var slug = baseSlug;
            var n = 2;

            while (await _db.Industries.AnyAsync(i => i.Slug == slug && i.Id != ignoreId, ct))
                slug = $"{baseSlug}-{n++}";

            return slug;
        }
    }
}
