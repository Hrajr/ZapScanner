using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System.Xml.Linq;
using System.Net.Http;
using System.Net;
using HtmlAgilityPack;

namespace SecurityScanner
{
    public class program
    {
        private Process proc;
        private string FileDirectory;
        public string Target;
        public string assetsDirectory = System.IO.Directory.GetCurrentDirectory().ToString() + @"\..\..\..\assets\";
        public List<XElement> RiskFound;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting security scanner\r");
        }

        public async Task<bool> UrlIsReachable()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(Target);
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Failed connecting to target:\r {exception}");
                return false;
            }
        }

        public bool StartZapScan()
        {
            try
            {
                ClearLastSession();
                string command = $@"/c zap.bat -quickurl {Target} -quickout {SetFileName("xml")} -quickprogress -cmd";
                ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
                info.Arguments = command;
                info.RedirectStandardInput = true;
                info.UseShellExecute = false;
                info.WorkingDirectory = assetsDirectory + "zap";

                proc = Process.Start(info);
                proc.WaitForExit();
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Executing ZAP scan failed:\r {exception}");
                return false;
            }
        }

        public bool StartScan()
        {
            try
            {
                ClearLastSession();
                string command = $@"/c zap.bat -cmd -newsession {assetsDirectory}scans\ -autorun config.yml";
                ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
                info.Arguments = command;
                info.RedirectStandardInput = true;
                info.UseShellExecute = false;
                info.WorkingDirectory = assetsDirectory + "zap";

                proc = Process.Start(info);
                proc.WaitForExit();
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Executing ZAP scan failed:\r {exception}");
                return false;
            }
        }

        public List<string> GenerateResultHTML()
        {
            try
            {
                string command = $@"/c zap.bat -cmd -session {assetsDirectory}scans\ -last_scan_report {SetFileName("html")}";
                ProcessStartInfo info = new ProcessStartInfo("cmd.exe");
                info.Arguments = command;
                info.RedirectStandardInput = true;
                info.UseShellExecute = false;
                info.WorkingDirectory = assetsDirectory + "zap";

                proc = Process.Start(info);
                proc.WaitForExit();
                return SortResultHTML();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Executing ZAP scan failed:\r {exception}");
                return null;
            }
        }

        public List<XElement> SortResultXML()
        {
            var report = assetsDirectory + @"scans\report.xml";
            RiskFound = new List<XElement>();
            try
            {
                XElement element = XElement.Load(report);
                foreach (XElement e in element.Descendants("riskdesc"))
                {
                    RiskFound.Add(e);
                }
                return RiskFound;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Failed sorting the results:\r {exception}");
                return null;
            }
        }

        private List<string> SortResultHTML()
        {
            List<string> results = new List<string>();
            var doc = new HtmlDocument();

            doc.Load(assetsDirectory + @"scans\report.html");
            var table = doc.DocumentNode.SelectNodes("//table[@class='summary']//tr");

            for (int i = 0; i < 8; i++)
            {
                results.Add(table[0].SelectNodes("//td")[i].InnerText);
                Console.WriteLine(table[0].SelectNodes("//td")[i].InnerText);
            }
            return results;
        }

        public void SetTarget(string target)
        {
            try
            {
                if (target.Substring(0, 4) != "http")
                {
                    Target = "https://" + target;
                }
                else Target = target;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Setting up target failed:\r {exception}");
            }
        }

        private string SetFileName(string format)
        {
            return FileDirectory = '"' + $@"{assetsDirectory}scans\report.{format}" + '"';
        }

        private void ClearLastSession()
        {
            Array.ForEach(Directory.GetFiles($@"{assetsDirectory}scans\"), delegate (string path) { File.Delete(path); });
        }
    }
}