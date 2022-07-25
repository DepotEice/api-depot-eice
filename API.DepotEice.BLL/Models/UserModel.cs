using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Models
{
    public class UserModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NormalizedEmail { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string ConcurrencyStamp { get; set; } = string.Empty;
        public string SecurityStamp { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public IEnumerable<RoleModel>? Roles { get; set; }
        public IEnumerable<ModuleModel>? Modules { get; set; }
        public IEnumerable<MessageModel>? Messages { get; set; }
        public IEnumerable<AppointmentModel>? Appointments { get; set; }
        public IEnumerable<UserTokenModel>? UserTokens { get; set; }
        public IEnumerable<ArticleModel>? Articles { get; set; }
        public IEnumerable<ArticleCommentModel>? ArticleComments { get; set; }
    }
}
