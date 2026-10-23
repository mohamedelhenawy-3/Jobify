using Domain.Entities.LooksUp.JobEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.JobService
{
    public class SimilarJobScorer
    {
        private static readonly HashSet<string> Stop = new()
    {
        "the","and","for","with","you","your","our","are","will","have","has","this","that",
        "from","not","but","all","can","able","work","working","job","role","team","years",
        "year","experience","strong","good","knowledge","skills","skill","ability","we","to",
        "of","in","at","an","a","or","as","be","is","on","by","senior","junior","lead","jr","sr"
    };

        public static HashSet<string> Keywords(string? text) =>
            Regex.Matches((text ?? "").ToLowerInvariant(), @"[a-z0-9#+.]+")
                 .Select(m => m.Value.Trim('.'))
                 .Where(w => w.Length > 1 && !Stop.Contains(w))
                 .ToHashSet();

        private static double OverlapMin(HashSet<string> a, HashSet<string> b) =>
            a.Count == 0 || b.Count == 0 ? 0 :
            (double)a.Intersect(b).Count() / Math.Min(a.Count, b.Count);

        private static double Jaccard(HashSet<string> a, HashSet<string> b) =>
            a.Count == 0 || b.Count == 0 ? 0 :
            (double)a.Intersect(b).Count() / a.Union(b).Count();

        public static int Score(JobCandidate src, JobCandidate other)
        {
            double score = 0;

            // 1) Title (40)
            score += 40 * OverlapMin(src.TitleWords, other.TitleWords);

            // 2) Requirements + description keywords (25)
            //    Jaccard on long text is naturally low, so scale it up and cap at 1
            score += 25 * Math.Min(1.0, Jaccard(src.BodyWords, other.BodyWords) * 3);

            // 3) Job type (10)
            if (src.JobType == other.JobType) score += 10;

            // 4) Location (10)
            if (!string.IsNullOrWhiteSpace(src.Location) &&
                src.Location.Trim().Equals(other.Location?.Trim(), StringComparison.OrdinalIgnoreCase))
                score += 10;

            // 5) Experience (10, adjacent level = 5)
            var diff = Math.Abs((int)src.Experience - (int)other.Experience);
            score += diff == 0 ? 10 : diff == 1 ? 5 : 0;

            // 6) Same company (5)
            if (src.CompanyId == other.CompanyId) score += 5;

            return (int)Math.Round(Math.Min(score, 100));
        }
    }
}
