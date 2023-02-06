using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace easy_save.Lib.Service
{
    public class JsonService
    {
        public class Data
        {
            public string name { get; set; }
            public string source { get; set; }
        }

        public static void launch()
        {
            Data data = new Data
            {
                name = "test",
                source = "plop"
            };

            Data data2 = new Data
            {
                name = "test",
                source = "plop"
            };

            string json = JsonConvert.SerializeObject(data);
            json += ","+JsonConvert.SerializeObject(data2);

            Console.WriteLine(Directory.GetFiles(@"..\..\..\..\easy_save.Lib\ConfigurationFiles\SaveProjects\", "*.json", SearchOption.AllDirectories).Length);
            Console.ReadLine();
            //File.WriteAllText(@"..\..\..\..\easy_save.Lib\ConfigurationFiles\SaveProjects\SaveWorks.json", json);
        }

    }
}
