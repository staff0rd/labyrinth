using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Web
{
    public class TaskHubSender
    {
        private readonly IHubContext<TaskHub> _hubContext;

        public TaskHubSender(IHubContext<TaskHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Progress(string taskId, int progress)
        {
            await _hubContext.Clients.Group(TaskHub.GetGroup(taskId))
                .SendAsync("TaskProgress", new { Id = taskId, Progress = progress});
        }
    }
}