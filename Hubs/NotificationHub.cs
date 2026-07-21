using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DevHub.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        // Simple mapping from UserId to ConnectionIds (a user can have multiple connections, e.g. multiple tabs)
        private static readonly ConcurrentDictionary<string, List<string>> UserConnections = new();

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                var connections = UserConnections.GetOrAdd(userId, _ => new List<string>());
                lock (connections)
                {
                    connections.Add(Context.ConnectionId);
                }
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(System.Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (!string.IsNullOrEmpty(userId))
            {
                if (UserConnections.TryGetValue(userId, out var connections))
                {
                    lock (connections)
                    {
                        connections.Remove(Context.ConnectionId);
                        if (!connections.Any())
                        {
                            UserConnections.TryRemove(userId, out _);
                        }
                    }
                }
            }
            return base.OnDisconnectedAsync(exception);
        }
    }
}
