using Domain.Entities.LooksUp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.DTOs.JobApplication
{
    public class UpdateAppliedJobStatus
    {
        [JsonConverter(typeof(JsonStringEnumConverter))] // <-- allow string enum

        public ApplicationStatus status { get; set; }
        public DateTime? InterviewDate { get; set; }
        public string? InterviewLocation { get; set; }
        public string? Notes { get; set; }
    }
}
