using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.IO;
using org.geekwisdom;


namespace gwUnitTests
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // TestParsedCommand();
            //    TestStudentService();
            //  LogUnitTest();
           //  TestData2();
            //            TestStudent();
            // FileIOTests();
            ODBCTests();
            //testType();
            Console.ReadLine();
        }

        public static void testType()
        {

            System.Reflection.Assembly currentAssem = System.Reflection.Assembly.GetExecutingAssembly();
            Type CorrectType = null;
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
            string xmlData = File.ReadAllText(@"c:\temp\abc.xml");
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
            GWDataIO FileTest = new GWDataIO("c:\\temp\\DataIOTest.config");
            FileTest.Insert("{\"Name\":\"Brad\",\"Address\":\"Test\",\"ID\":\"4\"}");
            GWDataTable result = FileTest.Search("[ Name _EQ_ \"Brad\" ] !nodeleted");

            Console.WriteLine(result.toXml());
        }
        public static void TestStudent()
        {
            string xmlData = File.ReadAllText(@"c:\temp\student.xml");
            GWDataTable mytable = new GWDataTable("", "root", "Student");
            mytable.loadXml(xmlData);
            GWDataTable ret = mytable.find("[ Name _LIKE_ \"Mike Gold\" ]");

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
            GWDataTable all = myStudents.Search("[ ID _GT_ 0 ]");
            Student first = (Student)all.GetRow(0);
            Console.WriteLine("Name is " + first.getName());
        }

        public static void TestParsedCommand()
        {

            //   GWParsedCommand mycmd = new GWParsedCommand("[ ( BRAD _EQ_ 3 ] ) ");
            GWParsedCommand mycmd = new GWParsedCommand("[ ( Name _EQ_ \"Brad Detchvery\" ) ]");
            Console.WriteLine("Field:" + mycmd.getField());
            Console.WriteLine("Op:" + mycmd.getOperator());
            Console.WriteLine("Value:" + mycmd.getValue());

        }

        public static void TestData2()
        {
            // try
            // {
            GWDataTable mytable = new GWDataTable("", "students");
            //GWDataTable mytable = new GWDataTable();

            Dictionary<string, string> myrow = new Dictionary<string, string>();
            myrow.Add("Name", "Brad");
            myrow.Add("Age", "43");
            mytable.Add(myrow);
//            Console.WriteLine(mytable.toXml());
            Dictionary<string, string> myrow2 = new Dictionary<string, string>();
            myrow2.Add("Name", "Cathy");
            myrow2.Add("Age", "39");
            mytable.Add(myrow2);
            //GWDataTable results = mytable.find("Name='Cathy'");
            GWDataTable results = mytable.find("[ Name _EQ_ 'Cathy' ]");
            Console.WriteLine(results.toXml());
            //Console.WriteLine(mytable.toXml());
            //System.out.println(mytable.toXml());
        }
        //catch (Exception e)
        //{
        //throw e;
        //}
        //}

        private static void ODBCTests()
        {
            string whereclause = "[ UserID _EQ_  2 _OR_ UserID _EQ_  3 ]";
            GWQL sqltester = new GWQL(whereclause);
            //sqltester.setAllowedFields(allowedFields);
            GWQLSqlStringBuilder mysqlbuilder = new GWQLSqlStringBuilder();
            String finalCmd = "";
            finalCmd = sqltester.getCommand(mysqlbuilder);
            string queryString = "SELECT * FROM sec_UsersTable WHERE " + finalCmd;
           // "DRIVER={MySQL ODBC 3.51 Driver};" +
         //   "SERVER=localhost;DATABASE=test;" +
              //  "UID=myuserid;PASSWORD=mypassword;" +
            //"OPTION=3"
            string connectionString = @"DSN=test";
            ArrayList MyParms = mysqlbuilder.getParams();

            using (OdbcConnection connection = new OdbcConnection(connectionString))
            {
                //OdbcCommand command = new OdbcCommand(queryString, connection);
                OdbcCommand command = connection.CreateCommand();
                command.CommandText = queryString;

                for (int i = 0; i < MyParms.Count; i++)
                {
                    command.Parameters.AddWithValue("@" + i.ToString(), MyParms[i]);
                }


                connection.Open();
                // Execute the DataReader and access the data.
                OdbcDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine("CustomerID={0}", reader[0]);
                }

                // Call Close when done reading.
                reader.Close();

                // The connection is automatically closed at 
                // the end of the Using block.
            }

        }

    }
}
