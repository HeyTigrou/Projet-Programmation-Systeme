using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using easy_save.Lib.Models;
using System.Configuration;

namespace easy_save.Lib.Service
{
    // This class is used to manage every save works
    public class SaveWorkManagerService
    {
        
        // This method is used to get the number of save works available
        public static int GetSaveProjectnumber()
        {
            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}");
            }
            string[] saveWorks = Directory.GetFiles($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}", "*.json", SearchOption.AllDirectories);
            return saveWorks.Length;
        }

        // This method is used to get all the save works
        public static SaveWorkModel[] GetSaveWorks()
        {
            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}");
            }
            string[] saveWorks = Directory.GetFiles($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}", "*.json", SearchOption.AllDirectories);
            SaveWorkModel[] works = new SaveWorkModel[saveWorks.Length];
            int i = 0;
            // We get all the save works and we add them to the save work list
            foreach (string saveWork in saveWorks)
            {
                string json = File.ReadAllText(saveWork);
                SaveWorkModel data = JsonConvert.DeserializeObject<SaveWorkModel>(json);
                works[i] = data;
                i++;
            }
            return works;
        }
        // This method is used to create a save work
        public static bool AddSaveWork(SaveWorkModel saveWork)
        {
            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}");
            }
            //var config = JsonConvert.DeserializeObject<ConfigFileModel>(File.ReadAllText(@"..\..\..\..\easy_save.Cmd\ConfigurationFiles\easy_save_config.json"));
            // We check if the number of save works is not superior to the maximum number of save works
            //config.Max_number_of_save
            if (GetSaveProjectnumber() >= Int32.Parse(ConfigurationManager.AppSettings["MaxNumberOfSave"]))
            {
                return false;
            }
            //  We check if the name of the save work is already used
            else if (GetSaveWorks().Any(x => x.Name == saveWork.Name))
            {
                return false;
            }
            //  We create the save work
            else
            {
                string json = JsonConvert.SerializeObject(saveWork);
                File.WriteAllText($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}{saveWork.Name}.json", json);
                return true;
            }

        }

        // This method is used to delete a save work
        public static bool DeleteSaveWork(string name)
        {
            /*
            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}");
            }
            */
            string path = $@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}{name}.json";
            try
            {
                // We delete the save work
                File.Delete(path);
                return true;
            }
            // If an exception is raised when we try to delete the save work, we return false
            catch (Exception)
            {
                return false;
            }
        }
    }
}
