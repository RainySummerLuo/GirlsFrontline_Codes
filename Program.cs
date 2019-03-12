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
            var dirPath = Environment.CurrentDirectory;
            dirPath = Path.Combine(Directory.GetParent(Directory.GetParent(dirPath).ToString()) + @"\data\");
            if (!Directory.Exists(Path.Combine(dirPath + @"download\")))
            {
                Directory.CreateDirectory(Path.Combine(dirPath + @"download\"));
            }
            Console.WriteLine("Please choose which server to download (en/cn):");
            string serverInput;
            do
            {
                serverInput = Console.ReadLine();
            } while (serverInput != "en" && serverInput != "cn");
            if (!Directory.Exists(Path.Combine(dirPath + @"download\" + serverInput + @"\")))
            {
                Directory.CreateDirectory(Path.Combine(dirPath + @"download\" + serverInput + @"\"));
            }
            Console.WriteLine();

            var filePath = Path.Combine(dirPath + @"\wiki-" + serverInput + ".html");
            /*
            try
            {
            */
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var streamReader = new StreamReader(fileStream, Encoding.Default);
            fileStream.Seek(0, SeekOrigin.Begin);

            string serverName = null;
            if (serverInput == "cn") serverName = "http://www.gfwiki.org";
            if (serverInput == "en") serverName = "https://www.gfwiki.com";

            var lineCount = 0;
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var sr = new StreamReader(fs, Encoding.Default);
            while (sr.ReadLine() != null)
            {
                lineCount++;
            }
            fs.Close();
            sr.Close();
            var content = streamReader.ReadLine();
            var downloadCount = 0;
            var totalCount = 0;
            Console.WriteLine("Start Download: totally " + lineCount + " items.");
            //var myWebClient = new WebClient();
            /*
            var handler = new WebRequestHandler();
            X509Certificate2 certificate = GetMyX509Certificate();
            handler.ClientCertificates.Add(certificate);
            */
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            client.DefaultRequestHeaders.Connection.Add("keep-alive");
            while (content != null)
            {
                totalCount++;
                var uriSplit = content.Split(Convert.ToChar("\""));
                var url = serverName + uriSplit[1].Trim();
                var localPath = url.Split('/')[6].Trim();
                var downloadPath = Path.Combine(Path.Combine(dirPath + @"download\" + serverInput + @"\") +
                                                HttpUtility.UrlDecode(localPath, Encoding.UTF8));
                var picBool = true;
                if (serverInput == "cn") {
                    picBool = (localPath.Substring(0, 3) == "Pic");
                }
                if (picBool && File.Exists(downloadPath) == false)
                {
                    //myWebClient.DownloadFile(url, downloadPath);
                    var picContent = await client.GetByteArrayAsync(url);
                    var strPicContent = Encoding.Default.GetString(picContent);
                    if (strPicContent.Substring(0, 15) == "<!DOCTYPE html>") {
                        Console.WriteLine("[ERROR] " + url);
                    } else {
                        downloadCount++;
                        var downloadFs = new FileStream(downloadPath, FileMode.Create);
                        downloadFs.Write(picContent, 0, picContent.Length);
                        var progress = (decimal)totalCount / lineCount * 100;
                        Console.WriteLine("[" + progress.ToString("0.00") + "%][NO." + downloadCount + " Download] " + url);
                    }
                }
                content = streamReader.ReadLine();
            }
            fileStream.Close();
            streamReader.Close();
            Console.WriteLine("All the download links have been vistited.");
            Console.ReadLine();
            /*}
            catch
            {
                // ignored
            }
            finally
            {
                fileStream?.Close();
                streamReader?.Close();
            }*/
        }

        /*
        private static void Downloader(string url, string filepath)
        {
        }
        */
    }
}
