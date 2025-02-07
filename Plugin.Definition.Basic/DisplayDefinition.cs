using System.Runtime.InteropServices;
using Microsoft.Win32;
using Plugin.Core;

namespace Plugin.Definition
{
    public class SetDisplayAutoRotationDefinition : RegistrySettingDefinition
    {
        protected override string KeyName
        {
            get { return "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\AutoRotation"; }
        }

        protected override RegistryValueKind ValueKind
        {
            get { return RegistryValueKind.DWord; }
        }

        [Flags]
        public enum AR_STATE : int
        {
            AR_ENABLED = 0x0,//自動回転オン
            AR_DISABLED = 0x1,//自動回転オフ
            AR_SUPPRESSED = 0x2,//自動回転抑制状態
            AR_REMOTESESSION = 0x4,//リモートセッション
            AR_MULTIMON = 0x8,//マルチモニター
            AR_NOSENSOR = 0x10,//回転センサーなし。セット無効。
            AR_NOT_SUPPORTED = 0x20,//システムが回転非対応。セット無効。
            AR_DOCKED = 0x40,//ドッキング状態
            AR_LAPTOP = 0x80//ラップトップ
        };

        [DllImport("User32.dll")]
        private static extern bool GetAutoRotationState(out AR_STATE pState);
        //SetAutoRotationは非推奨なので、セットにはレジストリを使用。

        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "SetDisplayAutoRotation"; }
        }

        public override string[] SelectItem
        {
            get
            {
                return new string[]
                {
                    "true", "false"
                };
            }
        }

        public override string Description
        {
            get { return "ディスプレイ自動回転有効無効"; }
        }

        public override void ApplyValue(string value)
        {
            AR_STATE autoRotationState = 0;
            bool success = GetAutoRotationState(out autoRotationState);
            if (!success)
            {
                return;
            }

            if (autoRotationState == AR_STATE.AR_ENABLED || autoRotationState == AR_STATE.AR_DISABLED)
            {
                //変更処理
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

                string valueName = "Enable";
                Registry.SetValue(KeyName, valueName, setValue, ValueKind);
            }
            else 
            {
                //自動回転非対応なので変更しない
                return;
            }
        }

        public override string GetValue()
        {
            AR_STATE autoRotationState = 0;
            bool success = GetAutoRotationState(out autoRotationState);
            if (!success)
            {
                return "not get value";
            }

            string state = "not supported";
            switch (autoRotationState) 
            {
                case AR_STATE.AR_ENABLED:
                    state = "true";
                    break;
                case AR_STATE.AR_DISABLED:
                    state = "false";
                    break;
                case AR_STATE.AR_SUPPRESSED:
                    state = "suppressed";
                    break;
                case AR_STATE.AR_REMOTESESSION:
                    state = "remote session";
                    break;
                case AR_STATE.AR_MULTIMON:
                    state = "multi monitor";
                    break;
                case AR_STATE.AR_NOSENSOR:
                    state = "non sensor";
                    break;
                case AR_STATE.AR_NOT_SUPPORTED:
                    state = "not supported";
                    break;
                case AR_STATE.AR_DOCKED:
                    state = "docked";
                    break;
                case AR_STATE.AR_LAPTOP:
                    state = "laptop";
                    break;
            }
            return state;
        }
    }
}
