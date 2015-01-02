using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace PetZombieAndroid
{
    public class FileWorker: PetZombie.IDataService
    {
        public string filename;

        public FileWorker()
        {
            this.filename = "myfile.txt";
        }

        public void Write(PetZombie.User obj)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullpath = Path.Combine(path, filename);

            string[] lines = new string[11];
            lines[0] = Convert.ToString(obj.LivesCount);
            lines[1] = Convert.ToString(obj.BrainsCount);
            lines[2] = Convert.ToString(obj.LastLevel);
            lines[3] = obj.Zombie.Name;
            lines[4] = Convert.ToString(obj.Zombie.Satiety);
            lines[5] = Convert.ToString(obj.Money);

            lines[6] = Convert.ToString(obj.Weapon[0].Count);
            lines[7] = Convert.ToString(obj.Weapon[1].Count);
            lines[8] = Convert.ToString(obj.Weapon[2].Count);
            lines[9] = obj.time;
            lines[10] = obj.GetTimer();

            File.WriteAllLines(fullpath, lines);
            //File.WriteAllText(fullpath, "Nemo");

            /*
            var writer = new XmlSerializer(obj.GetType());
            var file = File.Create(fullpath);
            writer.Serialize(file, obj);
            file.Close();

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

            serializer.Serialize(obj, fullpath);
            */
        }

        public PetZombie.User Read()
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullpath = Path.Combine(path, filename);

            try{
            var text = System.IO.File.ReadAllLines(fullpath);
            List<PetZombie.Weapon> weapon = new List<PetZombie.Weapon>();
            weapon.Add(new PetZombie.Soporific(Convert.ToInt32(text[6])));
            weapon.Add(new PetZombie.Bomb(Convert.ToInt32(text[7])));
            weapon.Add(new PetZombie.Gun(Convert.ToInt32(text[8])));

            PetZombie.User user = new PetZombie.User(Convert.ToInt32(text[0]), Convert.ToInt32(text[1]), new PetZombie.ZombiePet(text[3],
                Convert.ToSingle(text[4])), weapon, Convert.ToInt32(text[5]), Convert.ToInt32(text[2]), text[9], text[10]);
             
            return user;
            }
            catch
            {
              return null;
            }
            /*
            var serializer = new XmlSerializer(typeof(PetZombie.User));
            FileStream fs = new FileStream(fullpath, FileMode.Open);
            using (var reader = new StreamReader(fs))
            {
                return (PetZombie.User)serializer.Deserialize(reader);
            }

            /*
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string fullpath = Path.Combine(path, filename);
            XmlSerializer xml =  new XmlSerializer(typeof(PetZombie.User));
            
            FileStream fs = new FileStream(fullpath, FileMode.OpenOrCreate);
            TextReader reader = new StreamReader(fs);

            PetZombie.User user = (PetZombie.User)xml.Deserialize(reader);
            fs.Close();

            return user;

            var serializer = new SharpSerializer();
            var user = serializer.Deserialize(fullpath);
            return (PetZombie.User)user;
            */
        }
    }
}

