﻿using System;
using System.IO;
using System.Net;
using System.Text;

// ReSharper disable once IdentifierTypo
namespace GirlsFrontline_Downloader
{
    internal static class Program
    {
        private static void Main()
        {
            var dirPath = Environment.CurrentDirectory;
            dirPath = Path.Combine(Directory.GetParent(Directory.GetParent(dirPath).ToString()) + @"\data\");
            Console.WriteLine("Please choose which server to download (en/cn):");
            string serverInput;
            do
            {
                serverInput = Console.ReadLine();
            } while (serverInput != "en" && serverInput != "cn");
            var filePath = Path.Combine(dirPath + @"\wiki-" + serverInput + ".html");
            FileStream fileStream = null;
            StreamReader streamReader = null;
            try
            {
                fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                streamReader = new StreamReader(fileStream, Encoding.Default);
                fileStream.Seek(0, SeekOrigin.Begin);

                var content = streamReader.ReadLine();
                var myWebClient = new WebClient();
                var count = 0;
                while (content != null)
                {
                    count++;
                    var uriSplit = content.Split(Convert.ToChar("\""));
                    var url = "http://www.gfwiki.org" + uriSplit[1].Trim();
                    var localPath = url.Split('/')[6].Trim();
                    var downloadPath = Path.Combine(Path.Combine(dirPath + @"download\" + serverInput + @"\") + localPath);
                    myWebClient.DownloadFile(url, downloadPath);
                    Console.WriteLine("[NO."+ count +" Download] " + url);
                    content = streamReader.ReadLine();
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                fileStream?.Close();
                streamReader?.Close();
            }
        }

        /*
        private static void Downloader(string url, string filepath)
        {

        }
        */
    }
}
