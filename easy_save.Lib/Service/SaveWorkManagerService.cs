using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using easy_save.Lib.Models;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace easy_save.Lib.Service
{
    /// <summary>
    /// This class is used to manage every save works
    /// </summary>
    public class SaveWorkManagerService
    {
        /// <summary>
        /// This method is used to get all the save works
        /// </summary>
        /// <returns></returns>
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
                SaveWorkModel data = JsonSerializer.Deserialize<SaveWorkModel>(json);
                works[i] = data;
            }

            return works;
        }


        /// <summary>
        /// This method is used to create a save work
        /// </summary>
        /// <param name="saveWork"></param>
        /// <returns></returns>
        public bool AddSaveWork(SaveWorkModel saveWork)
        {
            if (!Directory.Exists($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}"))
            {
                Directory.CreateDirectory($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}");
            }

            if (GetSaveWorks().Any(x => x.Name == saveWork.Name))
            {
                return false;
            }

            else
            {
                string json = JsonSerializer.Serialize(saveWork);
                File.WriteAllText($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}{saveWork.Name}.json", json);
                return true;
            }

        }

        /// <summary>
        /// This method is used to delete a save work
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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
