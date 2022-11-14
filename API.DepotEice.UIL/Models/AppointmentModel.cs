namespace API.DepotEice.UIL.Models
{
    public class AppointmentModel
    {
        public int Id { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public bool IsAccepted { get; set; }

        public string UserId { get; set; } = string.Empty;
    }
}
