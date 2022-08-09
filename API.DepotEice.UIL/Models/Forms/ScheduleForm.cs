using System.ComponentModel.DataAnnotations;

namespace API.DepotEice.UIL.Models.Forms
{
    public class ScheduleForm
    {
        [MaxLength(100)]
        public string Title { get; set; }

        public string Details { get; set; }

        [Required]
        public DateTime StartsAt { get; set; }

        [Required]
        public DateTime EndsAt { get; set; }
    }
}
