namespace API.DepotEice.DAL.IRepositories;

public interface IRepositoryBase<TKey, TEntity>
    where TEntity : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerable<TEntity> GetAll();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    TEntity? GetByKey(TKey key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    TKey Create(TEntity entity);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    bool Update(TKey key, TEntity entity);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Delete(TKey key);
}
