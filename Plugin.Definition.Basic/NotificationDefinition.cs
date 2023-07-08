using Microsoft.Win32;
using Plugin.Core;

namespace Plugin.Definition
{
    public class ToastEnabledRegistrySettingDefinition : RegistrySettingDefinition
    {
        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "EnabledNotifications"; }
        }

        protected override string KeyName
        {
            get { return "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\PushNotifications"; }
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

            string valueName = "ToastEnabled";
            Registry.SetValue(KeyName, valueName, setValue, ValueKind);
        }

        public override string GetValue()
        {
            string getValue = "";

            string valueName = "ToastEnabled";
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
            get { return "通知の有効無効"; }
        }
    }

    /*
    public class DisableHelpStickerRegistrySettingDefinition : RegistrySettingDefinition
    {
        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "EnabledShowHelpHint"; }
        }

        protected override string KeyName
        {
            get { return "HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\EdgeUI"; }
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

            string valueName = "DisableHelpSticker";//本来はtrueでヘルプヒント表示しない。falseで表示する。
            Registry.SetValue(KeyName, valueName, setValue, ValueKind);
        }

        public override string GetValue()
        {
            string getValue = "";

            string valueName = "DisableHelpSticker";
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
            get { return "ヘルプヒント表示の有効無効"; }
        }
    }
    */
}
