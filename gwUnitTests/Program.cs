using System;
using System.Collections.Generic;
using System.IO;
using org.geekwisdom;
using org.geekwisdom.data;

namespace gwUnitTests
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //  LogUnitTest();
            //TestData2();
            // TestSettings();
            LogUnitTest();
            Console.ReadLine();
        }

        public static void TestSettings()
        {
            string R = "";
            string curdir = Directory.GetCurrentDirectory();
            GWSettings mySettings = new GWSettings();
            Console.WriteLine(curdir);

            R = mySettings.GetSetting(curdir + "/example.properties", "hellomsg", "default");
            Console.WriteLine(R);
            R = mySettings.GetSetting(curdir + "/testnet.config", "hellomsg", "default");
            Console.WriteLine(R);

        }
        public static void LogUnitTest()
        {
            GWLogger myLogger = GWLogger.getInstance(5);
            myLogger.WriteLog(2, GWLogger.LogType.Error, "just a test3");
            return;
        }

        public static void TestData()
        {


            string curdir = Directory.GetCurrentDirectory();
            string xmlData = File.ReadAllText(curdir + "/abc.xml");
            GWDataTable mytable = new GWDataTable();
            mytable.loadXml(xmlData);
            for (int i = 0; i < mytable.length(); i++)
            {
                GWDataRow col = mytable.GetRow(i);
                foreach (KeyValuePair<string, string> entry in col.entrySet())
                {
                    Console.WriteLine(entry.Key + ":" + entry.Value);
                }



            }

        }

        public static void TestData2()
        {
            try
            {
                //GWDataTable mytable = new GWDataTable("","students");
                GWDataTable mytable = new GWDataTable();

                Dictionary<string, string> myrow = new Dictionary<string, string>();
                myrow.Add("Name", "Brad");
                myrow.Add("Age", "43");
                mytable.Add(myrow);
                Dictionary<string, string> myrow2 = new Dictionary<string, string>();
                myrow2.Add("Name", "Cathy");
                myrow2.Add("Age", "39");
                mytable.Add(myrow2);
                GWDataTable results = mytable.find("Name='Cathy'");
                Console.WriteLine(results.toXml());
                //Console.WriteLine(mytable.toXml());
                //System.out.println(mytable.toXml());
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
