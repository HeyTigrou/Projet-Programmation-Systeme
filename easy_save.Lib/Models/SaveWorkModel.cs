﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Models
{
    public class SaveWorkModel
    {
        public string Name { get; set; }
        public string InputPath { get; set; }
        public string OutputPath { get; set; }
        public int SaveType { get; set; }
    }
}
