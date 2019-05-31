using System;
using System.Collections.Generic;
using System.Text;

namespace org.geekwisdom.data
{
    public class GWDataRow
    {
    private Dictionary<string, string> dataItem;
public GWDataRow(Dictionary<string,string> i)
    {
        dataItem = i;
    }

    public void Set (string key,string value)
    {
        dataItem.Add(key, value);
    }

        public string Get(string key, string value)
        {
            string retval = dataItem[key];
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
