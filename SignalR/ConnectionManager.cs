using Microsoft.AspNetCore.SignalR;

namespace WebAPI.SignalR
{
    public class ConnectionManager: Hub
    {
        public void SendToAll(int storageId)
        {
            Clients.All.InvokeAsync("UpdateStorage", storageId);
            
        }
    }
}
