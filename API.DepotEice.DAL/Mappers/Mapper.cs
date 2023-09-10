using API.DepotEice.DAL.Entities;
using System.Data;
using System.Reflection;

namespace API.DepotEice.DAL.Mappers;

internal static class Mapper
{
    /// <summary>
    /// Map a <see cref="IDataRecord"/> data object to the designated type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of the object to convert to</typeparam>
    /// <param name="record">The IDataRecord coming from the database</param>
    /// <returns>
    /// A new instance of the <typeparamref name="T"/> with all the properties retrieved from the database
    /// </returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NullReferenceException"></exception>
    public static T MapFromDB<T>(this IDataRecord record)
    {
        if (record is null)
        {
            throw new ArgumentNullException(nameof(record));
        }

        Type type = typeof(T);

        object? instance = Activator.CreateInstance(type);

        if (instance is null)
        {
            throw new NullReferenceException($"Activating an instance of ${type} returned a null object");
        }

        var objProps = type.GetProperties();

        foreach (PropertyInfo propInfo in objProps)
        {
            object? recordProperty = record[propInfo.Name];

            if (recordProperty is DBNull)
            {
                propInfo.SetValue(instance, null);
            }
            else if (recordProperty?.GetType() == typeof(Guid))
            {
                propInfo.SetValue(instance, recordProperty.ToString());
            }
            else
            {
                propInfo.SetValue(instance, recordProperty);
            }
        }

        return (T)instance;
    }

    public static UserModuleEntity DBToUserModule(this IDataRecord record)
    {
        return new UserModuleEntity()
        {
            UserId = record["UserId"].ToString()
                ?? throw new NullReferenceException("UserModules column UserId is null"),
            ModuleId = (int)record["ModuleId"],
            IsAccepted = (bool)record["IsAccepted"]
        };
    }

    public static FileEntity DbToFile(this IDataRecord record)
    {
        return new FileEntity()
        {
            Id = (int)record["Id"],
            Key = (string)record["Key"],
            Path = record["Path"] is DBNull ? null : (string)record["Path"],
            Size = record["Size"] is DBNull ? null : (long)record["Size"],
            Type = (string)record["Type"],
            CreatedAt = (DateTime)record["CreatedAt"],
            UpdatedAt = record["UpdatedAt"] is DBNull ? null : (DateTime)record["UpdatedAt"],
            DeletedAt = record["DeletedAt"] is DBNull ? null : (DateTime)record["DeletedAt"]
        };
    }

    public static AddressEntity DbToAddress(this IDataRecord record)
    {
        return new AddressEntity()
        {
            Id = (int)record["Id"],
            Street = (string)record["Street"],
            HouseNumber = (string)record["HouseNumber"],
            Appartment = record["Appartment"] is DBNull or null ? null : (string)record["Appartment"],
            City = (string)record["City"],
            State = record["State"] is DBNull or null ? null : (string)record["State"],
            ZipCode = (string)record["ZipCode"],
            Country = (string)record["Country"],
            IsPrimary = (bool)record["IsPrimary"],
            UserId = record["UserId"].ToString()
        };
    }

    public static AppointmentEntity DbToAppointmentEntity(this IDataRecord record)
    {
        return new AppointmentEntity()
        {
            Id = (int)record["Id"],
            StartAt = (DateTime)record["StartAt"],
            EndAt = (DateTime)record["EndAt"],
            IsAccepted = (bool)record["IsAccepted"],
            UserId = record["UserId"].ToString()
        };
    }

    public static ArticleCommentEntity DbToArticleComment(this IDataRecord record)
    {
        return new ArticleCommentEntity()
        {
            Id = (int)record["Id"],
            Note = (int)record["Note"],
            Review = (string)record["Review"],
            UserId = (string)record["UserId"],
            ArticleId = (int)record["ArticleId"],
            CreatedAt = (DateTime)record["CreatedAt"],
            UpdatedAt = (record["UpdatedAt"] is DBNull) ? null : (DateTime)record["UpdatedAt"],
            DeletedAt = (record["DeletedAt"] is DBNull) ? null : (DateTime)record["DeletedAt"]
        };
    }

    public static ArticleEntity DbToArticle(this IDataRecord record)
    {
        return new ArticleEntity()
        {
            Id = (int)record["Id"],
            MainImageId = (int)record["MainImageId"],
            Title = (string)record["Title"],
            Body = (string)record["Body"],
            CreatedAt = (DateTime)record["CreatedAt"],
            UpdatedAt = (record["UpdatedAt"] is DBNull) ? null : (DateTime)record["UpdatedAt"],
            DeletedAt = (record["DeletedAt"] is DBNull) ? null : (DateTime)record["DeletedAt"],
            IsPinned = (bool)record["IsPinned"],
            UserId = record["UserId"].ToString() ??
                throw new NullReferenceException("Record[\"UserId\"] is null")
        };
    }

    public static MessageEntity DbToMessage(this IDataRecord record)
    {
        return new MessageEntity()
        {
            Id = (int)record["Id"],
            Content = (string)record["Content"],
            SenderId = record["SenderId"].ToString() ??
                throw new NullReferenceException("The property SenderId is null"),
            ReceiverId = record["ReceiverId"].ToString() ??
                throw new NullReferenceException("The property ReceiverId is null"),
            SentAt = (DateTime)record["SentAt"],
            Read = (bool)record["Read"]
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
            FileId = (int)record["FileId"],
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
            StartAt = (DateTime)record["StartAt"],
            EndAt = (DateTime)record["EndAt"],
            ModuleId = (int)record["ModuleId"]
        };
    }

    public static UserEntity DbToUser(this IDataRecord record)
    {
        if (record is null)
        {
            throw new ArgumentNullException(nameof(record));
        }

        return new UserEntity()
        {
            Id = record["Id"].ToString() ??
                throw new NullReferenceException($"The property Id is null"),
            ProfilePictureId = record["ProfilePictureId"] is DBNull
                ? null
                : (int)record["ProfilePictureId"],
            Email = record["Email"] is DBNull ? null : (string)record["Email"],
            NormalizedEmail = (record["NormalizedEmail"] is DBNull or null)
                ? null
                : (string)record["NormalizedEmail"],
            EmailConfirmed = (bool)record["EmailConfirmed"],
            SchoolEmail = (record["SchoolEmail"] is DBNull) ? null : (string)record["SchoolEmail"],
            NormalizedSchoolEmail = record["NormalizedSchoolEmail"]?.ToString() ?? null,
            FirstName = record["FirstName"] is DBNull ? null : (string)record["FirstName"],
            LastName = record["LastName"] is DBNull ? null : (string)record["LastName"],
            Gender = record["Gender"] is DBNull ? null : (string)record["Gender"],
            BirthDate = record["BirthDate"] is DBNull ? null : (DateTime)record["BirthDate"],
            MobileNumber = record["MobileNumber"].ToString() ?? null,
            PhoneNumber = record["PhoneNumber"]?.ToString() ?? null,
            ConcurrencyStamp = record["ConcurrencyStamp"]?.ToString() ??
                throw new NullReferenceException($"The concurrency stamp property is null!"),
            SecurityStamp = record["SecurityStamp"]?.ToString() ??
                throw new NullReferenceException($"The property Id is null"),
            IsActive = (bool)record["IsActive"],
            CreatedAt = (DateTime)record["CreatedAt"],
            UpdatedAt = record["UpdatedAt"] is DBNull ? null : (DateTime)record["UpdatedAt"],
            DeletedAt = record["DeletedAt"] is DBNull ? null : (DateTime)record["DeletedAt"]
        };
    }

    public static UserTokenEntity DbToUserToken(this IDataRecord record)
    {
        return new UserTokenEntity()
        {
            Id = record["Id"].ToString() ??
                throw new NullReferenceException("Record key Id return null!"),

            Type = (string)record["Type"],
            Value = (string)record["Value"],
            DeliveryDate = (DateTime)record["DeliveryDate"],
            ExpirationDate = (DateTime)record["ExpirationDate"],
            UserId = record["UserId"].ToString() ??
                throw new NullReferenceException("Record key UserId returned null!")
        };

    }
}
