using easy_save.Cmd.ConfigurationFiles.Interface_text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Service
{
    public class ChoosenLanguage
    {
        public string AllProcess_Continue = "";
        public string AllProcess_EndProcess = "";
        public string AllProcess_Error = "";
        public string AllProcess_StartProcess = "";
        public string BackupDetails_Continue = "";
        public string Main_Menu = "";
        public string Main_Option = "";
        public string Remove_Continue = "";
        public string Remove_MissingFile = "";
        public string Remove_Name = "";
        public string Remove_SucessRemove = "";
        public string Save_Continue = "";
        public string Save_ErroLimit = "";
        public string Save_ErrorInput = "";
        public string Save_ErrorSaveType = "";
        public string Save_Name = "";
        public string Save_PathDestination = "";
        public string Save_PathSave = "";
        public string Save_Process = "";
        public string Save_SaveType = "";
        public string StartSave_Continue = "";
        public string StartSave_Error = "";
        public string StartSave_Name = "";
        public string StartSave_StartProcess = "";

        public void GetLanguage(string language)
        {
            switch(language)
            {
                case "en":
                    {
                        english();
                        break;
                    }
                case "fr":
                    {
                        french();
                        break; }
            }
        }
        private void french()
        {
            AllProcess_Continue = fr.AllProcess_Continue;
            AllProcess_Continue = fr.AllProcess_Continue;
            AllProcess_EndProcess = fr.AllProcess_EndProcess;
            AllProcess_Error = fr.AllProcess_Error;
            AllProcess_StartProcess = fr.AllProcess_StartProcess;
            BackupDetails_Continue = fr.BackupDetails_Continue;
            Main_Menu = fr.Main_Menu;
            Main_Option = fr.Main_Option;
            Remove_Continue = fr.Remove_Continue;
            Remove_MissingFile = fr.Remove_MissingFile;
            Remove_Name = fr.Remove_Name;
            Remove_SucessRemove = fr.Remove_SucessRemove;
            Save_Continue = fr.Save_Continue;
            Save_ErroLimit = fr.Save_ErroLimit;
            Save_ErrorInput = fr.Save_ErrorInput;
            Save_ErrorSaveType = fr.Save_ErrorSaveType;
            Save_Name = fr.Save_Name;
            Save_PathDestination = fr.Save_PathDestination;
            Save_PathSave = fr.Save_PathSave;
            Save_Process = fr.Save_Process;
            Save_SaveType = fr.Save_SaveType;
            StartSave_Continue = fr.StartSave_Continue;
            StartSave_Error = fr.StartSave_Error;
            StartSave_Name = fr.StartSave_Name;
            StartSave_StartProcess = fr.StartSave_StartProcess;
        }
        private void english()
        {
            AllProcess_Continue = en.AllProcess_Continue; 
            AllProcess_Continue = en.AllProcess_Continue;
            AllProcess_EndProcess = en.AllProcess_EndProcess;
            AllProcess_Error = en.AllProcess_Error;
            AllProcess_StartProcess = en.AllProcess_StartProcess;
            BackupDetails_Continue = en.BackupDetails_Continue;
            Main_Menu = en.Main_Menu;
            Main_Option = en.Main_Option;
            Remove_Continue = en.Remove_Continue;
            Remove_MissingFile = en.Remove_MissingFile;
            Remove_Name = en.Remove_Name;
            Remove_SucessRemove = en.Remove_SucessRemove;
            Save_Continue = en.Save_Continue;
            Save_ErroLimit = en.Save_ErroLimit;
            Save_ErrorInput = en.Save_ErrorInput;
            Save_ErrorSaveType = en.Save_ErrorSaveType;
            Save_Name = en.Save_Name;
            Save_PathDestination = en.Save_PathDestination;
            Save_PathSave = en.Save_PathSave;
            Save_Process = en.Save_Process;
            Save_SaveType = en.Save_SaveType;
            StartSave_Continue = en.StartSave_Continue;
            StartSave_Error = en.StartSave_Error;
            StartSave_Name = en.StartSave_Name;
            StartSave_StartProcess = en.StartSave_StartProcess;
    }
    }
}
