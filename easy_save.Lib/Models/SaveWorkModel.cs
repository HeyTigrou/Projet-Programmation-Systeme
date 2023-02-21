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

namespace easy_save.Lib.Models
{
    public enum SaveWorkState
    {
        Unstarted,
        Running,
        Done,
        Paused
    }
    /// <summary>
    /// This Model is used to manage the save works
    /// </summary>
    [DataContract]
    public class SaveWorkModel : INotifyPropertyChanged
    {
        [DataMember] public string Name { get; set; }
        [DataMember] public string InputPath { get; set; }
        [DataMember] public string OutputPath { get; set; }
        [DataMember] public int SaveType { get; set; }
        private string progression;
        public string Progression { 
            get { return (progression != null) ? progression : "" ; } 
            set { progression = value; OnPropertyChanged("Progression"); } 
        }

        private string state;
        public string State
        {
            get { return (state != null) ? state : nameof(SaveWorkState.Unstarted); }
            set { state = value; OnPropertyChanged("State"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
