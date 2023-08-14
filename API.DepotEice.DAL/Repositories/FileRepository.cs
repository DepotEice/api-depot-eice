using API.DepotEice.DAL.Entities;
using API.DepotEice.DAL.IRepositories;
using API.DepotEice.DAL.Mappers;
using DevHopTools.DataAccess;
using DevHopTools.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.Repositories
{
    public class FileRepository : RepositoryBase, IFileRepository
    {
        private readonly ILogger _logger;

        public FileRepository(ILogger<FileRepository> logger, IDevHopConnection connection)
            : base(connection)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        public int Create(FileEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spFiles_Create", true);

            command.AddParameter("key", entity.Key);
            command.AddParameter("path", entity.Path);
            command.AddParameter("size", entity.Size);
            command.AddParameter("type", entity.Type);

            string? scalarResult = _connection.ExecuteScalar(command).ToString();

            if (string.IsNullOrEmpty(scalarResult))
            {
                _logger.LogError("{dt} - The creation of the File entity failed",
                    DateTime.Now);

                return -1;
            }

            return int.Parse(scalarResult);
        }

        public bool Delete(int key)
        {
            if (key <= 0)
            {
                throw new ArgumentException("The key cannot be less than or equal to 0", nameof(key));
            }

            string query = "DELETE FROM Files WHERE Id = @Id";

            Command command = new Command(query, false);

            command.AddParameter("Id", key);

            return _connection.ExecuteNonQuery(command) > 0;
        }

        public IEnumerable<FileEntity> GetAll()
        {
            string query = "SELECT * FROM Files";

            Command command = new Command(query, false);

            return _connection.ExecuteReader(command, dr => dr.DbToFile());
        }

        public FileEntity? GetByKey(int key)
        {
            string query = "SELECT * FROM Files WHERE Id = @Id";

            Command command = new Command(query, false);
            command.AddParameter("Id", key);

            return _connection
                .ExecuteReader(command, dr => dr.DbToFile())
                .SingleOrDefault();
        }

        public FileEntity? GetByKey(string key)
        {
            string query = "SELECT * FROM Files WHERE Key = @Key";

            Command command = new Command(query, false);

            command.AddParameter("Key", key);

            return _connection
                .ExecuteReader(command, dr => dr.DbToFile())
                .SingleOrDefault();
        }

        public bool Update(int key, FileEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
