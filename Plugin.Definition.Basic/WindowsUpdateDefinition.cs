using Microsoft.Win32;
using Plugin.Core;

namespace Plugin.Definition
{
    public class NoAutoUpdateSettingDefinition : RegistrySettingDefinition
    {
        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "EnabledWindowsAutoUpdate"; }
        }

        protected override string KeyName
        {
            get { return "HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\\AU"; }
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
                case "false":
                    setValue = "1";
                    break;
                case "true":
                    setValue = "0";
                    break;
            }
            
            string valueName = "NoAutoUpdate";//本来はtrueでWindows自動更新をしない。falseでWindows自動更新を行う。
            Registry.SetValue(KeyName, valueName, setValue, ValueKind);
        }

        public override string GetValue()
        {
            string getValue = "";
            
            string valueName = "NoAutoUpdate";
            var value = Registry.GetValue(KeyName, valueName, null);

            switch (value)
            {
                case null:
                    getValue = "not set";
                    break;
                case 1:
                    getValue = "false";
                    break;
                case 0:
                    getValue = "true";
                    break;
            }

            return getValue;
        }

        public override string Description
        {
            get { return "Windows自動更新の有効無効"; }
        }
    }
}
