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
        public int GetSaveProjectnumber()
        {
            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}");
            }
            
            string[] saveWorks = Directory.GetFiles($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}", "*.json", SearchOption.AllDirectories);
            
            return saveWorks.Length;
        }

        // This method is used to get all the save works
        public SaveWorkModel[] GetSaveWorks()
        {
            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}");
            }
            string[] saveWorks = Directory.GetFiles($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}", "*.json", SearchOption.AllDirectories);
            SaveWorkModel[] works = new SaveWorkModel[saveWorks.Length];

            for (int i = 0; i < saveWorks.Length; i++)
            {
                string json = File.ReadAllText(saveWorks[i]);
                SaveWorkModel data = JsonConvert.DeserializeObject<SaveWorkModel>(json);
                works[i] = data;
            }

            return works;
        }
        
        // This method is used to create a save work
        public bool AddSaveWork(SaveWorkModel saveWork)
        {
            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}");
            }
            
            if (GetSaveProjectnumber() >= Int32.Parse(ConfigurationManager.AppSettings["MaxNumberOfSave"]))
            {
                return false;
            }

            else if (GetSaveWorks().Any(x => x.Name == saveWork.Name))
            {
                return false;
            }

            else
            {
                string json = JsonConvert.SerializeObject(saveWork);
                File.WriteAllText($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}{saveWork.Name}.json", json);
                return true;
            }

        }

        // This method is used to delete a save work
        public bool DeleteSaveWork(string name)
        {
            string path = $@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}{name}.json";
            try
            {
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
