using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bifrost.Model.Models;
using Eir.Common.Logging;
using FreyrViewer.Common;
using Odin.Common;

namespace FreyrViewer.Ui
{
    public static class StartupSupport
    {
        private static readonly bool _skipAskDownload = Debugger.IsAttached;
        private static readonly Guid ApplicationId = Guid.Parse("0fc0e9b3-c98f-4aec-9824-5498ea1ba165");
        private static readonly string Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static async Task<bool> AppStartupAsync(IStoreFactory storeFactory)
        {
            ApplicationVersion newerVersion = await TryGetNewerVersion(storeFactory);

            if (newerVersion == null)
            {
                return true;
            }

            if (await AskInstallNewerVersion(storeFactory, newerVersion))
            {
                return false;
            }

            return true;
        }

        private static async Task<ApplicationVersion> TryGetNewerVersion(IStoreFactory storeFactory)
        {
            try
            {
                var packageManager = new PackageManager(storeFactory);

                if (await packageManager.IsLatestVersion(ApplicationId, Version))
                {
                    return null;
                }

                return await packageManager.GetLatestVersionInformation(ApplicationId, Version);
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException("Error in TryGetNewerVersion", ex);
                return null;
            }
        }

        private static async Task<bool> AskInstallNewerVersion(IStoreFactory storeFactory, ApplicationVersion newerVersion)
        {
            if (_skipAskDownload)
            {
                return false;
            }

            try
            {
                Log.To.Main.Add($"There is a new version available. Current version: {Version}, available version: {newerVersion}");

                DialogResult doYouWantToDownload = Mbox.Show(
                    string.Join("\r\n",
                        "Great Scott! There's an update available.",
                        $"Changes in version {newerVersion} include:",
                        "",
                        newerVersion.Description,
                        "",
                        "Would you like to download and install the latest version?"),
                    null,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (doYouWantToDownload != DialogResult.Yes)
                {
                    return false;
                }
                //SplashManager.Loader.ShowFloatingSplash(SplashManager.Loader.GetDefaultOwner());
                //splashFormStatus.Text = "Downloading...";
                

                var packageManager = new PackageManager(storeFactory);

                string packageFilename = await packageManager.DownloadLatestVersionPackage(ApplicationId);

                return PackageManager.InstallApplication(packageFilename);
            }
            catch (Exception ex)
            {
                Log.To.Main.AddException("Error in AskInstallNewerVersion", ex);
                return false;
            }
        }
    }
}