using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace Utils
{
    public class Utils
    {

        static Random _rnd = new Random();

        public static int Rnd(int n) { return _rnd.Next(n); }

        public static T ReadObject<T>(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            object ret = ser.Deserialize(fs);
            fs.Close();
            return (T)ret;
        }

        public static void WriteObject<T>(string filename, T obj)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            XmlSerializer ser = new XmlSerializer(typeof(T));
            ser.Serialize(fs, obj);
            fs.Close();
        }

        public static void ErrorSound()
        {
            System.Media.SystemSounds.Exclamation.Play();
        }

    }
}
