using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Models
{
    public class ArticleCommentModel
    {
        public int Id { get; set; }
        public int Note { get; set; }
        public string Review { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ArticleModel Article { get; set; }
        public UserDto User { get; set; }
    }
}
