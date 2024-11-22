using Cursus.RepositoryContract.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Cursus.Service.Hubs
{
    public class ChatHub : Hub
    {

        public ChatHub()
        {
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"User {Context.ConnectionId.ToString().Substring(0, 2)} has joined the group {groupName}.");
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"User {Context.ConnectionId.ToString().Substring(0,2)} has leave the group {groupName}.");
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveMessage", $"User {Context.ConnectionId.ToString().Substring(0, 2)}: {message}");
        }
    }
}
