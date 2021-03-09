using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeTest.Shop.Components.Attributes
{
    public class SettingAttribute : Attribute
    {
        public SettingAttribute(string keyConfig)
        {
            KeyConfig = keyConfig ?? throw new ArgumentNullException(nameof(keyConfig));
        }

        public string KeyConfig { get; }
    }
}
