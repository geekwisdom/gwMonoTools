/**************************************************************************************
' Script Name: GWDataTable.cs
' **************************************************************************************
' @(#)    Purpose:
' @(#)    This is a shared component available to all .NET (C#) applications. It allows a common 
' @(#)    data row / data table object that for manipulating sets of related data abstractly.
' @(#)    Regardless of the specific architecture (database, files, xml, json used)
' **************************************************************************************
'  Written By: Brad Detchevery
' Created:     2019-05-29 - Initial Architecture
' 
' **************************************************************************************
'Note: Changing this routine effects all programs that manipulate data sets
'-------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;

namespace org.geekwisdom
{
    public class GWDataTable
    {
        private List<GWRowInterface> data = new List<GWRowInterface>();
        string xml = "";
        string tablename = "table1";
        string defObject = "org.geekwisdom.GWDataRow";
        private Dictionary<String, String> parsedMap = new Dictionary<String, String>();

        public GWDataTable()
        {

        }

        public GWDataTable(string xmlinfo, string TableName, string _defObject)
        {
            xml = xmlinfo;
            if (TableName == "") tablename = "root";
            else tablename = TableName;
            defObject = _defObject;
        }

        public GWDataTable(string xmlinfo, string TableName)
        {
            xml = xmlinfo;
            if (TableName == "") tablename = "root";
            else tablename = TableName;
        }

        public GWDataTable find(string whereclause)
        {
            return find(whereclause, defObject);
        }

        public GWDataTable find(string qlwhereclause,string _defObj)
        {
            GWQL xPathTester = new GWQL(qlwhereclause);
            GWQLXPathBuilder myxpath = new GWQLXPathBuilder();
            string whereclause = "";
            try
            {
                whereclause = xPathTester.getCommand(myxpath);
            }
            catch (GWException e) { throw e; }
            string xmldata = toXml();
            //System.out.println(xmldata);
            GWDataTable retTable = new GWDataTable("", tablename);
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmldata);
                string qry = "//xmlDS/" + tablename + "[" + whereclause + "]";
                XmlNodeList nodes = doc.DocumentElement.SelectNodes(qry);

                //System.out.println("Length is: " + nodes.getLength());
                foreach (XmlNode xn in nodes)
                {
                    Dictionary<string, string> theitem = new Dictionary<string, string>();
                    //string firstName = xn["FirstName"].InnerText;
                    //string lastName = xn["LastName"].InnerText;
                    XmlNodeList children = xn.ChildNodes;
                    foreach (XmlNode thechild in children)
                    {
                        //thechild.Name;
                        //thechild.InnerText;
                        theitem.Add(tablename + "." + thechild.Name, thechild.InnerText);
                    }
                    if (theitem.Count > 0)
                    {

                        Type type = Type.GetType(_defObj);
                        if (type == null)
                        {
                            System.Reflection.Assembly currentAssem = System.Reflection.Assembly.GetEntryAssembly();
                            Type CorrectType = null;
                            foreach (Type t1 in currentAssem.GetTypes())
                            {
                                if (t1.Name == _defObj)
                                    CorrectType = t1;

                            }
                            type = CorrectType;

                        }
                        if (type != null)
                        {
                             
                            GWRowInterface therow = (GWRowInterface) Activator.CreateInstance(type,theitem);
                            retTable.Add(therow);
                        }
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
            return retTable;
        }

        public void loadXml(string xmlstring)
        {
            //read xml file add to array
            ReadXml(xmlstring);
        }

        private void ReadXml(string XMLInput)
        {
            string XMLstring = XMLInput.Replace("&", "%26");
            //    XMLstring =XMLString.replaceAll( "(<\\?[^<]*\\?>)?", "");
            if (XMLstring.IndexOf("<soap:") > 0)
                {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(XMLstring);

                var soapBody = xmlDocument.GetElementsByTagName("soap:Body")[0];
                XMLstring = soapBody.InnerXml;
            }
            //  System.out.println(XMLString);
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(XMLstring);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.StackTrace);
                throw e;
            }
            XmlElement root = doc.DocumentElement;
            XmlNode firstchild = root.FirstChild.NextSibling;
            tablename = firstchild.Name;
            GWDataRow newrow = new GWDataRow(readnode(root, "", ""));
            data.Add(newrow);

            // data.add(item);
        }

        private Dictionary<String, String> readnode(XmlNode node, string buildedKey, string lastkey)
        {
            string nodeName = node.Name;
            //   int thislength=0;
            bool newone = false;
            if (nodeName.ToLower() != "#text")
            {
                //buildedKey = nodeName;
                String[] a = buildedKey.Split('.');
                if (a.Length  == 2)
                {
                    //System.out.println("Added hashmap to array!" + parsedMap.get("company.staff.firstname"));
                    if (parsedMap.Count > 0)
                    {
                        GWDataRow newrow = new GWDataRow(parsedMap);
                        data.Add(newrow);
                        parsedMap = new Dictionary<string, string>();
                        newone = false;
                    }
                    //    parsedMap.put(buildedKey,node.getNodeValue());

                }
                //  System.out.println ("B  is " + buildedKey + " " + a.length);
                lastkey = buildedKey;
                buildedKey += "." + nodeName;
            }
            //else nodeName = "";
            if (node.Value != null)
            {
                buildedKey = buildedKey.Substring(1);
                string v = node.Value.Replace("%26", "&");
                 parsedMap.Add(buildedKey, v);
                //     System.out.println(newone);

                //        System.out.println ("This key: " + buildedKey + " Last: " + lastkey);
                //	System.out.println("Adding to parsedmap");

                

            }

            if (node.NodeType == XmlNodeType.Element)
            {
                if (node.Attributes.Count > 0)
                {
                    XmlNamedNodeMap startAttr = node.Attributes;
                    for (int i = 0; i < startAttr.Count; i++)
                    {
                        XmlNode attr = startAttr.Item(i);
                        // buildedKey += attr.getNodeValue() + ".";
                    }
                }
            }
            int c = 0;
            XmlNodeList children = node.ChildNodes;
            foreach (XmlNode child in children)
            {
                c++;
                readnode(child, buildedKey, lastkey);
            }
            return parsedMap;
        }

        public GWRowInterface GetRow(int RowNum)
        {
            return data[RowNum];
        }

        public void Add(GWRowInterface newrow)
        {
            data.Add(newrow);
        }

        public void Add(Dictionary<string,string> item)
        {
            Dictionary<string, string> newitem = new Dictionary<string, string>();
            
            foreach (KeyValuePair<string, string> entry in item)
            {
                string ColName = entry.Key;
                string ColValue = entry.Value;
                if (entry.Key.IndexOf(".") < 0) ColName = tablename + "." + entry.Key;

                newitem.Add(ColName, ColValue);
            }
            GWDataRow therow = new GWDataRow(newitem);

            data.Add(therow);
        }

        public int length()
        {
            return data.Count;
        }

        private bool validatearray(GWDataRow row)
        {
            return true;
        }

        private string repeat(string pattern, int count)
        {
            string retval = "";
            for (int i = 0; i < count; i++)
            {
                retval += pattern;
            }
            return retval;
        }

        public void Remove(int rownum)
        {
            //remove row num from table	
            
            data.RemoveAt(rownum);
        }

        public string toJSON()
        {

        List<Dictionary<String, String>> jsonData = new List<Dictionary<String,String>>();
            for (int i=0;i<data.Count;i++)
            {
                Dictionary<String, String> item = data[i].ToRawArray();
                Dictionary<String, String> newitem = new Dictionary<string, string>();
                foreach (var entry in item)
                {
                    string Value = entry.Value;
                    string key = entry.Key;
                    string truekey = key;
                    if (key.IndexOf(".") >=0)
                    {
                        string [] parts = key.Split('.');
                        
                        string TableNameTest = parts[0];
                        if (this.tablename != TableNameTest) TableNameTest = parts[1];
                        if (this.tablename == TableNameTest)
                        {
                            truekey = parts[parts.Length-1];
                            if (Value != null)
                                Value = Value.Replace("&", "&26");
                            newitem.Add(truekey, Value);
                        }
                        else
                        {
                            newitem.Add(truekey, null);
                        }

                        }
                    }

                jsonData.Add(newitem);
            }


            Dictionary<string, object> root = new Dictionary<string, object>();
            root.Add(tablename, jsonData);
            string json = JsonConvert.SerializeObject(root, Newtonsoft.Json.Formatting.None);
            
            return json;
        }

        public string toXml()
        {
            System.IO.StringWriter retval = new StringWriter();
            WriteXml(retval);
            return retval.ToString();
        }
        private void WriteXml(StringWriter sw)
        {
            WriteXml(sw, "");
        }
        public void WriteXml(StringWriter finalsw, string nodename)
        {
            int colstart = 0;
            if (nodename == "") nodename = "xmlDS";
            else colstart = 1;
            StringWriter sw = new StringWriter();
            for (int i = 0; i < data.Count; i++)
            {
                GWRowInterface col = data[i];
                Dictionary<string, string> item = col.ToArray();
                string header = "";
                string footer = "";
                string Retval = "";
                
               foreach (KeyValuePair<string, string> entry in item)
                    {
                    string key = entry.Key;
                    string value = entry    .Value;
                    if (value != null)
                    {
                        value = value.ToString().Replace("&", "%26");

                        String[] a = key.Split('.');
                        if (header == "")
                        {
                            for (int j = colstart; j < a.Length - 1; j++)
                            {
                                string tabs = repeat(" ", j);
                                if (!(a[j] == nodename )) header = header + tabs + "<" + a[j] + ">\n";
                            }
                        }

                        if (footer == "")
                        {
                            for (int j = a.Length - 2; j >= colstart; j--)
                            {
                                // int count=-1 * j + a.length;
                                int count = j;
                                string tabs = repeat(" ", count);
                                if (!(a[j] == nodename)) footer = footer + tabs + "</" + a[j] + ">\n";
                            }
                        }
                        Retval = Retval + repeat(" ", a.Length) + "<" + a[a.Length - 1] + ">" + value + "</" + a[a.Length - 1] + ">\n";
                    }
                    //    System.out.println(key + " " + value);  
                }
               
                sw.Write(header);
                sw.Write(Retval);
                sw.Write(footer);
            }
            finalsw.Write("<?xml version=\"1.0\"?>\n");
            finalsw.Write("<" + nodename + ">\n");
            finalsw.Write(sw.ToString());
            finalsw.Write("</" + nodename + ">\n");
        }

    }
}
