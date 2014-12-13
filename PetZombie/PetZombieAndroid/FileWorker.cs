using System;
using System.IO;
using Polenter.Serialization;

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
            var serializer = new SharpSerializer();


            //Type type = obj.GetType();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullpath = Path.Combine(path, filename);
            /*
            XmlSerializer xml =  new XmlSerializer(type);

            using (var fStream = new FileStream(fullpath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xml.Serialize(fStream, obj);
            }
            */
            serializer.Serialize(obj, fullpath);
        }

        public PetZombie.User Read()
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullpath = Path.Combine(path, filename);
            /*
            XmlSerializer xml =  new XmlSerializer(typeof(PetZombie.User));
            FileStream fs = new FileStream(fullpath, FileMode.OpenOrCreate);
            TextReader reader = new StreamReader(fs);

            PetZombie.User user = (PetZombie.User)xml.Deserialize(reader);
            fs.Close();

            return user;
*/
            var serializer = new SharpSerializer();
            var user = serializer.Deserialize(fullpath);
            return (PetZombie.User)user;
        }
    }
}

