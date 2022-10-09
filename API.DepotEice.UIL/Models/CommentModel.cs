namespace API.DepotEice.UIL.Models;

public class CommentModel
{
    public int Id { get; set; }
    public int Note { get; set; }
    public string Review { get; set; }
    public UserModel User { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
