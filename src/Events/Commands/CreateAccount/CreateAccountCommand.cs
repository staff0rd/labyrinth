using MediatR;

namespace Events 
{
    public class CreateAccountCommand : IRequest<Result>
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}