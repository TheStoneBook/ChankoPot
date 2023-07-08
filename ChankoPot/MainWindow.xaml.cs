using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using Plugin.Core;

namespace ChankoPot
{
    public class SettingObject
    {
        public SettingObject(SettingDefinition definition)
        {
            Definition = definition;
            Value = definition.SelectItem[0];
            Current = definition.GetValue();
        }

        public SettingDefinition Definition { get; set; }

        public string Value { get; set; }
        
        public string Current { get; set; }

        public SettingObjectData GenerateSettingObjectData()
        {
            SettingObjectData data = new SettingObjectData();

            data.Version=Definition.Version;
            data.Name= Definition.Name;
            data.Value = Value;

            return data;
        }
    }

    public partial class MainWindow : Window
    {
        private SettingDefinition[] definitionList;

        private ObservableCollection<SettingDefinition> _settingDefinitions = new ObservableCollection<SettingDefinition>()
        {
        };
        private ObservableCollection<SettingDefinition> SettingDefinitions { get { return this._settingDefinitions; } }

        private ObservableCollection<SettingObject> _settingObjects = new ObservableCollection<SettingObject>()
        {
        };

        private ObservableCollection<SettingObject> SettingObjects { get { return this._settingObjects; } }

        public MainWindow()
        {
            InitializeComponent();

            Receipt receipt = new Receipt();
            receipt = DataSave.Load();

            var pluginArray = LoadDefinitionsFromPlugins();
            if(pluginArray != null)
            {
                var pluginList = pluginArray.ToList();
                definitionList = pluginArray;
                if (receipt != null)
                {

                    foreach (var data in receipt.SettingObjectDatas)
                    {

                        foreach (var plugin in pluginList)
                        {
                            if (data.Name == plugin.Name)
                            {
                                SettingObject settingObject = new SettingObject(plugin);
                                settingObject.Value = data.Value;
                                SettingObjects.Add(settingObject);
                            }
                        }
                    }
                }
            }

            SettingListBox.ItemsSource = SettingObjects;
            PluginListBox.ItemsSource = SettingDefinitions;

            UpdateDefinitionView();
        }


        private async void PreviewObjectsMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            await Task.Delay(10);

            var listBox = sender as ListBox;

            if (listBox == null) return;

            if (listBox.SelectedItem == null) return;

            var let = listBox.SelectedItem;
            DragDrop.DoDragDrop(listBox, listBox.SelectedItem, DragDropEffects.Move);
        }

        private void ObjectDrop(object sender, DragEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null) return;
 
            var item = (SettingDefinition)e.Data.GetData("Definition");

            if (item == null) return;

            AddObject(item);
        }

        private async void PreviewDefinitionMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            await Task.Delay(10);

            var listBox = sender as ListBox;

            if (listBox == null) return;

            if (listBox.SelectedItem == null) return;

            DataObject dataObject = new DataObject("Definition", listBox.SelectedItem);

            DragDrop.DoDragDrop(listBox, dataObject, DragDropEffects.Move);
        }

        private void DefinitionDrop(object sender, DragEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null) return;

            var item = (SettingObject)e.Data.GetData(typeof(SettingObject));
            if (item == null) return;

            RemoveObject(item);
        }

        void AddObject(SettingDefinition definition)
        {
            foreach(var s in SettingObjects)
            {
                if (s.Definition.Name == definition.Name) return;
            }

            SettingObject settingObject = new SettingObject(definition);

            SettingObjects.Add(settingObject);
            UpdateDefinitionView();
        }

        void RemoveObject(SettingObject settingObject)
        {
            if(settingObject != null)
            {
                SettingObjects.Remove(settingObject);
                UpdateDefinitionView();
            }
        }

        void UpdateDefinitionView()
        {
            SettingDefinitions.Clear();

            if (definitionList == null) return;

            foreach (var v in definitionList)
            {
                bool registered = false;

                foreach(var s in SettingObjects)
                {
                    if(s.Definition.Name == v.Name)
                    {
                        registered = true;
                        break;
                    }
                }

                if (!registered)
                {
                    SettingDefinitions.Add(v);
                }
            }
        }

        SettingDefinition[] LoadDefinitionsFromPlugins()
        {
            //exeと同階層のpluginディレクトリから、dllのリストを取得
            string pluginDirectoryPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\plugin";

            if (!Directory.Exists(pluginDirectoryPath)) return null;

            string[] dllFilePathList = Directory.GetFiles(pluginDirectoryPath, "*.dll");

            List<SettingDefinition> definitions = new List<SettingDefinition>();

            foreach (string dllFilePath in dllFilePathList)
            {
                Assembly assembly = Assembly.LoadFrom(dllFilePath);

                Type[] allTypes = assembly.GetTypes();
                List<Type> foundTypes = new List<Type>();
                foreach (Type type in allTypes)
                {
                    foreach (var baseType in type.GetBaseTypes())
                    {
                        if (baseType.Name == "SettingDefinition")
                        {
                            definitions.Add((SettingDefinition)assembly.CreateInstance(type.FullName));
                            break;
                        }
                    }
                }
            }
            return definitions.ToArray();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var settingObject in SettingObjects)
            {
                settingObject.Definition.ApplyValue(settingObject.Value);
            }
            Reboot();
        }

        private void Reboot()
        {
            try
            {
                //自身をクローズ
                Application.Current.Shutdown();

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "shutdown.exe";

                //psi.Arguments = "-s -t 0";   // shutdown
                psi.Arguments = "-r -t 0";   // reboot
                
                //バックグラウンドで再起動実行
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                Process p = Process.Start(psi);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveRecipe();
        }

        private void SaveRecipe()
        {
            Receipt receipt = new Receipt();

            var datas = new List<SettingObjectData>();
            foreach(var settingObject in SettingObjects)
            {
                var data=settingObject.GenerateSettingObjectData();
                datas.Add(data);
            }

            receipt.SettingObjectDatas = datas.ToArray();
            DataSave.Save(receipt);
        }

        private void SystemEvents_SessionEnding(object sender, SessionEndingEventArgs e)
        {
            if (e.Reason == SessionEndReasons.Logoff)
            {
                //MessageBox.Show("ログオフを検知しました");
            }
            else if (e.Reason == SessionEndReasons.SystemShutdown)
            {
                Application.Current.Shutdown();
                //MessageBox.Show("シャットダウンを検知しました");
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ListBoxItem listBoxItem = FindVisualParent<ListBoxItem>(button);

            if (listBoxItem != null) 
            {
                var settingObject = listBoxItem.DataContext as SettingObject;
                if(settingObject != null)
                {
                    RemoveObject(settingObject);
                }
            }
        }

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
            {
                return null;
            }

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }
            else
            {
                return FindVisualParent<T>(parentObject);
            }
        }

        private void ReloadButton_Click(object sender, RoutedEventArgs e)
        {
            var plugin = LoadDefinitionsFromPlugins();
            definitionList = plugin;

            UpdateDefinitionView();
        }
    }
}
