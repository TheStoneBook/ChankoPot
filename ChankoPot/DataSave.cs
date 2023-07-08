using System.IO;
using System.Reflection;
using System.Text;
using YamlDotNet.Serialization;

namespace ChankoPot
{
    public class Receipt
    {
        //public string TimeStamp { get; set; }
        //public string Comment { get; set; }
        public SettingObjectData[] SettingObjectDatas { get; set; }
    }

    public class SettingObjectData
    {
        public string Version { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public static class DataSave
    {
        public static void Save(Receipt receipt)
        {
            string savePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\receipt.yaml";
            using TextWriter writer = File.CreateText(savePath);
            Serializer serializer = new Serializer();
            serializer.Serialize(writer, receipt);
        }

        public static Receipt Load()
        {
            string savePath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\receipt.yaml";

            if (!File.Exists(savePath)) return null;

            var input = new StreamReader(savePath, Encoding.UTF8);
            var deserializer = new Deserializer();
            var deserializeObject = deserializer.Deserialize<Receipt>(input);
            return deserializeObject;
        }
    }
}
