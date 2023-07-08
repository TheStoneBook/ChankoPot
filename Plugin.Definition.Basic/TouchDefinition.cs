using Microsoft.Win32;
using Plugin.Core;

namespace Plugin.Definition
{
    public class AllowEdgeSwipeRegistrySettingDefinition : RegistrySettingDefinition
    {
        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "EnabledEdgeSwipe"; }
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
                    setValue = "0";
                    break;
                case "true":
                    setValue = "1";
                    break;
            }

            string valueName = "AllowEdgeSwipe";
            Registry.SetValue(KeyName, valueName, setValue, ValueKind);
        }

        public override string GetValue()
        {
            string getValue = "";

            string valueName = "AllowEdgeSwipe";
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
            get { return "エッジスワイプの有効無効"; }
        }
    }

    public class ContactVisualizationRegistryValue : RegistrySettingDefinition
    {
        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "EnabledTouchEffect"; }
        }

        protected override string KeyName
        {
            get { return "HKEY_CURRENT_USER\\Control Panel\\Cursors"; }
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
            string value_ContactVisualization = "";
            string value_GestureVisualization = "";

            switch (value)
            {
                case "false":
                    value_ContactVisualization = "0";
                    value_GestureVisualization = "0";
                    break;
                case "true":
                    value_ContactVisualization = "1";
                    value_GestureVisualization = "31";
                    break;
            }

            string contactVisualization = "ContactVisualization";
            Registry.SetValue(KeyName, contactVisualization, value_ContactVisualization, ValueKind);

            string gestureVisualization = "GestureVisualization";
            Registry.SetValue(KeyName, gestureVisualization, value_GestureVisualization, ValueKind);
        }

        public override string GetValue()
        {
            string getValue = "";

            string contactVisualization = "ContactVisualization";
            var value_ContactVisualization = Registry.GetValue(KeyName, contactVisualization, null);

            string gestureVisualization = "GestureVisualization";
            var value_GestureVisualization = Registry.GetValue(KeyName, gestureVisualization, null);

            if (value_ContactVisualization == null || value_GestureVisualization == null)  
            {
                return "not set";
            }

            switch (value_ContactVisualization)
            { 
                case 0:
                    if ((int)value_GestureVisualization == 0) 
                    {
                        getValue = "false";
                    }
                    else
                    {
                        getValue = "not set";
                    }
                    break;
                case 1:
                    if ((int)value_GestureVisualization == 31)
                    {
                        getValue = "true";
                    }
                    else
                    {
                        getValue = "not set";
                    }
                    break;
                case 2:
                    if ((int)value_GestureVisualization == 31)
                    {
                        getValue = "true";
                    }
                    else
                    {
                        getValue = "not set";
                    }
                    break;
            }
            
            return getValue;
        }

        public override string Description
        {
            get { return "画面タッチエフェクトの有効無効"; }
        }
    }
}
