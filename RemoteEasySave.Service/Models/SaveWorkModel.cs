using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RemoteEasySave.Lib.Models
{
    public enum SaveWorkState
    {
        Unstarted,
        Running,
        Done,
        Paused,
        Paused_ApplicationIsRunning
    }
    /// <summary>
    /// This Model is used to manage the save works
    /// </summary>
    public class SaveWorkModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public int SaveType { get; set; }


        public string Progression { 
            get { return (progression != null) ? progression : "" ; } 
            set { progression = value; OnPropertyChanged("Progression"); } 
        }

        public string State
        {
            get { return (state != null) ? state : nameof(SaveWorkState.Unstarted); }
            set { state = value; OnPropertyChanged("State"); }
        }

        private string progression;
        private string state;

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
