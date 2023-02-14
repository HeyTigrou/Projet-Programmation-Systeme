using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using easy_save.Lib.Models;
using System.Configuration;
using System.Collections.ObjectModel;

namespace easy_save.Lib.Service
{
    // This class is used to manage every save works
    public class SaveWorkManagerService
    {
        // This method is used to get all the save works
        public void GetSaveWorks(ObservableCollection<SaveWorkModel> Processes)
        {
            Processes.Clear();

            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}");
            }

            string[] saveWorks = Directory.GetFiles($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}", "*.json", SearchOption.AllDirectories);

            for (int i = 0; i < saveWorks.Length; i++)
            {
                string json = File.ReadAllText(saveWorks[i]);
                SaveWorkModel data = JsonConvert.DeserializeObject<SaveWorkModel>(json);
                Processes.Add(data);
            }
        }


        // This method is used to create a save work
        public bool AddSaveWork(SaveWorkModel saveWork, ObservableCollection<SaveWorkModel> Processes)
        {
            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}");
            }

            if (Processes.Any(x => x.Name == saveWork.Name))
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
