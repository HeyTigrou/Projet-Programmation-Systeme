using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectSoftware
{

    public class FileExtensionModel
    {
        public ObservableCollection<string> CryptingExtensions { get; set; }

        public ObservableCollection<string> SelectedCryptingExtensions { get; set; }

        public ObservableCollection<string> PriorityExtensions { get; set; }

        public ObservableCollection<string> SelectedPriorityExtensions { get; set; }

        /// <summary>
        /// Creates a singleton of the model.
        /// </summary>
        private static FileExtensionModel extensionInstance;

        private static FileExtensionModel priorityInstance;

        private FileExtensionModel()
        {

        }

        /// <summary>
        /// This constructor adds all the existing extensions to the Extensions ObservableCollection, and initiates the selesctedExtensions ObservableCollection a empty.
        /// </summary>
        public static FileExtensionModel ExtensionInstance
        {
            get
            {
                if (extensionInstance == null)
                {
                    extensionInstance = new FileExtensionModel();
                    extensionInstance.CryptingExtensions = new ObservableCollection<string>()
                    {
                        ".txt",
                        ".doc",
                        ".docx",
                        ".pdf",
                        ".xls",
                        ".xlsx",
                        ".ppt",
                        ".pptx",
                        ".jpg",
                        ".jpeg",
                        ".png",
                        ".gif",
                        ".bmp",
                        ".mp3",
                        ".wav",
                        ".mp4",
                        ".avi",
                        ".wmv",
                        ".mov",
                        ".zip",
                        ".rar",
                        ".7z",
                        ".exe",
                        ".dll",
                        ".iso",
                        ".bat",
                        ".cmd"
                    };


                    extensionInstance.SelectedCryptingExtensions = new ObservableCollection<string>() { };
                }
                return extensionInstance;
            }
        }
        
        public static FileExtensionModel PriorityInstance
        {
            get
            {
                if (priorityInstance == null)
                {
                    priorityInstance = new FileExtensionModel();
                    priorityInstance.PriorityExtensions = new ObservableCollection<string>()
                    {
                        ".txt",
                        ".doc",
                        ".docx",
                        ".pdf",
                        ".xls",
                        ".xlsx",
                        ".ppt",
                        ".pptx",
                        ".jpg",
                        ".jpeg",
                        ".png",
                        ".gif",
                        ".bmp",
                        ".mp3",
                        ".wav",
                        ".mp4",
                        ".avi",
                        ".wmv",
                        ".mov",
                        ".zip",
                        ".rar",
                        ".7z",
                        ".exe",
                        ".dll",
                        ".iso",
                        ".bat",
                        ".cmd"
                    };


                    priorityInstance.SelectedPriorityExtensions = new ObservableCollection<string>() { };
                }
                return priorityInstance;
            }
        }
        
    }
}
