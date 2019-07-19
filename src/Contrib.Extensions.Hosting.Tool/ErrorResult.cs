using System;
using System.CommandLine.Invocation;
using Microsoft.Extensions.Logging;

namespace Contrib.Extensions.Hosting.Tool
{
    internal class ErrorResult : IInvocationResult
    {
        private readonly Exception _exception;
        private readonly ILogger _logger;

        public ErrorResult(Exception exception, ILogger logger)
        {
            _exception = exception;
            _logger = logger;
        }

        public void Apply(InvocationContext context)
        {
            if (_exception is CommandLineException clex)
            {
                _logger.LogError(clex.Message);
                context.ResultCode = clex.ExitCode;
            }
            else
            {
                _logger.LogError(_exception, "Unhandled {ExceptionType}: {ExceptionMessage}", _exception.GetType(), _exception.Message);
                context.ResultCode = 1;
            }
        }
    }
}