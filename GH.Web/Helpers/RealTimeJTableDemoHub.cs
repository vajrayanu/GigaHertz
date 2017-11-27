using SignalR.Hubs;

namespace GH.Web.Controllers
{
    public class RealTimeJTableDemoHub : Hub
    {
        public void SendMessage(string clientName, string message)
        {
            Clients.GetMessage(clientName, message);
        }
    }
}