using API.DepotEice.DAL.Entities;
using System.Data;

namespace API.DepotEice.DAL.Mappers
{
    static class Mapper
    {
        public static AppointmentEntity DbToAppointmentEntity(this IDataRecord record)
        {
            return new AppointmentEntity()
            {
                Id = (int)record["Id"],
                StartAt = (DateTime)record["StartAt"],
                EndAt = (DateTime)record["EndAt"],
                Accepted = (bool)record["Accepted"],
                UserId = (string)record["UserId"]
            };
        }

        public static ArticleCommentEntity DbToArticleComment(this IDataRecord record)
        {
            return new ArticleCommentEntity()
            {
                Id = (int)record["Id"],
                Note = (int)record["Note"],
                Review = (string)record["Review"],
                CreatedAt = (DateTime)record["CreatedAt"],
                UpdatedAt = (record["UpdatedAt"] is DBNull) ? null : (DateTime)record["UpdatedAt"],
                ArticleId = (int)record["ArticleId"],
                UserId = (string)record["UserId"]
            };
        }

        public static ArticleEntity DbToArticle(this IDataRecord record)
        {
            return new ArticleEntity()
            {
                Id = (int)record["Id"],
                Title = (string)record["Title"],
                Body = (string)record["Body"],
                CreatedAt = (DateTime)record["CreatedAt"],
                UpdatedAt = (record["UpdatedAt"] is DBNull) ? null : (DateTime)record["UpdatedAt"],
                Pinned = (bool)record["Pinned"],
                UserId = (string)record["UserId"]
            };
        }

        public static MessageEntity DbToMessage(this IDataRecord record)
        {
            return new MessageEntity()
            {
                Id = (int)record["Id"],
                Content = (string)record["Content"],
                SenderId = (string)record["SenderId"],
                ReceiverId = (string)record["ReceiverId"]
            };
        }

        public static ModuleEntity DbToModule(this IDataRecord record)
        {
            return new ModuleEntity()
            {
                Id = (int)record["Id"],
                Name = (string)record["Name"],
                Description = (string)record["Description"]
            };
        }

        public static OpeningHoursEntity DbToOpeningHours(this IDataRecord record)
        {
            return new OpeningHoursEntity()
            {
                Id = (int)record["Id"],
                OpenAt = (DateTime)record["OpenAt"],
                CloseAt = (DateTime)record["CloseAt"]
            };
        }

        public static RoleEntity DbToRole(this IDataRecord record)
        {
            return new RoleEntity()
            {
                Id = record["Id"].ToString() ??
                    throw new NullReferenceException("Record key Id return null!"),

                Name = (string)record["Name"]
            };
        }

        public static ScheduleFileEntity DbToScheduleFile(this IDataRecord record)
        {
            return new ScheduleFileEntity()
            {
                Id = (int)record["Id"],
                FilePath = (string)record["FilePath"],
                ScheduleId = (int)record["ScheduleId"]
            };
        }

        public static ScheduleEntity DbToSchedule(this IDataRecord record)
        {
            return new ScheduleEntity()
            {
                Id = (int)record["Id"],
                Title = (record["Title"] is DBNull) ? null : (string)record["Title"],
                Details = (record["Details"] is DBNull) ? null : (string)record["Details"],
                StartsAt = (DateTime)record["StartsAt"],
                EndsAt = (DateTime)record["EndsAt"],
                ModuleId = (int)record["ModuleId"]
            };
        }

        public static UserEntity DbToUser(this IDataRecord record)
        {
            return new UserEntity()
            {
                Id = record["Id"].ToString() ??
                    throw new NullReferenceException("Record key ID is null!"),

                Email = (string)record["Email"],
                NormalizedEmail = (string)record["NormalizedEmail"],
                FirstName = (string)record["FirstName"],
                LastName = (string)record["LastName"],
                ProfilePicture = (string)record["ProfilePicture"],
                BirthDate = (DateTime)record["BirthDate"],
                ConcurrencyStamp = record["ConcurrencyStamp"].ToString() ??
                    throw new NullReferenceException("Record key ConcurrencyStamp is null!"),

                SecurityStamp = record["SecurityStamp"].ToString() ??
                    throw new NullReferenceException("record key SecurityStamp is null!"),

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
                Id = record["Id"].ToString() ??
                    throw new NullReferenceException("Record key Id return null!"),

                Type = (string)record["Type"],
                Value = record["Value"].ToString() ??
                    throw new NullReferenceException("Record key Value returned null!"),

                DeliveryDateTime = (DateTime)record["DeliveryDate"],
                ExpirationDateTime = (DateTime)record["ExpirationDate"],
                UserId = (string)record["UserId"]
            };

        }
    }
}
