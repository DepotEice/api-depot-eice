using DevHopTools.DataAccess.Interfaces;

namespace API.DepotEice.DAL.Repositories;

public abstract class RepositoryBase
{
    internal IDevHopConnection _connection;

    public RepositoryBase(IDevHopConnection connection)
    {
        if (connection is null)
            throw new ArgumentNullException(nameof(connection));
        
        _connection = connection;
    }
}
