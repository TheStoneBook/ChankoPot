using Microsoft.Win32;
using Plugin.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Definition
{
    public class ThreeAndFourFingerTouchGestureEnabledRegistrySettingDefinition : RegistrySettingDefinition 
    {
        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "ThreeAndFourFingerTouchGestureEnabled"; }
        }

        protected override string KeyName
        {
            get { return "HKEY_CURRENT_USER\\Control Panel\\Desktop"; }
        }

        protected override RegistryValueKind ValueKind
        {
            get { return RegistryValueKind.DWord; }
        }

        public override string[] SelectItem
        {
            get
            {
                return new string[] { "true", "false" };
            }
        }

        public override void ApplyValue(string value)
        {
            string setValue = "";

            switch (value)
            {
                case "true":
                    setValue = "1";
                    break;
                case "false":
                    setValue = "0";
                    break;
            }

            string valueName = "TouchGestureSetting";
            Registry.SetValue(KeyName, valueName, setValue, ValueKind);
        }

        public override string GetValue()
        {
            string getValue = "";

            string valueName = "TouchGestureSetting";
            var value = Registry.GetValue(KeyName, valueName, null);

            switch (value)
            {
                case null:
                    getValue = "not set";
                    break;
                case 0:
                    getValue = "false";
                    break;
                case 1:
                    getValue = "true";
                    break;
            }

            return getValue;
        }

        public override string Description
        {
            get { return "3本指と4本指タッチジェスチャ有効無効"; }
        }
    }
}
