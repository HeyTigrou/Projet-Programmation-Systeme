using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Service
{
    internal class CryptService
    {
        public int Crypt(string inputPath, string outputPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = $@"{ConfigurationManager.AppSettings["CryptosoftPath"]}";
            startInfo.Arguments = $@"""{inputPath}"" ""{outputPath}"" ""{ConfigurationManager.AppSettings["CryptKeyPath"]}""";
            startInfo.UseShellExecute = false;

            try
            {
                int code = 0;
                using (Process process = new Process())
                {
                    process.StartInfo = startInfo;

                    process.Start();

                    process.WaitForExit();
                    code = process.ExitCode;
                }
                return code;
            }
            catch
            {
                return -1;
            }
        }
        
        public bool Generate ()
        {
            var random = new Random();
            ulong key = (ulong)random.Next() << 32 | (ulong)random.Next();
            string keyPath = $@"{ConfigurationManager.AppSettings["CryptKeyPath"]}";
            try
            {
                using (StreamWriter sw = new StreamWriter(keyPath))
                {
                    sw.WriteLine(key.ToString());
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
