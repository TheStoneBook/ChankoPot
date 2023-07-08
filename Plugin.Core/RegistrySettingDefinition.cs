using Microsoft.Win32;

namespace Plugin.Core
{
    public abstract class RegistrySettingDefinition : SettingDefinition
    {
        protected abstract string KeyName { get; }

        protected abstract RegistryValueKind ValueKind { get; }

        public override void ApplyValue(string value)
        {
            Registry.SetValue(KeyName, Name, value, ValueKind);
        }

        public override string GetValue()
        {
            var value = Registry.GetValue(KeyName, Name, null);
            if (value == null)
            {
                return "null";
            }

            string valueStr = value.ToString() ?? "null";

            return valueStr;
        }
    }
}
