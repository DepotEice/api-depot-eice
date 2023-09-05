namespace API.DepotEice.UIL.Hubs
{
    public interface IChatHub
    {
        Task CommunicateNewUserArrived(string connectionId);
        Task SendMessageAsync(string user, string message);
    }
}
