using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DepotEice.DAL.IRepositories
{
    public interface IRepositoryBase<TEntity, TKey>
        where TEntity : class
    {
        /// <summary>
        /// Retrieve all records of type <typeparamref name="TEntity"/> from the database
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> of type <typeparamref name="TEntity"/>. If no data is
        /// present in the database, returns an empty <see cref="IEnumerable{T}"/>
        /// </returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Retrieve a record of type <typeparamref name="TEntity"/> with the given 
        /// <paramref name="key"/> from the database
        /// </summary>
        /// <param name="key">
        /// The record's ID of type <typeparamref name="TKey"/>
        /// </param>
        /// <returns>
        /// An entity of type <typeparamref name="TEntity"/> if there is effectively a record with
        /// this ID. <c>null</c> Otherwise
        /// </returns>
        TEntity? GetByKey(TKey key);

        /// <summary>
        /// Create a new record of type <typeparamref name="TEntity"/> in the database and return
        /// its newly created primary key of type <typeparamref name="TKey"/>
        /// </summary>
        /// <param name="entity">
        /// The record to create. Throws a <see cref="ArgumentNullException"/> if it is <c>null</c>
        /// </param>
        /// <returns>
        /// The newly created record's ID. Depending on <paramref name="entity"/>'s type, it can 
        /// return <c>null</c> or a default value
        /// </returns>
        TKey Create(TEntity entity);

        /// <summary>
        /// Update the record of type <typeparamref name="TEntity"/> in the database
        /// </summary>
        /// <param name="entity">
        /// The entity to update
        /// </param>
        /// <returns>
        /// <c>true</c> If one or more rows were affected, meaning the update succeded. <c>false</c>
        /// Otherwise
        /// </returns>
        bool Update(TEntity entity);

        /// <summary>
        /// Delete the given entity of type <typeparamref name="TEntity"/> from the database
        /// </summary>
        /// <param name="entity">
        /// The entity to delete
        /// </param>
        /// <returns>
        /// <c>true</c> If one or more rows were affected, meaning the update succeded. <c>false</c>
        /// Otherwise
        /// </returns>
        bool Delete(TEntity entity);
    }
}
