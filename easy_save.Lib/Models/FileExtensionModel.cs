using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DetectSoftware
{

    public class FileExtensionModel
    {
        public List<string> Extensions { get; set; }

        public List<string> SelectedExtensions { get; set; }

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
                    instance.Extensions = new List<string> {
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
                    instance.SelectedExtensions = new List<string> { };
                }
                return instance;
            }
        }
    }
}
