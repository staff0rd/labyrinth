using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Web
{
    public class TaskHub : Hub
    {
        public async Task Subscribe(string taskId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GetGroup(taskId));
        }

        public static string GetGroup(string taskId)
        {
            return $"task:{taskId}";
        }
    }
}