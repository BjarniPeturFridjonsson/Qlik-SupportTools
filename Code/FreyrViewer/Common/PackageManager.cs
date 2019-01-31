using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Bifrost.Model.Models;
using Eir.Common.Extensions;
using Eir.Common.Logging;
using Odin.Common;
using Odin.Common.Stores;

namespace FreyrViewer.Common
{
    public class PackageManager
    {
        private readonly IStoreFactory _storeFactory;
        private readonly string _userAgent = $"Qlik Common/{typeof(PackageManager).Assembly.GetName().Version}; (Package Manager)";

        public PackageManager(IStoreFactory storeFactory)
        {
            _storeFactory = storeFactory;
        }

        public async Task<bool> IsLatestVersion(Guid applicationId, string version)
        {
            IApplicationVersionStore applicationVersionStore = _storeFactory.GetApplicationVersionStore(_userAgent);
            var latest = await applicationVersionStore.GetLatest(applicationId);
            return !NewerVersionExist(version, latest.ToString());
        }

        private static bool NewerVersionExist(string myVersion, string latestVersion)
        {
            try
            {
                Version v1 = new Version(myVersion);
                Version v2 = new Version(latestVersion);
                int result = v1.CompareTo(v2);
                return result < 0;
            }
            catch
            {
                // Fallback on legacy logic.
                return !latestVersion.Equals(myVersion);
            }
        }

        /// <summary>
        /// Downloads the package for the latest version of the application ID. 
        /// </summary>
        /// <param name="applicationId">The GUID of the application for which to download the latest version</param>
        /// <returns>The path to the downloaded Package file</returns>
        public async Task<string> DownloadLatestVersionPackage(Guid applicationId)
        {
            try
            {
                IApplicationVersionStore applicationVersionStore = _storeFactory.GetApplicationVersionStore(_userAgent);
                var latest = await applicationVersionStore.GetLatest(applicationId);

                Log.To.Main.Add($"Downloading package from {latest.PackageUri}");

                using (var webClient = new WebClient())
                {
                    var path = Path.GetTempPath();
                    var fileName = Path.Combine(path, Guid.NewGuid().ToString("N") + ".zip");

                    webClient.Headers.Add(HttpRequestHeader.UserAgent, _userAgent);
                    webClient.UseDefaultCredentials = true;
                    webClient.Headers.Add("Accept", "application/x-zip-compressed");


                    await webClient.DownloadFileTaskAsync(latest.PackageUri, fileName);
                    Log.To.Main.Add("Package downloaded");

                    if (!GetMd5(fileName).Equals(latest.Md5))
                    {
                        throw new Exception("Checksum verification failed for downloaded package");
                    }

                    return fileName;
                }
            }
            catch (Exception ex)
            {
                Log.To.Main.Add($"Error when downloading package: {ex.GetNestedMessages()}");
                throw;
            }
        }

        public async Task<ApplicationVersion> GetLatestVersionInformation(Guid applicationId, string version)
        {
            IApplicationVersionStore applicationVersionStore = _storeFactory.GetApplicationVersionStore(_userAgent);
            var latest = await applicationVersionStore.GetLatest(applicationId);
            return latest;
        }

        private static string GetMd5(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open))
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] hash = md5.ComputeHash(fs);
                    return string.Concat(hash.Select(x => x.ToString("x2")));
                }
            }
        }

        public static bool InstallApplication(string packageFilename)
        {
            string tmpDir = "Not loaded yet";
            try
            {
                tmpDir = GetTemporaryDirectory();
                string tmpUpdaterFilename = GetUpdaterProgramFilename(tmpDir);
                var args = GetArguments(packageFilename);
                var argString = string.Join(" ", args.ToArray());
                Log.To.Main.Add($"Starting new Installation application witha params {argString}");
                var info = new ProcessStartInfo(tmpUpdaterFilename, argString)
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false
                };
                Process.Start(info);
                return true;
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException($"Failed installing application with packageFilename{packageFilename} and tempDir {tmpDir}", ex);
                return false;
            }
        }

        private static List<string> GetArguments(string packageFilename)
        {
            return new List<string>
            {
                "\"/package=" + packageFilename + "\"",
                "\"/destination=" + Directory.GetCurrentDirectory() + "\"",
                "\"/restartexe=" + Assembly.GetEntryAssembly().Location + "\"",
                "\"/restartargs=" + GetRestartArgs() + "\"",
                "\"/processid=" + Process.GetCurrentProcess().Id + "\""
            };
        }

        private static string GetRestartArgs()
        {
            // first element of GetCommandLineArgs result is always the exe file, skip
            // that and take the rest.
            return string.Join(" ", Environment.GetCommandLineArgs().Skip(1));
        }

        private static string GetUpdaterProgramFilename(string tmpDir)
        {
            var sourceFiles = new[]
            {
                "ApplicationUpdater.exe",
            };

            if (sourceFiles.Select(f => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, f)).Any(f => !File.Exists(f)))
            {
                throw new Exception("Updater program is missing");
            }

            foreach (var file in sourceFiles)
            {
                File.Copy(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file), Path.Combine(tmpDir, file));
            }

            return sourceFiles.Select(f => Path.Combine(tmpDir, f)).First(f => f.EndsWith("ApplicationUpdater.exe", StringComparison.OrdinalIgnoreCase));
        }

        private static string GetTemporaryDirectory()
        {
            string tempFileName = Path.GetTempFileName();
            if (File.Exists(tempFileName)) File.Delete(tempFileName);
            Directory.CreateDirectory(tempFileName);
            return tempFileName;
        }
    }
}