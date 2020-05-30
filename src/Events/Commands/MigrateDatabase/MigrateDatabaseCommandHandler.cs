using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class MigrateDatabaseCommandHandler : IRequestHandler<MigrateDatabaseCommand, Result>
    {
        private readonly DatabaseMigrator _migrator;

        public MigrateDatabaseCommandHandler(DatabaseMigrator migrator)
        {
            _migrator = migrator;
        }
        
        public async Task<Result> Handle(MigrateDatabaseCommand request, CancellationToken cancellationToken)
        {
            await _migrator.Migrate();
            return Result.Ok();  
        }
    }
}