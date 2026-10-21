using Microsoft.AspNetCore.SignalR;
using ST10445832_PROG6212_POE.Hubs;

namespace ST10445832_PROG6212_POE.Data
{
    public class StatusListener
    {
        private readonly IHubContext<StatusHub> _hubContext;

        public StatusListener(IHubContext<StatusHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyOnStatusUpdate(int claimID, string statusTitle)
        {
            await _hubContext.Clients.All.SendAsync("ClaimStatusUpdated", claimID, statusTitle);
        }
    }
}
//////////////////////////////////// END OF FILE \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\