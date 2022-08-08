namespace API.DepotEice.BLL.IServices
{
    public interface IServiceBase<TKey, TModel>
    {
        IEnumerable<TModel> GetAll();
        TModel? Create(TModel data);
        TModel? GetByKey(TKey key);
        TModel? Update(TKey key, TModel data);
        bool Delete(TKey key);
    }
}
