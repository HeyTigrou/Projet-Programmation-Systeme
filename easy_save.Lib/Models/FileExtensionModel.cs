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
        public ObservableCollection<string> Extensions { get; set; }

        public ObservableCollection<string> SelectedExtensions { get; set; }

        private static FileExtensionModel instance;

        private FileExtensionModel()
        {

        }

        public static FileExtensionModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FileExtensionModel();
                    instance.Extensions = new ObservableCollection<string>()
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
        instance.SelectedExtensions = new ObservableCollection<string>() { };
                }
                return instance;
            }
        }
    }
}
