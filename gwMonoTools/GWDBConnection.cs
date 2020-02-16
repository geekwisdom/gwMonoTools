using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Odbc;
using System.IO;
using Microsoft.Win32;

namespace org.geekwisdom
{
    public class GWDBConnection
    {
        private string finalconnectstring = "";
        private string un = "";
        private string pw = "";
        public GWDBConnection(string connectstr)
        {
            string[] CArray=null;
            if (File.Exists(connectstr))
            {
                string [] lines = File.ReadAllLines(connectstr);
                if (lines.Length == 1) lines = lines[0].Split(';');
                else CArray = lines;
            }
            else
            {
                CArray = connectstr.Split(';');

            }
            Dictionary<string, string> connectinfo = new Dictionary<string, string>();
            for (int i=0;i<CArray.Length;i++)
            {
                string itv = CArray[i];
                if (itv.IndexOf('=') > 0)
                {
                    string[] parts = itv.Split('=');
                    if (parts[0].IndexOf(':') > 0)
                    {
                        string[] m = parts[0].Split(':');
                        connectinfo.Add("driver", m[0].ToLower());
                        connectinfo.Add("host",parts[1].ToLower());
                                           
                    }
                    else
                    {
                        connectinfo.Add(parts[0].ToLower(), parts[1]);

                    }
                }
            }
            string port = "";
            string driver = "sqlsrv";
            string host = "localhost";
            string dbname = "";
            string drivername = "";
            string un = "";
            string pw = "";
            if (connectinfo.ContainsKey("driver")) driver = connectinfo["driver"];
            if (connectinfo.ContainsKey("host")) host = connectinfo["host"];
            if (connectinfo.ContainsKey("port")) port = connectinfo["port"];
            if (connectinfo.ContainsKey("dbname")) dbname = connectinfo["dbname"];
            if (connectinfo.ContainsKey("database")) dbname = connectinfo["database"];
            if (connectinfo.ContainsKey("database")) dbname = connectinfo["database"];
            if (connectinfo.ContainsKey("uid")) un = connectinfo["uid"];
            if (connectinfo.ContainsKey("pw"))  pw = connectinfo["pw"];
            if (connectinfo.ContainsKey("username")) un = connectinfo["username"];
            if (connectinfo.ContainsKey("password")) pw = connectinfo["password"];

            if (driver != "")
            {
                string driverfilter = driver;
                if (driver == "oci") driverfilter = "oracle";
                    string[] drivers = GetOdbcDriverNames(driverfilter);
                if (drivers.Length > 0) drivername = GetOdbcDriverNames(driver)[0];
                
            }

            if (driver.ToLower() == "sqlsrv")
            {
                if (drivername == "") drivername = "SQL Server";
                if (port == "") finalconnectstring = "Driver={" + drivername + "};Server=" + host + ";Database=" + dbname + ";";
                else finalconnectstring = "Driver={" + drivername + "}; Server=" + host + "," + port + ";Database=" + dbname + ";";
                if (un == "") finalconnectstring = finalconnectstring + "Trusted_Connection=Yes;";
                else finalconnectstring = finalconnectstring + "Uid=" + un + ";Pwd=" + pw + ";";
              }

            if (driver.ToLower() == "mysql")
            {
                //Driver ={ MySQL ODBC 5.2 ANSI Driver}; Server = myServerAddress; Database = myDataBase; User = myUsername; Password = myPassword; Option = 3
                if (drivername == "") drivername = "MySQL ODBC 5.3 UNICODE Driver";
                if (port == "") finalconnectstring = "Provider=MSDASQL;Driver={" + drivername + "};Server=" + host + ";Database=" + dbname + ";";
                else finalconnectstring = "Provider=MSDASQL;Driver={" + drivername + "}; Server=" + host + "," + port + ";Database=" + dbname + ";";
                finalconnectstring = finalconnectstring + "User=" + un + ";Password=" + pw + ";Option = 3;";
            }

            if (driver.ToLower() == "oci")
            {
                //SERVER = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = MyHost)(PORT = MyPort))(CONNECT_DATA = (SERVICE_NAME = MyOracleSID))); uid = myUsername; pwd = myPassword;
                if (drivername == "") drivername = "Microsoft ODBC Driver for Oracle";
                if (port == "") finalconnectstring = "Driver = {" + drivername + "};Server=" + host + ";Database=" + dbname + ";";
                else finalconnectstring = "Driver = {" + drivername + "}; Server=" + host + "," + port + ";Database=" + dbname + ";";
                finalconnectstring = finalconnectstring + "Uid = " + un + ";Pwd = " + pw + ";";
            }
            /*
            if (driver.ToLower() == "oci")
            {
                //Driver ={ MySQL ODBC 5.2 ANSI Driver}; Server = myServerAddress; Database = myDataBase; User = myUsername; Password = myPassword; Option = 3
                if (drivername == "") drivername = "MySQL ODBC 5.2 ANSI Driver";
                if (port == "") finalconnectstring = "Provider=MSDASQL;Driver = {" + drivername + "};Server=" + host + ";Database=" + dbname + ";";
                else finalconnectstring = "Provider=MSDASQL;Driver = {" + drivername + "}; Server=" + host + "," + port + ";Database=" + dbname + ";";
                finalconnectstring = finalconnectstring + "User = " + un + ";Password = " + pw + ";";
            }
    */    
    }

        public OdbcConnection getConnection()
        {
            OdbcConnection conn = new OdbcConnection(finalconnectstring);
            conn.Open();
            return conn;
        }

        private static string[] GetOdbcDriverNames(string filter)
        {
            string[] odbcDriverNames = null;
            List<String> finalOdbcNames = new List<String>();
            using (RegistryKey localMachineHive = Registry.LocalMachine)
            using (RegistryKey odbcDriversKey = localMachineHive.OpenSubKey(@"SOFTWARE\ODBC\ODBCINST.INI\ODBC Drivers"))
            {
                if (odbcDriversKey != null)
                {
                    odbcDriverNames = odbcDriversKey.GetValueNames();
                    if (filter !=  "")
                    {
                        for (int i=0;i<odbcDriverNames.Length;i++)
                        {
                            if (odbcDriverNames[i].ToLower().IndexOf(filter) >= 0 ) finalOdbcNames.Add(odbcDriverNames[i]);
                        }
                    return finalOdbcNames.ToArray();
                }
                    
                }
            return odbcDriverNames;
        }

        
        }

    }
}
