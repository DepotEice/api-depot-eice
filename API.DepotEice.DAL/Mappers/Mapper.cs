using API.DepotEice.DAL.Entities;
using System.Data;

namespace API.DepotEice.DAL.Mappers
{
    static class Mapper
    {
        public static AppointmentEntity DbToAppointmentEntity(this IDataRecord record)
        {
            return new AppointmentEntity(
                (int)record["Id"],
                (DateTime)record["StartAt"],
                (DateTime)record["EndAt"],
                (bool)record["Accepted"],
                (string)record["UserId"]);
        }

        public static ArticleCommentEntity DbToArticleComment(this IDataRecord record)
        {
            return new ArticleCommentEntity(
                (int)record["Id"],
                (int)record["Note"],
                (string)record["Review"],
                (DateTime)record["CreatedAt"],
                (record["UpdatedAt"] is DBNull) ? null : (DateTime)record["UpdatedAt"],
                (int)record["ArticleId"],
                (string)record["UserId"]);
        }

        public static ArticleEntity DbToArticle(this IDataRecord record)
        {
            return new ArticleEntity(
                (int)record["Id"],
                (string)record["Title"],
                (string)record["Body"],
                (DateTime)record["CreatedAt"],
                (record["UpdatedAt"] is DBNull) ? null : (DateTime)record["UpdatedAt"],
                (bool)record["Pinned"],
                (string)record["UserId"]);
        }

        public static MessageEntity DbToMessage(this IDataRecord record)
        {
            return new MessageEntity(
                (int)record["Id"],
                (string)record["Content"],
                (string)record["SenderId"],
                (string)record["ReceiverId"]);
        }

        public static ModuleEntity DbToModule(this IDataRecord record)
        {
            return new ModuleEntity(
                (int)record["Id"],
                (string)record["Name"],
                (string)record["Description"]);
        }

        public static OpeningHoursEntity DbToOpeningHours(this IDataRecord record)
        {
            return new OpeningHoursEntity(
                id: (int)record["Id"],
                openAt: (DateTime)record["OpenAt"],
                closeAt: (DateTime)record["CloseAt"]);
        }

        public static RoleEntity DbToRole(this IDataRecord record)
        {
            return new RoleEntity(
                (string)record["Id"],
                (string)record["Name"]);
        }

        public static ScheduleFileEntity DbToScheduleFile(this IDataRecord record)
        {
            return new ScheduleFileEntity(
                id: (int)record["Id"],
                filePath: (string)record["FilePath"],
                scheduleId: (int)record["ScheduleId"]);
        }

        public static ScheduleEntity DbToSchedule(this IDataRecord record)
        {
            return new ScheduleEntity(
                (int)record["Id"],
                (record["Title"] is DBNull) ? null : (string)record["Title"],
                (record["Details"] is DBNull) ? null : (string)record["Details"],
                (DateTime)record["StartsAt"],
                (DateTime)record["EndsAt"],
                (int)record["ModuleId"]);
        }

        public static UserEntity DbToUser(this IDataRecord record)
        {
            return new UserEntity()
            {
                Id = record["Id"].ToString(),
                Email = (string)record["Email"],
                NormalizedEmail = (string)record["NormalizedEmail"],
                FirstName = (string)record["FirstName"],
                LastName = (string)record["LastName"],
                ProfilePicture = (string)record["ProfilePicture"],
                BirthDate = (DateTime)record["BirthDate"],
                ConcurrencyStamp = record["ConcurrencyStamp"].ToString(),
                SecurityStamp = record["SecurityStamp"].ToString(),
                IsActive = (bool)record["IsActive"],
                CreatedAt = (DateTime)record["CreatedAt"],
                UpdatedAt = (record["UpdatedAt"] is DBNull) ? null : (DateTime)record["UpdatedAt"],
                DeletedAt = (record["DeletedAt"] is DBNull) ? null : (DateTime)record["DeletedAt"]
            };
        }

        public static UserTokenEntity DbToUserToken(this IDataRecord record)
        {
            return new UserTokenEntity()
            {
                Id = (string)record["Id"],
                Type = (string)record["Type"],
                Value = (string)record["Value"],
                DeliveryDateTime = (DateTime)record["DeliveryDate"],
                ExpirationDateTime = (DateTime)record["ExpirationDate"],
                UserId = (string)record["UserId"]
            };

        }
    }
}
