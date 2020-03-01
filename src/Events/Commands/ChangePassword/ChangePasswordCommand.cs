using MediatR;

namespace Events
{
    public class ChangePasswordCommand : IRequest<Result>
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}