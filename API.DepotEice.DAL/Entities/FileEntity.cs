using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Entities
{
    // TODO: Add the creator of the file and the last editor of the file
    // TODO: Add "permission" on the file by referencing a role or something like that
    public class FileEntity
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string? Path { get; set; } = string.Empty;
        public long? Size { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
