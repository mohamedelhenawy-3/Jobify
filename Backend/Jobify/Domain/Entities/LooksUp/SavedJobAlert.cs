using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.LooksUp
{
    public class SavedJobAlert
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Keyword { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastChecked { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
