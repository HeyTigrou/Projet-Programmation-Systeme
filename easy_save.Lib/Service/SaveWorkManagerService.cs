using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using easy_save.Lib.Models;

namespace easy_save.Lib.Service
{
    public class SaveWorkManagerService
    {
        public static int GetSaveProjectnumber()
        {
            string[] saveWorks = Directory.GetFiles(@"..\..\..\..\easy_save.Lib\ConfigurationFiles\SaveProjects\", "*.json", SearchOption.AllDirectories);
            return saveWorks.Length;
        }

        public static SaveWorkModel[] GetSaveWorks()
        {
            string[] saveWorks = Directory.GetFiles(@"..\..\..\..\easy_save.Lib\ConfigurationFiles\SaveProjects\", "*.json", SearchOption.AllDirectories);
            SaveWorkModel[] works = new SaveWorkModel[saveWorks.Length];
            int i = 0;
            foreach (string saveWork in saveWorks)
            {
                string json = File.ReadAllText(saveWork);
                SaveWorkModel data = JsonConvert.DeserializeObject<SaveWorkModel>(json);
                works[i] = data;
                i++;
            }
            return works;
        }

        public static bool AddSaveWork(SaveWorkModel saveWork)
        {
            if (GetSaveProjectnumber() >= 5)
            {
                return false;
            }
            else
            {
                string json = JsonConvert.SerializeObject(saveWork);
                File.WriteAllText($@"..\..\..\..\easy_save.Lib\ConfigurationFiles\SaveProjects\{saveWork.Name}.json", json);
                return true;
            }

        }
        
        public static bool DeleteSaveWork(string name)
        {
            string path = $@"..\..\..\..\easy_save.Lib\ConfigurationFiles\SaveProjects\{name}.json";
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
