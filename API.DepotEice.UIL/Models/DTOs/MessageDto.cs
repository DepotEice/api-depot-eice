namespace API.DepotEice.UIL.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public UserDto Sender { get; set; }
    public UserDto Receiver { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }
}
