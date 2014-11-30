using System;
using System.IO;
using System.Xml.Serialization;

namespace PetZombieAndroid
{
    public class FileWorker
    {
        public string name;
        public string ext;
        public int memory;
        public FileWorker()
        {
            this.name = "myfile";
            this.ext = ".txt";
            this.memory = 512;
        }

        public static void AddInfo(Object obj)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, "myfile.xml");

            XmlSerializer xml =  new XmlSerializer(obj.GetType());
            using (var fStream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xml.Serialize(fStream, obj);
            }

        }

        public static Object GetDeserializeObject(Type type)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filename = Path.Combine(path, "myfile.xml");

            XmlSerializer xml =  new XmlSerializer(type);
            FileStream fs = new FileStream(filename, FileMode.OpenOrCreate);
            TextReader reader = new StreamReader(fs);

            Object obj = xml.Deserialize(reader);

            return obj;
        }
    }
}

