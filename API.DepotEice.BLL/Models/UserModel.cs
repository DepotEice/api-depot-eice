using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.BLL.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicture { get; set; }
        public DateOnly BirthDate { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string SecurityStamp { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public IEnumerable<RoleModel> Roles { get; set; }
        public IEnumerable<ModuleModel>? Modules { get; set; }
        public IEnumerable<MessageModel>? Messages { get; set; }
        public IEnumerable<AppointmentModel>? Appointments { get; set; }
        public IEnumerable<UserTokenModel>? UserTokens { get; set; }
        public IEnumerable<ArticleModel>? Articles { get; set; }
        public IEnumerable<ArticleCommentModel>? ArticleComments { get; set; }
    }
}
