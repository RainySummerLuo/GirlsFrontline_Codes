using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

// ReSharper disable once IdentifierTypo
namespace GirlsFrontline_Downloader
{
    internal static class Program
    {
        private static async Task Main()
        {
            var currentDirectory = Environment.CurrentDirectory;
            var dataPath = Path.Combine(currentDirectory + @"\data\");
            var downPath = Path.Combine(dataPath + @"\download\");
            if (!Directory.Exists(Path.Combine(downPath)))
            {
                Directory.CreateDirectory(Path.Combine(downPath));
            }
            Console.WriteLine("Girls' Frontline Characters Image Download Tool");
            Console.WriteLine(@"by @RainySummer");
            string server;
            do
            {
                Console.Write("\nPlease choose which server to download (cn/en/jp): ");
                server = Console.ReadLine();
            } while (server != "cn" && server != "en" && server != "jp");

            var downServerPath = Path.Combine(downPath + @"\" + server + @"\");
            if (!Directory.Exists(downServerPath))
            {
                Directory.CreateDirectory(downServerPath);
            }
            Console.WriteLine();

            var filePath = Path.Combine(dataPath + @"\wiki\" + server + ".html");
            if (!File.Exists(filePath))
            {
                Console.WriteLine("[ERROR] Download links file not found.");
                Console.ReadLine();
                return;
            }
            
            string serverName = null;
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (server)
            {
                case "cn":
                    serverName = "http://www.gfwiki.org";
                    break;
                case "en":
                    serverName = "https://en.gfwiki.com";
                    break;
                case "jp":
                    serverName = "https://wiki.gamerclub.jp";
                    break;
            }
            
            var lineCount = 0;
            var countFs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var countSr = new StreamReader(countFs, Encoding.Default);
            while (countSr.ReadLine() != null)
            {
                lineCount++;
            }
            countFs.Close();
            countSr.Close();
            
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(fs, Encoding.UTF8);
            fs.Seek(0, SeekOrigin.Begin);
            string line;
            var downCount = 0;
            Console.WriteLine("Start Download: totally " + lineCount + " items.");
            
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            while ((line = sr.ReadLine()) != null)
            {
                downCount++;
                if (!line.Contains("<a href=\"") || !line.Contains("images"))
                {
                    continue;
                }
                var uriSplit = line.Split(new[]{"a href=\""}, StringSplitOptions.RemoveEmptyEntries);
                uriSplit = uriSplit[1].Split('"');
                var url = serverName + uriSplit[0].Trim();
                var filenameSplit = url.Split('/');
                var filename = filenameSplit[filenameSplit.Length - 1].Trim();
                var downloadPath = Path.Combine(downServerPath +
                                                HttpUtility.UrlDecode(filename, Encoding.UTF8));
                var sizeSplit = line.Split(new[] {" . . "}, StringSplitOptions.None)[1].Split('×');
                var imageWidth = sizeSplit[0].Trim();
                var imageHeight = sizeSplit[1].Trim();
                int.TryParse(imageWidth.Replace(",", ""), out var intImageWidth);
                
                bool picBool = false;
                switch (server)
                {
                    case "cn":
                        picBool = (filename.Substring(0, 3) == "Pic");
                        break;
                    case "en":
                        picBool = (imageWidth == imageHeight &&  intImageWidth >= 1024);
                        break;
                    case "jp":
                        picBool = (filename.Contains("%E7%AB%8B%E7%B5%B5"));
                        break;
                }
                if (!picBool) continue;
                if (File.Exists(downloadPath))
                {
                    Console.WriteLine("[ERROR] " + filename + " already existed.");
                    continue;
                }
                byte[] picContent;
                try
                {
                    picContent = await client.GetByteArrayAsync(url);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("[ERROR] " + url + " | " + e.Message);
                    File.Create(downloadPath);
                    continue;
                }
                catch (Exception e)
                {
                    Console.WriteLine("[ERROR] " + url + " | " + e.Message);
                    continue;
                }
                if (Encoding.Default.GetString(picContent).Substring(0, 15) == "<!DOCTYPE html>") {
                    Console.WriteLine("[ERROR] " + url);
                    File.Create(downloadPath);
                    continue;
                }
                var downFs = new FileStream(downloadPath, FileMode.Create);
                downFs.Write(picContent, 0, picContent.Length);
                downFs.Close();
                var progress = (decimal)downCount / lineCount * 100;
                Console.WriteLine("[DOWNL] [" + progress.ToString("0.00") + "%] " + filename);
            }
            fs.Close();
            sr.Close();
            Console.WriteLine("All download links have been vistited.");
            Console.ReadLine();
        }
    }
}
