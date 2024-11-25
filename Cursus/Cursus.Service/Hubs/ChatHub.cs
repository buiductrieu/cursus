using Cursus.RepositoryContract.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Cursus.Service.Hubs
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, int> UserNumbers = new();
        private static int NextUserNumber = 1;

        public override async Task OnConnectedAsync()
        {
            // Assign a user number if this is a new connection
            if (!UserNumbers.ContainsKey(Context.ConnectionId))
            {
                UserNumbers[Context.ConnectionId] = NextUserNumber++;
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (UserNumbers.TryRemove(Context.ConnectionId, out int userNumber))
            {
                await Clients.All.SendAsync("SystemMessage", $"User {userNumber} has left the chat.");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            if (UserNumbers.TryGetValue(Context.ConnectionId, out int userNumber))
            {
                await Clients.Group(groupName).SendAsync("ReceiveMessage", $"{Context.ConnectionId}", $"User {userNumber}: {message}");
            }
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            if (UserNumbers.TryGetValue(Context.ConnectionId, out int userNumber))
            {
                await Clients.Group(groupName).SendAsync("SystemMessage", $"User {userNumber} has joined the group.");
            }
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            if (UserNumbers.TryGetValue(Context.ConnectionId, out int userNumber))
            {
                await Clients.Group(groupName).SendAsync("SystemMessage", $"User {userNumber} has left the group.");
            }
        }
    }
}
