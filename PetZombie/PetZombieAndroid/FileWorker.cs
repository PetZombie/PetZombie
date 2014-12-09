using System;
using System.IO;
using System.Xml.Serialization;

namespace PetZombieAndroid
{
    public class FileWorker: PetZombie.IDataService
    {
        public string filename;

        public FileWorker()
        {
            this.filename = "myfile.xml";
        }

        public void Write(PetZombie.User obj)
        {
            Type type = obj.GetType();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullpath = Path.Combine(path, filename);

            XmlSerializer xml =  new XmlSerializer(type);
            using (var fStream = new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xml.Serialize(fStream, obj);
            }
        }

        public PetZombie.User Read()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullpath = Path.Combine(path, filename);

            XmlSerializer xml =  new XmlSerializer(typeof(PetZombie.User));
            FileStream fs = new FileStream(fullpath, FileMode.OpenOrCreate);
            TextReader reader = new StreamReader(fs);

            PetZombie.User obj = xml.Deserialize(reader) as PetZombie.User;

            return obj;
        }
    }
}

