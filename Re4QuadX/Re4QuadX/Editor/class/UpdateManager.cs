using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Re4QuadX.Editor.Class
{
    public static class UpdateManager
    {
        private const string repo_owner = "r3nzk";
        private const string repo_name = "Re4QuadX";

        public static async Task<bool> CheckForUpdates()
        {
            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.Add(
                        new ProductInfoHeaderValue(repo_name, currentVersion.ToString()));

                    string url = $"https://api.github.com/repos/{repo_owner}/{repo_name}/releases/latest";
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    JObject releaseInfo = JObject.Parse(json);

                    string releaseName = (string)releaseInfo["name"];
                    string htmlUrl = (string)releaseInfo["html_url"];

                    if (string.IsNullOrEmpty(releaseName)){
                        Editor.Console.Warning("Update check failed: Could not find 'name' in GitHub API response.");
                        return false;
                    }

                    string versionString = releaseName.TrimStart('v', 'V');

                    if (Version.TryParse(versionString, out Version latestVersion)){
                        if (latestVersion > currentVersion)
                        {
                            ShowUpdateNotification(currentVersion, latestVersion, htmlUrl);
                            return true; //update is available
                        }
                    }else {
                        Editor.Console.Warning($"Update check failed: Could not parse version from release name '{releaseName}'.");
                    }
                }
            }
            catch (Exception ex){
                Editor.Console.Error($"Update check failed: {ex.Message}");
            }

            return false;
        }

        private static void ShowUpdateNotification(Version current, Version latest, string url)
        {
            string title = "Update Available";

            string message = $"A new version of RE4QuadX is available!\n\n" +
                             $"Your version: {current.ToString()}\n" +
                             $"Latest version: {latest.ToString()}\n\n" +
                             "Would you like to open the download page?";

            DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            if (result == DialogResult.Yes){
                try{
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true});
                }catch (Exception ex){
                    Editor.Console.Error($"Could not open the URL. Error: {ex.Message}");
                }
            }
        }
    }
}