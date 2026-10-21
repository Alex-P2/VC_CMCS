using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace ST10445832_PROG6212_POE.Hubs
{
    public class StatusHub : Hub
    {
        public void ClaimStatusUpdated(int claimID, string statusTitle)
        {
            Clients.All.SendAsync("ReceiveStatusUpdate", claimID, statusTitle);
        }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\