/* *************************************************************************************
' Script Name: GWSettings.cs
' **************************************************************************************
' @(#)    Purpose:
' @(#)    This is a shared component available to all .NET applications. It allows simple
' @(#)    settings system that can be used to store and retrieve an application's settings
' @(#)    You can store settings databases,Property, an INI file, etc.
' **************************************************************************************
'  Written By: Brad Detchevery
              2274 RTE 640, Hanwell NB
'
' Created:     2019-05-26 - Initial Architecture
' 
' **************************************************************************************
'Note: Changing this routine effects all programs that change system settings
'-------------------------------------------------------------------------------*/

using System;
using System.IO;
using System.Data.Odbc;

using System.Collections;
using System.Xml;

namespace org.geekwisdom
{
    public class GWSettings
    {
        private String ApplicationName;
        public GWSettings(String AppName)
        {
            ApplicationName = AppName;
            //Console.WriteLine(ApplicationName);
        }

        public GWSettings()
        {
            //NOte there are several methods for getting the application name. If the settings manager is constructued 
            //with s specific name then this is waht we will use, otherwise will use the name of the ese

            ApplicationName = "App";
            //Console.WriteLine(ApplicationName);
        }
        public string GetSetting(string SettingName, string DefaultValue)
        {

            return "Hello World2";

        }

        public string GetSetting(string FromLocation, string SettingName, string DefaultValue, string VersionNumber = "")
        {

            if (File.Exists(FromLocation))
            {
                if (FromLocation.IndexOf(".mdb") >= 0 || FromLocation.IndexOf(".accdb") >= 0)
                {


                    return GetSetting("Driver={Microsoft Access Driver (*.mdb, *.accdb)};" + "Dbq=" + FromLocation + ";Uid=Admin;Pwd=;", SettingName, DefaultValue, VersionNumber);

                }
                else if (FromLocation.IndexOf(".ini") >= 0)
                {
                    IniParser parser = new IniParser(FromLocation);
                    String SectionName = ApplicationName + "-" + VersionNumber;
                    String ReturnValue = parser.GetSetting(SectionName, SettingName);
                    if (ReturnValue == "") ReturnValue = parser.GetSetting(ApplicationName, SettingName);
                    //if (ReturnValue == "") ReturnValue=parser.GetSetting("",SettingName);
                    if (ReturnValue == "") ReturnValue = DefaultValue;
                    return ReturnValue;

                }
                else //if (FromLocation.IndexOf(".xml") >= 0 || FromLocation.IndexOf(".config") >= 0)
                {
                    //OPen the file see if it might be xml or text based
                    //Replace this with GetSEtting and GetSettingReverse!! HERE
                    string fileText = File.ReadAllText(FromLocation);
                    String r = GetAppSetting(fileText, SettingName);
                    if (r != "") return r;
                    if (r == "")
                    {
                        r = GetJavaPropertiesSetting(fileText, SettingName);
                    }
                    if (r != "") return r;
                    if (r == "")
                    {
                        //Console.WriteLine(e.Message);
                        //Looks like might be plain text file
                        string line;
                        string result = "";
                        bool done = false;
                        // Read the file and display it line by line.
                        System.IO.StreamReader file =
                           new System.IO.StreamReader(FromLocation);
                        line = file.ReadLine();
                        if (line == null) done = true;
                        while (!done)
                        {
                            char firstchar = line[0];
                            if (firstchar != '#' && firstchar != ';')
                            {
                                if (line.IndexOf("=") > 0)
                                {
                                    string myString = line.Replace(System.Environment.NewLine, "");
                                    string[] pairs = myString.Split('=');
                                    if (pairs[0].Trim().ToLower() == SettingName.Trim().ToLower()) { result = pairs[1]; done = true; }
                                }
                            }
                            line = file.ReadLine();
                            if (line == null) done = true;
                        }

                        file.Close();
                        if (result != "") return result;
                        return DefaultValue;
                        // Suspend the screen.


                    }
                    //Appears to be an XML
                    //return GetSetting("Driver={Microsoft Access Driver (*.mdb)};"+ "Dbq=" + FromLocation + ";Uid=Admin;Pwd=;",SettingName, DefaultValue, VersionNumber);

                }
            }
            else if (FromLocation.IndexOf("<?xml") >= 0)
            {

            }
            else if (FromLocation.IndexOf("Driver=") >= 0)
            {

                //Do a connection to a database and get the info

                using (OdbcConnection conn = new OdbcConnection(FromLocation))
                {
                    conn.Open();

                    // 1.  create a command object identifying the stored procedure
                    string TheCommand;
                    bool IsStoredProcedure = false;
                    string Result = "RESULT";
                    if (FromLocation.IndexOf("*.mdb") >= 0 || FromLocation.IndexOf("*.accdb") >= 0)
                    {
                        TheCommand = "SELECT SettingValue FROM SYSTEM_CONFIGURATION WHERE SettingName=?";
                        IsStoredProcedure = false;
                        Result = "SettingValue";
                    }
                    else
                    {
                        TheCommand = "{call GetSetting (?)}";
                        IsStoredProcedure = true;
                    }
                    OdbcParameter ODBCParameter = new OdbcParameter("@SettingName", OdbcType.NChar, 50);
                    ODBCParameter.Value = SettingName;

                    OdbcCommand cmd = new OdbcCommand(TheCommand, conn);

                    if (IsStoredProcedure) cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(ODBCParameter);



                    // execute the command
                    using (OdbcDataReader rdr = cmd.ExecuteReader())
                    {
                        // iterate through results, printing each to console
                        String SettingValue = "";
                        while (rdr.Read())
                        {

                            SettingValue = rdr[Result].ToString();

                        }
                        if (SettingValue == "") return DefaultValue;
                        return SettingValue;
                    }
                }
            }
            else if (FromLocation.IndexOf("Provider=") >= 0)
            {

                //Access database Do a connection to a database and get the info
                using (OdbcConnection conn = new OdbcConnection(FromLocation))
                {
                    conn.Open();

                    // 1.  create a command object identifying the stored procedure

                    OdbcParameter ODBCParameter = new OdbcParameter("@SettingName", OdbcType.NChar, 50);
                    ODBCParameter.Value = SettingName;

                    OdbcCommand cmd = new OdbcCommand("{call GetSetting (?)}", conn);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(ODBCParameter);



                    // execute the command
                    using (OdbcDataReader rdr = cmd.ExecuteReader())
                    {
                        // iterate through results, printing each to console
                        String SettingValue = "";
                        while (rdr.Read())
                        {
                            SettingValue = rdr["Result"].ToString();
                        }
                        if (SettingValue == "") return DefaultValue;
                        return SettingValue;
                    }
                }
            }
            else
            {
                //Console.WriteLine(e.Message);
                //Looks like might be plain text file
                string[] lines = FromLocation.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                string result = "";

                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];
                    char firstchar = line[0];
                    if (firstchar != '#' && firstchar != ';')
                    {
                        if (line.IndexOf("=") > 0)
                        {
                            string myString = line.Replace(System.Environment.NewLine, "");
                            string[] pairs = myString.Split('=');

                            if (pairs[0].Trim().ToLower() == SettingName.Trim().ToLower()) { result = pairs[1]; }
                        }
                    }

                }
            }

            return "";

        }

        public string GetAppSettingReverse(string ConfigFile, string SettingName, string DefaultValue = "")
        {
            // Get an app setting from a DIFFERENT .CONFIG file
            string RetVal = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ConfigFile);
                RetVal = doc.DocumentElement.SelectSingleNode("/configuration/appSettings/add[@value='" + SettingName + "']").Attributes["key"].Value.ToString();
            }
            catch (Exception e)
            {
                RetVal = "";
            }
            return RetVal;
        }

        public string GetAppSetting(string ConfigFile, string SettingName, string DefaultValue = "")
        {
            // Get an app setting from a DIFFERENT .CONFIG file
            string RetVal = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ConfigFile);
                RetVal = doc.DocumentElement.SelectSingleNode("/configuration/appSettings/add[@key='" + SettingName + "']").Attributes["value"].Value.ToString();

            }
            catch (Exception e)
            {
                RetVal = "";
            }
            return RetVal;
        }


        public string GetJavaPropertiesSetting(string ConfigFile, string SettingName, string DefaultValue = "")
        {
            // Get an app setting from a DIFFERENT .CONFIG file
            string RetVal = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ConfigFile);
                RetVal = doc.DocumentElement.SelectSingleNode("/properties/entry[@key='" + SettingName + "']").InnerText.ToString();

            }
            catch (Exception e)
            {
                RetVal = "";
            }
            return RetVal;
        }



    }
    class IniParser
    {
        private Hashtable keyPairs = new Hashtable();
        private String iniFilePath;

        private struct SectionPair
        {
            public String Section;
            public String Key;
        }

        /// <summary>
        /// Opens the INI file at the given path and enumerates the values in the IniParser.
        /// </summary>
        /// <param name="iniPath">Full path to INI file.</param>
        public IniParser(String iniPath)
        {
            TextReader iniFile = null;
            String strLine = null;
            String currentRoot = null;
            String[] keyPair = null;

            iniFilePath = iniPath;

            if (File.Exists(iniPath))
            {
                try
                {
                    iniFile = new StreamReader(iniPath);

                    strLine = iniFile.ReadLine();

                    while (strLine != null)
                    {
                        strLine = strLine.Trim().ToUpper();

                        if (strLine != "")
                        {
                            if (strLine.StartsWith("[") && strLine.EndsWith("]"))
                            {
                                currentRoot = strLine.Substring(1, strLine.Length - 2);
                            }
                            else
                            {
                                keyPair = strLine.Split(new char[] { '=' }, 2);

                                SectionPair sectionPair;
                                String value = null;

                                if (currentRoot == null)
                                    currentRoot = "ROOT";

                                sectionPair.Section = currentRoot;
                                sectionPair.Key = keyPair[0];

                                if (keyPair.Length > 1)
                                    value = keyPair[1];

                                keyPairs.Add(sectionPair, value);
                            }
                        }

                        strLine = iniFile.ReadLine();
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (iniFile != null)
                        iniFile.Close();
                }
            }
            else
                throw new FileNotFoundException("Unable to locate " + iniPath);

        }

        /// <summary>
        /// Returns the value for the given section, key pair.
        /// </summary>
        /// <param name="sectionName">Section name.</param>
        /// <param name="settingName">Key name.</param>
        public String GetSetting(String sectionName, String settingName)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper();
            sectionPair.Key = settingName.ToUpper();

            return (String)keyPairs[sectionPair];
        }

        /// <summary>
        /// Enumerates all lines for given section.
        /// </summary>
        /// <param name="sectionName">Section to enum.</param>
        public String[] EnumSection(String sectionName)
        {
            ArrayList tmpArray = new ArrayList();

            foreach (SectionPair pair in keyPairs.Keys)
            {
                if (pair.Section == sectionName.ToUpper())
                    tmpArray.Add(pair.Key);
            }

            return (String[])tmpArray.ToArray(typeof(String));
        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        /// <param name="settingValue">Value of key.</param>
        public void AddSetting(String sectionName, String settingName, String settingValue)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper();
            sectionPair.Key = settingName.ToUpper();

            if (keyPairs.ContainsKey(sectionPair))
                keyPairs.Remove(sectionPair);

            keyPairs.Add(sectionPair, settingValue);
        }

        /// <summary>
        /// Adds or replaces a setting to the table to be saved with a null value.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void AddSetting(String sectionName, String settingName)
        {
            AddSetting(sectionName, settingName, null);
        }

        /// <summary>
        /// Remove a setting.
        /// </summary>
        /// <param name="sectionName">Section to add under.</param>
        /// <param name="settingName">Key name to add.</param>
        public void DeleteSetting(String sectionName, String settingName)
        {
            SectionPair sectionPair;
            sectionPair.Section = sectionName.ToUpper();
            sectionPair.Key = settingName.ToUpper();

            if (keyPairs.ContainsKey(sectionPair))
                keyPairs.Remove(sectionPair);
        }

        /// <summary>
        /// Save settings to new file.
        /// </summary>
        /// <param name="newFilePath">New file path.</param>
        public void SaveSettings(String newFilePath)
        {
            ArrayList sections = new ArrayList();
            String tmpValue = "";
            String strToSave = "";

            foreach (SectionPair sectionPair in keyPairs.Keys)
            {
                if (!sections.Contains(sectionPair.Section))
                    sections.Add(sectionPair.Section);
            }

            foreach (String section in sections)
            {
                strToSave += ("[" + section + "]\r\n");

                foreach (SectionPair sectionPair in keyPairs.Keys)
                {
                    if (sectionPair.Section == section)
                    {
                        tmpValue = (String)keyPairs[sectionPair];

                        if (tmpValue != null)
                            tmpValue = "=" + tmpValue;

                        strToSave += (sectionPair.Key + tmpValue + "\r\n");
                    }
                }

                strToSave += "\r\n";
            }

            try
            {
                TextWriter tw = new StreamWriter(newFilePath);
                tw.Write(strToSave);
                tw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Save settings back to ini file.
        /// </summary>
        public void SaveSettings()
        {
            SaveSettings(iniFilePath);
        }
    }




}
