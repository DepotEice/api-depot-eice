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
    public class AddressRepository : RepositoryBase, IAddressRepository
    {
        private readonly ILogger _logger;

        public AddressRepository(ILogger<AddressRepository> logger, IDevHopConnection connection) : base(connection)
        {
            if (logger is null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        /// <summary>
        /// Create a new Address data in the Addresses table in the database
        /// </summary>
        /// <param name="entity">The entity to insert. Cannot be null</param>
        /// <returns>-1 If the operation failed at some point. The newly created ID if created successfully</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public int Create(AddressEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spAddresses_Create", true);

            command.AddParameter("Street", entity.Street);
            command.AddParameter("HouseNumber", entity.HouseNumber);
            command.AddParameter("Appartment", entity.Appartment);
            command.AddParameter("City", entity.City);
            command.AddParameter("State", entity.State);
            command.AddParameter("ZipCode", entity.ZipCode);
            command.AddParameter("Country", entity.Country);
            command.AddParameter("IsPrimary", entity.IsPrimary);
            command.AddParameter("UserId", entity.UserId);

            string? scalarResult = _connection.ExecuteScalar(command).ToString();

            if (string.IsNullOrEmpty(scalarResult))
            {
                _logger.LogError($"{DateTime.Now} - The creation of the Address entity failed");
                return -1;
            }

            return int.Parse(scalarResult);
        }

        /// <summary>
        /// Delete an Address data from the Addresses table of the database based on the given ID
        /// </summary>
        /// <param name="key">The ID of the address to delete</param>
        /// <returns>true If the delete operation went successfully. false Otherwise</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool Delete(int key)
        {
            if (key < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            string query = "DELETE FROM [dbo].[Addresses] WHERE [Addresses].[Id] = @Id";

            Command command = new Command(query);

            command.AddParameter("Id", key);

            return _connection.ExecuteNonQuery(command) > 0;
        }

        /// <summary>
        /// Get all the addresses from the table Addresses from the database
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="AddressEntity"/></returns>
        public IEnumerable<AddressEntity> GetAll()
        {
            string query = "SELECT * FROM [dbo].[Addresses]";

            Command command = new Command(query);

            return _connection.ExecuteReader(command, address => Mapper.DbToAddress(address));
        }

        /// <summary>
        /// Get an address from the table Addresses from the database based on its ID
        /// </summary>
        /// <param name="key">The ID of the required address</param>
        /// <returns>
        /// <c>null</c> If nothing is found or if more than 1 row is returned. <see cref="AddressEntity"/> Otherwise
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public AddressEntity? GetByKey(int key)
        {
            if (key < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            string query = "SELECT * FROM [dbo].[Addresses] WHERE [Addresses].[Id] = @Id";

            Command command = new Command(query);

            command.AddParameter("Id", key);

            return _connection
                .ExecuteReader(command, address => Mapper.DbToAddress(address))
                .SingleOrDefault();
        }

        /// <summary>
        /// Update a Address record of the Addresses table in the database for the given ID
        /// </summary>
        /// <param name="key">The ID of the address record</param>
        /// <param name="entity">The Address information to update</param>
        /// <returns>
        /// true. If the one or more record has been updated. false Otherwise
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public bool Update(int key, AddressEntity entity)
        {
            if (key < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(key));
            }

            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            Command command = new Command("spAddresses_Update", true);

            command.AddParameter("Street", entity.Street);
            command.AddParameter("HouseNumber", entity.HouseNumber);
            command.AddParameter("Appartment", entity.Appartment);
            command.AddParameter("City", entity.City);
            command.AddParameter("State", entity.State);
            command.AddParameter("ZipCode", entity.ZipCode);
            command.AddParameter("Country", entity.Country);
            command.AddParameter("IsPrimary", entity.IsPrimary);
            command.AddParameter("AddressId", key);

            return _connection.ExecuteNonQuery(command) > 0;
        }
    }
}
