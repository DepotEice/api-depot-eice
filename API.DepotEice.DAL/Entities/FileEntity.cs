using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    public class FileEntity
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string? Path { get; set; } = string.Empty;
        public int? Size { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
