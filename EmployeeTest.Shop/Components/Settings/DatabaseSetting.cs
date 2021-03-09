using EmployeeTest.Shop.Components.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeTest.Shop.Components.Settings
{
    [Setting("Database")]
    public class DatabaseSetting
    {
        public string Connection { get; set; }
        public string Version { get; set; }
        public bool UseMariaDb { get; set; }
        public bool MigrationService { get; set; }
        public bool Resilent { get; set; }
    }
}
