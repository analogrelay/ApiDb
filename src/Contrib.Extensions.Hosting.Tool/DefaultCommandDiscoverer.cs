using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.DragonFruit;
using System.CommandLine.Invocation;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Contrib.Extensions.Hosting.Tool
{
    internal class DefaultCommandDiscoverer : ICommandDiscoverer
    {
        private readonly IServiceProvider _services;
        private readonly ILogger<DefaultCommandDiscoverer> _logger;

        public DefaultCommandDiscoverer(IServiceProvider services, ILogger<DefaultCommandDiscoverer> logger)
        {
            _services = services;
            _logger = logger;
        }

        public CommandLineBuilder CreateCommandLineBuilder()
        {
            // Check for the original EntryPoint
            var exeAsm = Assembly.GetEntryAssembly();
            var entryPoint = exeAsm.EntryPoint;
            if (entryPoint == null)
            {
                throw new InvalidOperationException("Failed to locate assembly entry point!");
            }
            _logger.LogInformation("Discovered entry point type: {EntryPointType}.", entryPoint.DeclaringType);
            var programType = entryPoint.DeclaringType;

            var builder = BuildCommand(programType, _services, rootBuilder: null);

            return builder;
        }

        internal CommandLineBuilder BuildCommand(Type programType, IServiceProvider services, CommandLineBuilder? rootBuilder)
        {
            var name = rootBuilder == null ?
                GetName(programType.Assembly) :
                GetName(programType);
            var builder = new CommandLineBuilder(new Command(name));

            rootBuilder = rootBuilder ?? builder;

            // Find the execute method
            var executeMethod = FindMethod(programType, "ExecuteAsync");

            // Load Sub-commands
            var subcommands = CollectSubcommands(programType, services, rootBuilder).ToList();
            foreach (var command in subcommands)
            {
                _logger.LogInformation("Adding sub-command: {SubCommandName} to {RootCommandName}.", command.Name, builder.Command.Name);
                builder.AddCommand(command);
            }

            if (executeMethod != null)
            {
                _logger.LogInformation("Configuring execute method: {ExecuteMethodType}.{ExecuteMethodName}", executeMethod.DeclaringType, executeMethod.Name);
                // The 'target' argument isn't even used...
                builder.Command.ConfigureFromMethod(executeMethod, target: () => null);

                // Try to add docs from XML doc comments
                var docCommentsPath = Path.ChangeExtension(executeMethod.DeclaringType.Assembly.Location, ".xml");
                if (!File.Exists(docCommentsPath))
                {
                    _logger.LogWarning("Could not find documentation xml file: {DocXmlPath}.", docCommentsPath);
                }
                builder.ConfigureHelpFromXmlComments(executeMethod, docCommentsPath);

                // Activate the target via DI
                // We need to add this to the root builder because the middleware on our builder will be lost.
                rootBuilder.UseMiddleware(c => c.BindingContext.AddService(executeMethod.DeclaringType, () => ActivatorUtilities.CreateInstance(services, executeMethod.DeclaringType, Array.Empty<object>())));
            }
            else
            {
                builder.Command.Handler = CommandHandler.Create<IConsole>((console) =>
                {
                    new HelpBuilder(console).Write(builder.Command);
                });
            }

            ConfigureFromAttribute<DescriptionAttribute>(
                programType,
                builder.Command,
                (cmd, attr) => cmd.Description = attr.Description);

            ConfigureFromAttribute<DisplayNameAttribute>(
                programType,
                builder.Command,
                (cmd, attr) => cmd.Name = attr.DisplayName);

            return builder;
        }

        private void ConfigureFromAttribute<TAttr>(MemberInfo member, Command command, Action<Command, TAttr> configurer)
            where TAttr : Attribute
        {
            var attr = member.GetCustomAttribute<TAttr>(inherit: true);
            if (attr == null)
            {
                return;
            }
            else
            {
                configurer(command, attr);
            }
        }

        private string GetName(Assembly entryPoint)
        {
            // TODO: An attribute to override?
            return entryPoint.GetName().Name;
        }

        private string GetName(Type programType)
        {
            // TODO: An attribute to override?
            if (programType.Name.EndsWith("Command"))
            {
                return programType.Name.Substring(0, programType.Name.Length - 7).ToKebabCase();
            }
            else
            {
                return programType.Name.ToKebabCase();
            }
        }

        private IEnumerable<Command> CollectSubcommands(Type programType, IServiceProvider services, CommandLineBuilder rootBuilder)
        {
            var getSubCommandsMethod = FindMethod(programType, "GetSubcommands");
            if (getSubCommandsMethod == null)
            {
                _logger.LogDebug("No subcommands for {ProgramType}.", programType);
                return Array.Empty<Command>();
            }

            var parameters = getSubCommandsMethod.GetParameters();

            var args = Array.Empty<object>();
            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(IServiceProvider))
            {
                args = new object[] { services };
            }

            if (!getSubCommandsMethod.IsStatic)
            {
                throw new InvalidOperationException("GetSubcommands must be static, if present.");
            }

            if (getSubCommandsMethod.ReturnType == typeof(IEnumerable<Command>))
            {
                _logger.LogDebug("Getting subcommand instances from {ProgramType}.", programType);
                return (IEnumerable<Command>)getSubCommandsMethod.Invoke(null, args);
            }
            else if (getSubCommandsMethod.ReturnType == typeof(IEnumerable<Type>))
            {
                _logger.LogDebug("Getting subcommand types from {ProgramType}.", programType);
                var types = (IEnumerable<Type>)getSubCommandsMethod.Invoke(null, args);
                return types.Select(t => BuildCommand(t, services, rootBuilder).Command);
            }
            else
            {
                throw new InvalidOperationException("GetSubcommands must return 'IEnumerable<Command>' or 'IEnumerable<Type>', if present");
            }
        }

        private MethodInfo? FindMethod(Type programType, string methodName)
        {
            var candidates = programType
                .GetMethods()
                .Where(m => m.IsPublic && m.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
                .ToList();
            _logger.LogDebug("Found {CandidateCount} methods with name {MethodName} on {ProgramType}.", candidates.Count, methodName, programType);
            if (candidates.Count == 0)
            {
                return null;
            }
            else if (candidates.Count > 1)
            {
                throw new AmbiguousMatchException($"Multiple '{programType.FullName}.{methodName}' methods were found!");
            }

            return candidates[0];
        }
    }
}