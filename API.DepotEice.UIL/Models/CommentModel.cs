using API.DepotEice.BLL.Dtos;

namespace API.DepotEice.UIL.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public int Note { get; set; }
        public string Review { get; set; }
        public UserDto User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
