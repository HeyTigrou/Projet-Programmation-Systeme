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

            // Gets an array of file paths with a json extension.
            string[] saveWorks = Directory.GetFiles($@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}", "*.json", SearchOption.AllDirectories);

            // Creates an array to store all the saveWorks.
            SaveWorkModel[] works = new SaveWorkModel[saveWorks.Length];


            // Adds save work to the works array.
            for (int i = 0; i < saveWorks.Length; i++)
            {
                string json = File.ReadAllText(saveWorks[i]);
                SaveWorkModel data = JsonConvert.DeserializeObject<SaveWorkModel>(json);
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

            // If the save work already exists, returns false.
            if (GetSaveWorks().Any(x => x.Name == saveWork.Name))
            {
                return false;
            }

            else
            {
                // Creates a file with the save work details.
                string json = JsonConvert.SerializeObject(saveWork);
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
            // Creates file path with the file name.
            string path = $@"{ConfigurationManager.AppSettings["SaveProjectEmplacement"]}{name}.json";
            try
            {
                // Deletes file.
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
