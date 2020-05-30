using System.Linq;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace Console
{
    public class RegisterCommands
    {
        public RegisterCommands(Assembly assembly)
        {
            _assembly = assembly;
        }

        public virtual void Apply(CommandLineApplication app)
        {
            var types =  _assembly
                .GetTypes()
                .Where(t => t.IsDefined(typeof(CommandAttribute), false));
            
            var contextArgs = new object[] { app };

            foreach (var type in types)
            {
                var impl = s_addSubcommandMethod.MakeGenericMethod(type);
                try
                {
                    impl.Invoke(this, contextArgs);
                }
                catch (TargetInvocationException ex)
                {
                    // unwrap
                    throw ex.InnerException ?? ex;
                }
            }
        }

        private static readonly MethodInfo s_addSubcommandMethod
            = typeof(RegisterCommands).GetRuntimeMethods()
                .Single(m => m.Name == nameof(AddCommandImpl));
        private readonly Assembly _assembly;

        private void AddCommandImpl<TCommand>(CommandLineApplication app)
            where TCommand : class
        {
            app.Command<TCommand>(null, null);
        }
    }
}
