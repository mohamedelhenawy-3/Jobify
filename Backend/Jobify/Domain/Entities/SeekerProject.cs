using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class SeekerProject
    {
        public int Id { get; set; }
        public int JobSeekerProfileId { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public string? Url { get; set; }
        public string? Technologies { get; set; }     
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ProfileImage> Images { get; set; } = new List<ProfileImage>();

    }
}
