using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Service
{
    public class CryptService
    {
        /// <summary>
        /// Crypts the given file and saves the crypted file in the given output path.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        public int Crypt(string inputPath, string outputPath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = $@"{ConfigurationManager.AppSettings["CryptosoftPath"]}";
            startInfo.Arguments = $@"""{inputPath}"" ""{outputPath}"" ""{ConfigurationManager.AppSettings["CryptKeyPath"]}""";
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow= true;

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
        
        /// <summary>
        /// Generates a crypt key.
        /// </summary>
        /// <returns></returns>
        public bool Generate ()
        {
            // Chosses a random 64 bit number.
            var random = new Random();
            ulong key = (ulong)random.Next() << 32 | (ulong)random.Next();
            string keyPath = $@"{ConfigurationManager.AppSettings["CryptKeyPath"]}";
            try
            {
                // Writes it to the key file.
                using (StreamWriter sw = new StreamWriter(keyPath))
                {
                    sw.WriteLine(key.ToString());
                }
                return true;
            }
            catch 
            {
                return false;
            }
        }
    }
}
