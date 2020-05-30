using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;

namespace Events
{
    [Command(Name = "migrate-database", Description = "Migrate the databse to latest schema")]
    public class MigrateDatabaseCommand : ResultCommand { }
}