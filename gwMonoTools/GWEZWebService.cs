using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Reflection;
using System.Text;

namespace org.geekwisdom
{
    public class GWEZWebService
    {
        protected string ServiceFile;
        protected string ServiceName;
        protected string UserName;
        protected GWDataTable fault403;



    public GWEZWebService(string servicename, string filepath, string username)
    {
        ServiceName = servicename;
        UserName = username;
        ServiceFile = filepath + servicename + ".xml";
        initialize403();
    }


    public GWEZWebService(string servicename, string filepath)
    {
        ServiceName = servicename;
        ServiceFile = filepath + servicename + ".xml";

    }
        public GWEZWebService(String servicename)
        {
            ServiceName = servicename;
            ServiceFile = System.IO.Path.GetTempPath() + servicename + ".xml";
            initialize403();
        }
        private void initialize403()
        {
            fault403 = new GWDataTable("", "fault");
            GWDataRow newrow = new GWDataRow();
            newrow.Set("code", "Server");
            newrow.Set("faultstring", "403 Access Denied");
            fault403.Add(newrow);
        }

        private String showError(String faultstring, String detail, String format)
        {
            GWDataTable errormsg = new GWDataTable("", "fault");
            GWDataRow newrow = new GWDataRow();
            newrow.Set("code", "Server");
            newrow.Set("faultstring", faultstring);
            if (detail != "") newrow.Set("detail", detail);
            errormsg.Add(newrow);
            if (format.ToLower() == "xml") return errormsg.toXml();
            if (format.ToLower() == "json")
            {
                try
                {
                    return errormsg.toJSON();
                }
                catch (Exception e)
                {
                    return "UNKOWN JSON DETAIL DECODE ERROR: ";
                }
            }

            return errormsg.toXml();
        }

        public string Fulfill(string Operation, List<string> Params, string format)
        {
            if (Operation == "") return this.showError("404 Not Found", "Missing Operation", format);
            //GWDataTable retval = new GWDataTable("","Results");
            string FinalOutput = "";
            GWDataTable GWServiceResults = new GWDataTable("", "Results");
            GWDataTable WebServiceConfig = new GWDataTable();

            if (!(System.IO.File.Exists(this.ServiceFile)))
            {
                return this.showError("404 Not Found", "No such service available: " + this.ServiceName, format);
            }

            string GWServiceFile = file_get_contents(this.ServiceFile);
            WebServiceConfig.loadXml(GWServiceFile);
            string OPType = "";
            string OpSource = "";
            string UserName = "";
            try
            {
                GWDataTable FoundRows = WebServiceConfig.find("[ OperationName _EQ_ \"" + Operation + "\" ]");

                if (FoundRows.length() == 0)
                {
                    return this.showError("404 Not Found", "No such operation available" + Operation, format);
                }

                if (FoundRows.length() > 0)
                {
                    GWDataRow FoundRow = (GWDataRow)FoundRows.GetRow(0);
                    OPType = FoundRow.Get("OperationType");
                    OpSource = FoundRow.Get("OperationSource");
                    bool AuthEnabled = FoundRow.has_column("AllowedUsers");
                    if (AuthEnabled)
                    {
                        String UserAuthNames = FoundRow.Get("AllowedUsers");

                        if (this.UserName == "")
                        {
                            if (format.ToLower() == "xml") return this.fault403.toXml();
                            if (format.ToLower() == "json") return this.fault403.toJSON();
                            return this.fault403.ToString();
                        }
                        int has_semi = UserAuthNames.IndexOf(";");
                        string[] AllowedUsers;
                        if (has_semi > 0)
                        {
                            AllowedUsers = UserAuthNames.Split(';');
                        }
                        else
                        {
                            AllowedUsers = new String[1];
                            AllowedUsers[0] = UserAuthNames;
                        }
                        bool AccessGranted = false;
                        for (int i = 0; i < AllowedUsers.Length; i++)
                        {
                            if (UserName.ToLower() == AllowedUsers[i].ToLower()) AccessGranted = true;
                        }
                        if (AccessGranted == false)
                        {
                            if (format.ToLower() == "xml") return this.fault403.toXml();
                            if (format.ToLower() == "json") return this.fault403.toJSON();
                            return this.fault403.ToString();
                        }
                    }
                    int isStoredProcedure = OpSource.IndexOf(".dll");
                    if (isStoredProcedure > 0)
                    {
                        List<string> PArray = new List<string>();
                        //string[] MethodData;

                        try
                        {
                            string[] MthdInfo = parseMethod(OPType);

                            if (Params.Count != MthdInfo.Length - 2)
                            {
                                return this.showError("500 Server Error", "Incorrect Parameters for Operation " + Operation, format);
                            }

//                            AssemblyName an = AssemblyName.GetAssemblyName(MthdInfo[0]);
                            AssemblyName an = AssemblyName.GetAssemblyName(OpSource);
                            Assembly assembly = Assembly.Load(an);
                            Type t = assembly.GetType(MthdInfo[0]);
                            if (t == null)
                            {
                                return "Yep null";
                            }
                            else
                            {
                                List<string> plist = new List<string>();
                                Dictionary<string, string> ParamsArray = orderParams(Params);

                                if (ParamsArray != null)
                                {
                                    foreach (KeyValuePair<string, string> entry in ParamsArray)
                                    {
                                        string datatype = entry.Value;
                                        plist.Add(datatype);
                                    }

                                    object[] parameters = plist.ToArray();
                                    var o = Activator.CreateInstance(t);
                                    if (o == null) return "Error Invoking!";
                                    else
                                    {
                                        MethodInfo methodinfo = t.GetMethod(MthdInfo[1]);
                                        if (methodinfo == null) return "No such method!" + MthdInfo[1];
                                        else
                                        {
                                            object result = methodinfo.Invoke(o, parameters);
                                            string ResultStr = result.ToString();
                                            StringReader sread;
                                            //If result is not XML we will make it xml
                                            string FirstChar = ResultStr.Substring(0, 1);
                                            if (FirstChar != "<")
                                            {
                                                Dictionary<String, String> RetVal = new Dictionary<string, string>();
                                                RetVal.Add("Return", ResultStr);
                                                GWServiceResults.Add(RetVal);
                                                
                                            }
                                            else
                                            {
                                                sread = new StringReader(ResultStr);
                                                GWServiceResults.loadXml(sread.ToString());
                                            }
                                            
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.StackTrace);
                        }
                    }
                    else
                    {
                        //stored procedure stuff
                        GWDBConnection dbconn = new GWDBConnection(OpSource);
                        OdbcConnection conn = dbconn.getConnection();
                       // conn.Open();
                        OdbcCommand cmd = new OdbcCommand(OPType, conn);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        Dictionary<string, string> ParamsArray = orderParams(Params);
                        List<string> plist = new List<string>();
                        if (ParamsArray != null)
                        {
                            if (ParamsArray.Count > 0)
                            {
                                int i = 0;
                                foreach (KeyValuePair<string, string> entry in ParamsArray)
                                {
                                    // string datatype = entry.Value;
                                    //plist.Add(datatype);
                                    //}
                                    // TODO: array at 0 is the data value
                                    //       at 1 is the date type
                                    //       at 2 is optionally the size
                                    //OdbcType thetype = (OdbcType)Enum.Parse(typeof(OdbcType), datatype[1], true);
                                    //OdbcParameter ODBCParameter = new OdbcParameter("@" + entry.Key, thetype, Int32.Parse(datatype[2]));
                                    OdbcParameter ODBCParameter = new OdbcParameter("@P" + i.ToString(), OdbcType.VarChar);
                                    ODBCParameter.Value = entry.Value;
                                    cmd.Parameters.Add(ODBCParameter);
                                    i++;
                                }

                            }
                        }

                        OdbcDataReader rdr;
                        DataTable dt;
                        try
                        {
                            rdr = cmd.ExecuteReader();
                            dt = new DataTable();
                        }
                        catch (OdbcException c)
                        {
                            //return "<ErrorMessage><Error>" + c.Message.ToString() + " " + c.Errors.Count.ToString() + "</Error></ErrorMessage>";
                           return this.showError("ODBC Exception in " + this.ServiceName,c.Message.ToString() + " " + c.Errors.Count.ToString(),  format);
                            
                        }
                        dt.Load(rdr);
                        //DataTable dtCloned = dt.Clone();
                        //foreach (DataColumn dc in dtCloned.Columns)
                            //dc.DataType = typeof(string);
                        foreach (DataRow row in dt.Rows)
                        {
                           
                            Dictionary<string, string> gwrow = new Dictionary<string, string>();
                            foreach (DataColumn col in dt.Columns)
                            {
                               
                                gwrow.Add(col.ColumnName, row[col.ColumnName].ToString());
                            }
                            
                            GWServiceResults.Add(gwrow);
                        }
                        conn.Close();
                        }

                    //ServiceResults.Tables.Add(dtCloned);
                    
                }

                    
                    if (format.ToLower() == "xml") return GWServiceResults.toXml();
                    if (format.ToLower() == "json") return GWServiceResults.toJSON();
                    return "format: " + GWServiceResults.ToString();

                
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return null;
        }
            private string file_get_contents(string filename)
        {
            return System.IO.File.ReadAllText(filename);
        }

        private string[] parseMethod(string MethodType)
        {
            //given a method type like Enterprise.SBB.MultiCRUD.MultiCrud(?,"abc",?)
            //return the method name and an array of default parameters 
            List<string> retval = new List<string>();
            int foundbraket = MethodType.IndexOf("(");
            int founddot = MethodType.IndexOf(".");

            if (foundbraket < 0) return null;
            if (founddot < 0) return null;
            string firstpart = MethodType.Substring(0, foundbraket);
            int lastdot = firstpart.LastIndexOf(".");
            string ClassName = firstpart.Substring(0, lastdot);
            string[] MethodParts = firstpart.Split('.');
            string BracketInfo = MethodType.Substring(foundbraket);
            BracketInfo = BracketInfo.Replace("(", "");
            BracketInfo = BracketInfo.Replace(")", "");
            String[] ParamInfo = BracketInfo.Split(',');
            String mname = MethodParts[MethodParts.Length - 1];
            int b2 = mname.IndexOf("(");
            retval.Add(ClassName);
            retval.Add(mname);
            for (int j = 0; j < ParamInfo.Length; j++) retval.Add(ParamInfo[j]);
            return retval.ToArray();
        }



        private Dictionary<string,string> orderParams(List<String> Parms)
        {
            Dictionary<string,string> retval = new Dictionary<string, string>();
            for (int i = 0; i < Parms.Count; i++)
            {
                string p = Parms[i];
                if (p.IndexOf("=") > 0)
                {
                    string[] parts = p.Split('=');
                    retval.Add(parts[0], parts[1]);
                }
                else
                    retval.Add(i.ToString(), p);
            }
            return retval;

        }
    }
}
