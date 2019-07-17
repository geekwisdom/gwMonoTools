using System;
using System.Collections.Generic;
using System.IO;
using org.geekwisdom;


namespace gwUnitTests
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            TestStudentService();
            //  LogUnitTest();
            //TestData2();
            //            TestStudent();
            //FileIOTests();
            //testType();
            Console.ReadLine();
        }

        public static void testType()
        {

            System.Reflection.Assembly currentAssem = System.Reflection.Assembly.GetExecutingAssembly();
            Type CorrectType = null ;
            foreach (Type t1 in currentAssem.GetTypes())
            {
                if (t1.Name == "Student")
                    CorrectType = t1;

            }
            Type t = CorrectType;
            if (t != null) Console.WriteLine("OK!");
            else Console.WriteLine("IS NULL!!");
            //Console.WriteLine (t);
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
            string xmlData = File.ReadAllText(@"/tmp/abc.xml");
            GWDataTable mytable = new GWDataTable();
            mytable.loadXml(xmlData);
            for (int i = 0; i < mytable.length(); i++)
            {
                GWRowInterface col = mytable.GetRow(i);
                foreach (KeyValuePair<string, string> entry in col.entrySet())
                {
                    Console.WriteLine(entry.Key + ":" + entry.Value);
                }



            }

        }

        public static void FileIOTests()
        {
            //Method #1 
            //GWDataIO FileTest = new GWDataIO();
            //FileTest.insert("test","c:\\temp\\DataIOTest.config");
            //Medhot #2
            GWDataIO FileTest = new GWDataIO("/tmp/DataIOTest.config");
            FileTest.Insert("{\"Name\":\"Brad\",\"Address\":\"Test\",\"ID\":\"4\"}");
            GWDataTable result = FileTest.Search("Name='Brad'");
            Console.WriteLine(result.toXml());
        }
        public static void TestStudent()
        {
            string xmlData = File.ReadAllText(@"/tmp/student.xml");
            GWDataTable mytable = new GWDataTable("", "root", "Student");
            mytable.loadXml(xmlData);
            GWDataTable ret = mytable.find("Name='Mike Gold'");
            Console.WriteLine("Len is " + ret.length());
            for (int i = 0; i < ret.length(); i++)
            {
                Student col = (Student)ret.GetRow(i);
                Console.WriteLine("Name is " + col.getName());
            }
            

            }

       
        public static void TestStudentService()
        {
            StudentService myStudents = new StudentService();
            myStudents.Insert("{\"Name\":\"Brad\",\"Address\":\"Test\",\"ID\":\"4\"}");
            GWDataTable all = myStudents.Search("ID > 0");
            Student first = (Student)all.GetRow(0);
            Console.WriteLine("Name is " + first.getName());
        }

        public static void TestData2()
        {
            try
            {
                GWDataTable mytable = new GWDataTable("","students");
                //GWDataTable mytable = new GWDataTable();

                Dictionary<string, string> myrow = new Dictionary<string, string>();
                myrow.Add("Name", "Brad");
                myrow.Add("Age", "43");
                mytable.Add(myrow);
                Console.WriteLine(mytable.toXml());
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
    