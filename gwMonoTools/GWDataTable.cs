using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace org.geekwisdom.data
{
    public class GWDataTable
    {
        private List<GWDataRow> data = new List<GWDataRow>();
        string xml = "";
        String tablename = "table1";
        private Dictionary<String, String> parsedMap = new Dictionary<String, String>();

        public GWDataTable()
        {

        }

        public GWDataTable(String xmlinfo, String TableName)
        {
            xml = xmlinfo;
            if (TableName == "") tablename = "root";
            else tablename = TableName;
        }

        public GWDataTable find(String whereclause)
        {
            String xmldata = toXml();
            //System.out.println(xmldata);
            GWDataTable retTable = new GWDataTable("", tablename);
            try
            {

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmldata);
                String qry = "//xmlDS/" + tablename + "[" + whereclause + "]";
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
                        theitem.Add(thechild.Name, thechild.InnerText);
                    }
                    if (theitem.Count > 0)
                    {
                        GWDataRow therow = new GWDataRow(theitem);
                        retTable.Add(therow);
                    }
                }
            }
            catch (Exception e) { Console.WriteLine(e.StackTrace); }
            return retTable;
        }

        public void loadXml(String xmlstring)
        {
            //read xml file add to array
            ReadXml(xmlstring);
        }

        private void ReadXml(String XMLInput)
        {
            String XMLString = XMLInput.Replace("&", "%26");
            //    XMLString =XMLString.replaceAll( "(<\\?[^<]*\\?>)?", "");
            if (XMLString.IndexOf("<soap:") > 0)
                {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(XMLString);

                var soapBody = xmlDocument.GetElementsByTagName("soap:Body")[0];
                XMLString = soapBody.InnerXml;
            }
            //  System.out.println(XMLString);
            XmlDocument doc = new XmlDocument();

            try
            {
                doc.LoadXml(XMLString);
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.StackTrace);
                throw e;
            }
            XmlElement root = doc.DocumentElement;
            tablename = root.Name;
            GWDataRow newrow = new GWDataRow(readnode(root, "", ""));
            data.Add(newrow);

            // data.add(item);
        }

        private Dictionary<String, String> readnode(XmlNode node, String buildedKey, String lastkey)
        {
            String nodeName = node.Name;
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

        public GWDataRow GetRow(int RowNum)
        {
            return data[RowNum];
        }

        public void Add(GWDataRow newrow)
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
        public void WriteXml(StringWriter finalsw, String nodename)
        {
            int colstart = 0;
            if (nodename == "") nodename = "xmlDS";
            else colstart = 1;
            StringWriter sw = new StringWriter();
            for (int i = 0; i < data.Count; i++)
            {
                GWDataRow col = data[i];
                Dictionary<string, string> item = col.ToArray();
                string header = "";
                string footer = "";
                string Retval = "";
                
               foreach (KeyValuePair<string, string> entry in item)
                    {
                    string key = entry.Key;
                    string value = entry.Value;
                    if (value != null)
                    {
                        value = value.ToString().Replace("&", "%26");

                        String[] a = key.Split('.');
                        if (header == "")
                        {
                            for (int j = colstart; j < a.Length - 1; j++)
                            {
                                String tabs = repeat(" ", j);
                                if (!(a[j] == nodename )) header = header + tabs + "<" + a[j] + ">\n";
                            }
                        }

                        if (footer == "")
                        {
                            for (int j = a.Length - 2; j >= colstart; j--)
                            {
                                // int count=-1 * j + a.length;
                                int count = j;
                                String tabs = repeat(" ", count);
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
