using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace ApiDb.Util
{
    internal class AuthenticationHelper
    {
        private const string ClientId = "ad30ae9e-ac1b-4249-8817-d24f5d7ad3de";
        public static IPublicClientApplication App { get; } = CreateApp();

        internal static async Task<AuthenticationResult?> GetAuthenticationTokenAsync(string? userName, IEnumerable<string> scopes, ILogger logger, CancellationToken cancellationToken = default)
        {
            var accounts = await GetAccountsAsync(cancellationToken);
            if (accounts.Count == 0)
            {
                // Login time!
                var token = await AcquireNewTokenAsync(scopes, logger, cancellationToken);
                if (userName != null && !string.Equals(token.Account.Username, userName, StringComparison.Ordinal))
                {
                    logger.LogWarning("The '--username' value provided is {ExpectedUsername} but token acquisition returned {ActualUsername}. Using {ActualUsername}.", userName, token.Account.Username);
                }
                return token;
            }

            if (userName == null)
            {
                if (accounts.Count == 1)
                {
                    var account = accounts.First();
                    logger.LogInformation("Using user account: {Username}", account.Username);
                    return await App.AcquireTokenSilent(scopes, account).ExecuteAsync(cancellationToken);
                }
                else
                {
                    logger.LogError("Multiple tokens are cached. Specify which to use with the '--username' option. Use the 'tokens list' command to view all available tokens.");
                    return null;
                }
            }
            else
            {
                var account = accounts.First(a => string.Equals(a.Username, userName, StringComparison.Ordinal));
                if (account == null)
                {
                    return await AcquireNewTokenAsync(scopes, logger, cancellationToken);
                }

                logger.LogInformation("Using user account: {Username}", account.Username);
                return await App.AcquireTokenSilent(scopes, account).ExecuteAsync(cancellationToken);
            }
        }

        internal static async Task<IReadOnlyCollection<IAccount>> GetAccountsAsync(CancellationToken cancellationToken = default)
        {
            return (await App.GetAccountsAsync()).ToList();
        }

        internal static Task<AuthenticationResult> AcquireNewTokenAsync(IEnumerable<string> scopes, ILogger logger, CancellationToken cancellationToken = default)
        {
            return App.AcquireTokenWithDeviceCode(
                scopes,
                (result) =>
                {
                    logger.LogInformation(result.Message);
                    return Task.CompletedTask;
                }).ExecuteAsync(cancellationToken);
        }

        private static IPublicClientApplication CreateApp()
        {
            var cacheDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "apidb-util");
            var cachePath = Path.Combine(
                cacheDir,
                "tokencache.dat");

            var app = PublicClientApplicationBuilder
                .Create(ClientId)
                .Build();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                app.UserTokenCache.SetAfterAccessAsync(async (args) =>
                {
                    if (!Directory.Exists(cacheDir))
                    {
                        Directory.CreateDirectory(cacheDir);
                    }
                    var unprotected = args.TokenCache.SerializeMsalV3();
                    var content = ProtectedData.Protect(unprotected, null, DataProtectionScope.CurrentUser);
                    await File.WriteAllBytesAsync(cachePath, content);
                });

                app.UserTokenCache.SetBeforeAccessAsync(async (args) =>
                {
                    if (File.Exists(cachePath))
                    {
                        var content = await File.ReadAllBytesAsync(cachePath);
                        var unprotected = ProtectedData.Unprotect(content, null, DataProtectionScope.CurrentUser);
                        args.TokenCache.DeserializeMsalV3(unprotected);
                    }
                });
            }

            return app;
        }
    }
}