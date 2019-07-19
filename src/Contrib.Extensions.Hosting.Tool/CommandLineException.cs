using System.Runtime.Serialization;

namespace Contrib.Extensions.Hosting.Tool
{
    [System.Serializable]
    public class CommandLineException : System.Exception
    {
        public CommandLineException() { }

        public CommandLineException(string message) : this(message, exitCode: 1) { }

        public CommandLineException(string message, System.Exception inner) : this(message, exitCode: 1, inner) { }

        public CommandLineException(string message, int exitCode) : base(message)
        {
            ExitCode = exitCode;
        }

        public CommandLineException(string message, int exitCode, System.Exception inner) : base(message, inner)
        {
            ExitCode = exitCode;
        }

        protected CommandLineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ExitCode = info.GetInt32("ExitCode");
        }

        public int ExitCode { get; } = 1;

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ExitCode", ExitCode);
        }
    }
}