using Microsoft.Win32;
using Plugin.Core;
using System.Runtime.InteropServices;

namespace Plugin.Definition
{
    public class SetStandbyTimeoutACDefinition : SettingDefinition
    {
        /// <summary>
        /// アクティブな電源プランを取得
        /// </summary>
        /// <param name="UserRootPowerKey"></param>
        /// <param name="ActivePolicyGuid"></param>
        /// <returns></returns>
        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr ActivePolicyGuid);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerReadACValue(IntPtr RootPowerKey, Guid SchemeGuid, Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref int Type, IntPtr Buffer, ref uint BufferSize);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerWriteACValueIndex(IntPtr RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, uint AcValueIndex);

        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "SetPCStandbyTimeoutAC"; }
        }

        public override string[] SelectItem
        {
            get
            {
                return new string[]
                {
                    "sleepOFF", "10分","30分","1時間"
                };
            }
        }

        public override string Description
        {
            get { return "PCスリープまでの時間（電源接続時）"; }
        }

        public override void ApplyValue(string value)
        {
            uint setValue = 0;

            switch (value)
            {
                case "sleepOFF":
                    setValue = 0;
                    break;
                case "10分":
                    setValue = 10 * 60;
                    break;
                case "30分":
                    setValue = 30 * 60;
                    break;
                case "1時間":
                    setValue = 60 * 60;
                    break;
            }

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings
            Guid guidSleepSubgroup = new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");
            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings-sleep-idle-timeout
            Guid guidSleepTimeSetting = new Guid("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");
            Guid activeGuid;
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return;
            }
            activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            PowerWriteACValueIndex(IntPtr.Zero, ref activeGuid, ref guidSleepSubgroup, ref guidSleepTimeSetting, setValue);
        }

        public override string GetValue()
        {
            //アクティブな電源スキームguid取得
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return "ErrorCode:" + result.ToString();
            }
            //この値も表示させたい
            Guid activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings
            Guid guidSleepSubgroup = new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings-sleep-idle-timeout
            Guid guidSleepTimeSetting = new Guid("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");

            //階層的にはguidSleepSubgroup/guidSleepTimeSetting....guidSleepTimeSetting以外にもいくつかある

            //現在の電源のスリープ時間取得
            int registryValueKind = (int)RegistryValueKind.DWord;
            uint size = (uint)Marshal.SizeOf(typeof(int));
            //バッファにNullを渡してsizeに適切なバッファサイズ取得
            PowerReadACValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, IntPtr.Zero, ref size);
            
            IntPtr ptrValue = Marshal.AllocHGlobal((int)size);
            PowerReadACValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, ptrValue, ref size);
            int value = Marshal.ReadInt32(ptrValue);
            //メモリ開放
            Marshal.FreeHGlobal(ptrValue);

            if (value == 0)
            {
                return "sleepOFF";
            }
            return value.ToString() + "秒";
        }
    }


    public class SetStandbyTimeoutDCDefinition : SettingDefinition
    {
        /// <summary>
        /// アクティブな電源プランを取得
        /// </summary>
        /// <param name="UserRootPowerKey"></param>
        /// <param name="ActivePolicyGuid"></param>
        /// <returns></returns>
        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr ActivePolicyGuid);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerReadDCValue(IntPtr RootPowerKey, Guid SchemeGuid, Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref int Type, IntPtr Buffer, ref uint BufferSize);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerWriteDCValueIndex(IntPtr RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, uint AcValueIndex);

        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "SetPCStandbyTimeoutDC"; }
        }

        public override string[] SelectItem
        {
            get
            {
                return new string[]
                {
                    "sleepOFF", "10分","30分","1時間"
                };
            }
        }

        public override string Description
        {
            get { return "PCスリープまでの時間（バッテリ駆動時）"; }
        }

        public override void ApplyValue(string value)
        {
            uint setValue = 0;

            switch (value)
            {
                case "sleepOFF":
                    setValue = 0;
                    break;
                case "10分":
                    setValue = 10 * 60;
                    break;
                case "30分":
                    setValue = 30 * 60;
                    break;
                case "1時間":
                    setValue = 60 * 60;
                    break;
            }

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings
            Guid guidSleepSubgroup = new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");
            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings-sleep-idle-timeout
            Guid guidSleepTimeSetting = new Guid("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");
            Guid activeGuid;
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return;
            }
            activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            PowerWriteDCValueIndex(IntPtr.Zero, ref activeGuid, ref guidSleepSubgroup, ref guidSleepTimeSetting, setValue);
        }

        public override string GetValue()
        {
            //アクティブな電源スキームguid取得
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return "ErrorCode:" + result.ToString();
            }
            //この値も表示させたい
            Guid activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings
            Guid guidSleepSubgroup = new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings-sleep-idle-timeout
            Guid guidSleepTimeSetting = new Guid("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");

            //階層的にはguidSleepSubgroup/guidSleepTimeSetting....guidSleepTimeSetting以外にもいくつかある

            //現在の電源のスリープ時間取得
            int registryValueKind = (int)RegistryValueKind.DWord;
            uint size = (uint)Marshal.SizeOf(typeof(int));
            //バッファにNullを渡してsizeに適切なバッファサイズ取得
            PowerReadDCValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, IntPtr.Zero, ref size);

            IntPtr ptrValue = Marshal.AllocHGlobal((int)size);
            PowerReadDCValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, ptrValue, ref size);
            int value = Marshal.ReadInt32(ptrValue);
            //メモリ開放
            Marshal.FreeHGlobal(ptrValue);

            if (value == 0)
            {
                return "sleepOFF";
            }
            return value.ToString() + "秒";
        }
    }


    public class SetUnattendTimeoutDCDefinition : SettingDefinition
    {
        /// <summary>
        /// アクティブな電源プランを取得
        /// </summary>
        /// <param name="UserRootPowerKey"></param>
        /// <param name="ActivePolicyGuid"></param>
        /// <returns></returns>
        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr ActivePolicyGuid);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerReadDCValue(IntPtr RootPowerKey, Guid SchemeGuid, Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref int Type, IntPtr Buffer, ref uint BufferSize);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerWriteDCValueIndex(IntPtr RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, uint AcValueIndex);

        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "SetPCUnattendTimeoutDC"; }
        }

        public override string[] SelectItem
        {
            get
            {
                return new string[]
                {
                    "sleepOFF", "10分","30分","1時間"
                };
            }
        }

        public override string Description
        {
            get { return "無人スリープまでの時間（バッテリー駆動時）"; }
        }

        public override void ApplyValue(string value)
        {
            uint setValue = 0;

            switch (value)
            {
                case "sleepOFF":
                    setValue = 0;
                    break;
                case "10分":
                    setValue = 10 * 60;
                    break;
                case "30分":
                    setValue = 30 * 60;
                    break;
                case "1時間":
                    setValue = 60 * 60;
                    break;
            }

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings
            Guid guidSleepSubgroup = new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");
            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings-sleep-unattended-idle-timeout
            Guid guidSleepTimeSetting = new Guid("7bc4a2f9-d8fc-4469-b07b-33eb785aaca0");
            Guid activeGuid;
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return;
            }
            activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            PowerWriteDCValueIndex(IntPtr.Zero, ref activeGuid, ref guidSleepSubgroup, ref guidSleepTimeSetting, setValue);
        }

        public override string GetValue()
        {
            //アクティブな電源スキームguid取得
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return "ErrorCode:" + result.ToString();
            }
            //この値も表示させたい
            Guid activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings
            Guid guidSleepSubgroup = new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings-sleep-idle-timeout
            Guid guidSleepTimeSetting = new Guid("7bc4a2f9-d8fc-4469-b07b-33eb785aaca0");

            //階層的にはguidSleepSubgroup/guidSleepTimeSetting....guidSleepTimeSetting以外にもいくつかある

            //現在の電源のスリープ時間取得
            int registryValueKind = (int)RegistryValueKind.DWord;
            uint size = (uint)Marshal.SizeOf(typeof(int));
            //バッファにNullを渡してsizeに適切なバッファサイズ取得
            PowerReadDCValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, IntPtr.Zero, ref size);

            IntPtr ptrValue = Marshal.AllocHGlobal((int)size);
            PowerReadDCValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, ptrValue, ref size);
            int value = Marshal.ReadInt32(ptrValue);
            //メモリ開放
            Marshal.FreeHGlobal(ptrValue);

            if (value == 0) 
            {
                return "sleepOFF";
            }
            return value.ToString() + "秒";
        }
    }


    public class SetUnattendTimeoutACDefinition : SettingDefinition
    {
        /// <summary>
        /// アクティブな電源プランを取得
        /// </summary>
        /// <param name="UserRootPowerKey"></param>
        /// <param name="ActivePolicyGuid"></param>
        /// <returns></returns>
        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr ActivePolicyGuid);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerReadACValue(IntPtr RootPowerKey, Guid SchemeGuid, Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref int Type, IntPtr Buffer, ref uint BufferSize);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerWriteACValueIndex(IntPtr RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, uint AcValueIndex);

        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "SetPCUnattendTimeoutAC"; }
        }

        public override string[] SelectItem
        {
            get
            {
                return new string[]
                {
                    "sleepOFF", "10分","30分","1時間"
                };
            }
        }

        public override string Description
        {
            get { return "無人スリープまでの時間（電源接続時）"; }
        }

        public override void ApplyValue(string value)
        {
            uint setValue = 0;

            switch (value)
            {
                case "sleepOFF":
                    setValue = 0;
                    break;
                case "10分":
                    setValue = 10 * 60;
                    break;
                case "30分":
                    setValue = 30 * 60;
                    break;
                case "1時間":
                    setValue = 60 * 60;
                    break;
            }

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings
            Guid guidSleepSubgroup = new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");
            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings-sleep-unattended-idle-timeout
            Guid guidSleepTimeSetting = new Guid("7bc4a2f9-d8fc-4469-b07b-33eb785aaca0");
            Guid activeGuid;
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return;
            }
            activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            PowerWriteACValueIndex(IntPtr.Zero, ref activeGuid, ref guidSleepSubgroup, ref guidSleepTimeSetting, setValue);
        }

        public override string GetValue()
        {
            //アクティブな電源スキームguid取得
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return "ErrorCode:" + result.ToString();
            }
            //この値も表示させたい
            Guid activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings
            Guid guidSleepSubgroup = new Guid("238C9FA8-0AAD-41ED-83F4-97BE242C8F20");

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/sleep-settings-sleep-idle-timeout
            Guid guidSleepTimeSetting = new Guid("7bc4a2f9-d8fc-4469-b07b-33eb785aaca0");

            //階層的にはguidSleepSubgroup/guidSleepTimeSetting....guidSleepTimeSetting以外にもいくつかある

            //現在の電源のスリープ時間取得
            int registryValueKind = (int)RegistryValueKind.DWord;
            uint size = (uint)Marshal.SizeOf(typeof(int));
            //バッファにNullを渡してsizeに適切なバッファサイズ取得
            PowerReadACValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, IntPtr.Zero, ref size);

            IntPtr ptrValue = Marshal.AllocHGlobal((int)size);
            PowerReadACValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, ptrValue, ref size);
            int value = Marshal.ReadInt32(ptrValue);
            //メモリ開放
            Marshal.FreeHGlobal(ptrValue);

            if (value == 0)
            {
                return "sleepOFF";
            }
            return value.ToString() + "秒";
        }
    }


    public class SetDisplayTimeoutACDefinition : SettingDefinition
    {
        /// <summary>
        /// アクティブな電源プランを取得
        /// </summary>
        /// <param name="UserRootPowerKey"></param>
        /// <param name="ActivePolicyGuid"></param>
        /// <returns></returns>
        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr ActivePolicyGuid);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerReadACValue(IntPtr RootPowerKey, Guid SchemeGuid, Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref int Type, IntPtr Buffer, ref uint BufferSize);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerWriteACValueIndex(IntPtr RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, uint AcValueIndex);

        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "SetDisplayTimeoutAC"; }
        }

        public override string[] SelectItem
        {
            get
            {
                return new string[]
                {
                    "sleepOFF", "10分","30分","1時間"
                };
            }
        }

        public override string Description
        {
            get { return "ディスプレイスリープまでの時間（電源接続時）"; }
        }

        public override void ApplyValue(string value)
        {
            uint setValue = 0;

            switch (value)
            {
                case "sleepOFF":
                    setValue = 0;
                    break;
                case "10分":
                    setValue = 10 * 60;
                    break;
                case "30分":
                    setValue = 30 * 60;
                    break;
                case "1時間":
                    setValue = 60 * 60;
                    break;
            }

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/display-settings
            Guid guidSleepSubgroup = new Guid("7516b95f-f776-4464-8c53-06167f40cc99");
            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/display-settings-display-idle-timeout
            Guid guidSleepTimeSetting = new Guid("3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e");
            Guid activeGuid;
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return;
            }
            activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            PowerWriteACValueIndex(IntPtr.Zero, ref activeGuid, ref guidSleepSubgroup, ref guidSleepTimeSetting, setValue);
        }

        public override string GetValue()
        {
            //アクティブな電源スキームguid取得
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return "ErrorCode:" + result.ToString();
            }
            //この値も表示させたい
            Guid activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/display-settings
            Guid guidSleepSubgroup = new Guid("7516b95f-f776-4464-8c53-06167f40cc99");

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/display-settings-display-idle-timeout
            Guid guidSleepTimeSetting = new Guid("3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e");

            //階層的にはguidSleepSubgroup/guidSleepTimeSetting....

            //現在のディスプレイスリープ時間取得
            int registryValueKind = (int)RegistryValueKind.DWord;
            uint size = (uint)Marshal.SizeOf(typeof(int));
            //バッファにNullを渡してsizeに適切なバッファサイズ取得
            PowerReadACValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, IntPtr.Zero, ref size);

            IntPtr ptrValue = Marshal.AllocHGlobal((int)size);
            PowerReadACValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, ptrValue, ref size);
            int value = Marshal.ReadInt32(ptrValue);
            //メモリ開放
            Marshal.FreeHGlobal(ptrValue);

            if (value == 0)
            {
                return "sleepOFF";
            }
            return value.ToString() + "秒";
        }
    }


    public class SetDisplayTimeoutDCDefinition : SettingDefinition
    {
        /// <summary>
        /// アクティブな電源プランを取得
        /// </summary>
        /// <param name="UserRootPowerKey"></param>
        /// <param name="ActivePolicyGuid"></param>
        /// <returns></returns>
        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerGetActiveScheme(IntPtr UserRootPowerKey, ref IntPtr ActivePolicyGuid);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerReadDCValue(IntPtr RootPowerKey, Guid SchemeGuid, Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, ref int Type, IntPtr Buffer, ref uint BufferSize);

        [DllImport("powrprof.dll", SetLastError = true)]
        public static extern uint PowerWriteDCValueIndex(IntPtr RootPowerKey, ref Guid SchemeGuid, ref Guid SubGroupOfPowerSettingsGuid, ref Guid PowerSettingGuid, uint AcValueIndex);

        public override string Version
        {
            get { return "0.1.0"; }
        }

        public override string Name
        {
            get { return "SetDisplayTimeoutDC"; }
        }

        public override string[] SelectItem
        {
            get
            {
                return new string[]
                {
                    "sleepOFF", "10分","30分","1時間"
                };
            }
        }

        public override string Description
        {
            get { return "ディスプレイスリープまでの時間（バッテリ駆動時）"; }
        }

        public override void ApplyValue(string value)
        {
            uint setValue = 0;

            switch (value)
            {
                case "sleepOFF":
                    setValue = 0;
                    break;
                case "10分":
                    setValue = 10 * 60;
                    break;
                case "30分":
                    setValue = 30 * 60;
                    break;
                case "1時間":
                    setValue = 60 * 60;
                    break;
            }

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/display-settings
            Guid guidSleepSubgroup = new Guid("7516b95f-f776-4464-8c53-06167f40cc99");
            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/display-settings-display-idle-timeout
            Guid guidSleepTimeSetting = new Guid("3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e");
            Guid activeGuid;
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return;
            }
            activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            PowerWriteDCValueIndex(IntPtr.Zero, ref activeGuid, ref guidSleepSubgroup, ref guidSleepTimeSetting, setValue);
        }

        public override string GetValue()
        {
            //アクティブな電源スキームguid取得
            IntPtr ptr = IntPtr.Zero;
            uint result = PowerGetActiveScheme(IntPtr.Zero, ref ptr);
            if (result != 0)
            {
                Console.WriteLine("エラーが発生しました。エラーコード: " + result);
                Console.ReadLine();
                return "ErrorCode:" + result.ToString();
            }
            //この値も表示させたい
            Guid activeGuid = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/display-settings
            Guid guidSleepSubgroup = new Guid("7516b95f-f776-4464-8c53-06167f40cc99");

            //参考URL:https://learn.microsoft.com/ja-jp/windows-hardware/customize/power-settings/display-settings-display-idle-timeout
            Guid guidSleepTimeSetting = new Guid("3c0bc021-c8a8-4e07-a973-6b14cbcb2b7e");

            //階層的にはguidSleepSubgroup/guidSleepTimeSetting....

            //現在のディスプレイスリープ時間取得
            int registryValueKind = (int)RegistryValueKind.DWord;
            uint size = (uint)Marshal.SizeOf(typeof(int));
            //バッファにNullを渡してsizeに適切なバッファサイズ取得
            PowerReadDCValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, IntPtr.Zero, ref size);

            IntPtr ptrValue = Marshal.AllocHGlobal((int)size);
            PowerReadDCValue(IntPtr.Zero, activeGuid, guidSleepSubgroup, ref guidSleepTimeSetting, ref registryValueKind, ptrValue, ref size);
            int value = Marshal.ReadInt32(ptrValue);
            //メモリ開放
            Marshal.FreeHGlobal(ptrValue);

            if (value == 0)
            {
                return "sleepOFF";
            }
            return value.ToString() + "秒";
        }
    }
}
