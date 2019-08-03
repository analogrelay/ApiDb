// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Contrib.Extensions.Hosting.Tool
{
    /// <summary>
    /// Listens for Ctrl+C or SIGTERM and initiates shutdown.
    /// </summary>
    public class ToolLifetime : IHostLifetime, IDisposable
    {
        private readonly ManualResetEvent _shutdownBlock = new ManualResetEvent(false);
        private CancellationTokenRegistration _applicationStartedRegistration;
        private CancellationTokenRegistration _applicationStoppingRegistration;

        public ToolLifetime(IHostEnvironment environment, IHostApplicationLifetime applicationLifetime, IOptions<HostOptions> hostOptions, ILoggerFactory loggerFactory)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            ApplicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            HostOptions = hostOptions?.Value ?? throw new ArgumentNullException(nameof(hostOptions));
            Logger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
        }

        private IHostEnvironment Environment { get; }

        private IHostApplicationLifetime ApplicationLifetime { get; }

        private HostOptions HostOptions { get; }

        private ILogger Logger { get; }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            _applicationStartedRegistration = ApplicationLifetime.ApplicationStarted.Register(state =>
            {
                ((ToolLifetime)state).OnApplicationStarted();
            },
            this);
            _applicationStoppingRegistration = ApplicationLifetime.ApplicationStopping.Register(state =>
            {
                ((ToolLifetime)state).OnApplicationStopping();
            },
            this);

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.CancelKeyPress += OnCancelKeyPress;

            // Console applications start immediately.
            return Task.CompletedTask;
        }

        private void OnApplicationStarted()
        {
            Logger.LogInformation("Application started. Press Ctrl+C to shut down.");
            Logger.LogInformation("Hosting environment: {envName}", Environment.EnvironmentName);
            Logger.LogInformation("Content root path: {contentRoot}", Environment.ContentRootPath);
        }

        private void OnApplicationStopping()
        {
            Logger.LogInformation("Application is shutting down. Press Ctrl+C again to forcibly terminate the app.");
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            ApplicationLifetime.StopApplication();
            _shutdownBlock.WaitOne();
            // On Linux if the shutdown is triggered by SIGTERM then that's signaled with the 143 exit code.
            // Suppress that since we shut down gracefully. https://github.com/aspnet/AspNetCore/issues/6526
            System.Environment.ExitCode = 0;
        }

        private void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if(ApplicationLifetime.ApplicationStopping.IsCancellationRequested)
            {
                // Fast shutdown
                Console.WriteLine("Forcibly terminating.");
                _shutdownBlock.Set();
                return;
            }
            e.Cancel = true;
            ApplicationLifetime.StopApplication();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // There's nothing to do here
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _shutdownBlock.Set();

            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
            Console.CancelKeyPress -= OnCancelKeyPress;

            _applicationStartedRegistration.Dispose();
            _applicationStoppingRegistration.Dispose();
        }
    }
}