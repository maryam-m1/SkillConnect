using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SkillConnect.Hubs
{
    // Ye Hub real-time connections ko handle karega
    public class MatchingHub : Hub
    {
        // Jab koi user Matching page open karega, ye usey welcome karega
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        // Aap yahan matching algorithm ki real-time updates bhi add kar sakte hain
    }
}