using System;
using System.IO;
using System.Net;
using System.Text;

namespace GirlsFrontline_Downloader
{
    internal static class Program
    {
        private static void Main() {
            var filePath = Environment.CurrentDirectory.ToString();
            filePath = Directory.GetParent(filePath).ToString();
            filePath = Directory.GetParent(filePath).ToString();
            filePath = Path.Combine(filePath + @"\data\wiki-cn.html");
            FileStream fileStream = null;
            StreamReader streamReader = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                streamReader = new StreamReader(fileStream, Encoding.Default);
                fileStream.Seek(0, SeekOrigin.Begin);
 
                var content = streamReader.ReadLine();
                var myWebClient = new WebClient();
                while (content != null) {
                    var uriSplit = content.Split(Convert.ToChar("\""));
                    var url = "http://www.gfwiki.org" + uriSplit[1].Trim();
                    var localPath = url.Split('/')[6].Trim();
                    myWebClient.DownloadFile(url, Path.Combine("D:\\gfwiki\\cn\\" + localPath));
                    Console.WriteLine("[Download] " + url);
                    content = streamReader.ReadLine();
                }
            } catch {
                // ignored
            } finally {
                fileStream?.Close();
                streamReader?.Close();
            }
        }

        private static void Downloader(string uri) {

        }
    }
}
