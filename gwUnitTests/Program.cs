using System;
using System.IO;
using org.geekwisdom;

namespace gwUnitTests
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            string R = "";
            string curdir = Directory.GetCurrentDirectory();
            GWSettings mySettings = new GWSettings();
            Console.WriteLine(curdir);

            R = mySettings.GetSetting(curdir+"/example.properties", "hellomsg", "default");
            Console.WriteLine(R);
            R = mySettings.GetSetting(curdir+"/testnet.config", "hellomsg", "default");
            Console.WriteLine(R);
            Console.ReadLine();
        }
    }
}
    