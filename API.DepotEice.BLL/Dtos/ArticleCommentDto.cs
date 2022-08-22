using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Dtos
{
    public class ArticleCommentDto
    {
        public int Id { get; set; }
        public int Note { get; set; }
        public string Review { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ArticleDto Article { get; set; }
        public UserDto User { get; set; }
    }
}
