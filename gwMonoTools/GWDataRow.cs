/**************************************************************************************
' Script Name: GWDataRow.cs
' **************************************************************************************
' @(#)    Purpose:
' @(#)    This is a shared component available to all JAVA applications. It allows a common 
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
using System.Text;

namespace org.geekwisdom
{
    public class GWDataRow :GWRowInterface
    {
    private Dictionary<string, string> dataItem;
public GWDataRow(Dictionary<string,string> i)
    {
        dataItem = i;
    }

        public GWDataRow()
        {
            dataItem = new Dictionary<string, string>();
        }

        public void Set (string key,string value)
    {
        dataItem.Add(key, value);
    }

        public string Get(string key)
        {
            string retval = "";
            if (dataItem.ContainsKey(key))
                {
                retval = dataItem[key];
            }
        else
            {
                foreach (KeyValuePair<string, string> entry in dataItem)
                {
                    string ColName = entry.Key;
                    string[] cols = ColName.Split('.');
                    string shortname = cols[cols.Length - 1];
                    if (shortname == key) return entry.Value;
                }
                
               }
            return retval;
            }
        
       
    
        public Dictionary<string,string> ToArray()
        {
            return dataItem;
        }

        public Dictionary<string, string> ToRawArray()
        {
            return dataItem;
        }

        public bool has_column(string columnname)
        {
            return dataItem.ContainsKey(columnname);
        }


        //KeyValuePair<string, string> entry in item)
        public List<KeyValuePair<string,string>> entrySet()
        {
            //Set<Entry<String, String>>
            List<KeyValuePair<string,string>> retval = new List<KeyValuePair<string, string>>();
            foreach (KeyValuePair<string, string> entry in dataItem)
            {
                string ColName = entry.Key;
                
                String[] a = ColName.Split('.');
                if (a.Length > 0) ColName = a[a.Length - 1];
                String ColValue = entry.Value;
                KeyValuePair<string, string> k = new KeyValuePair<string, string>(ColName, ColValue);
                retval.Add(k);
                
            }
            return retval;
        }



    }
}
