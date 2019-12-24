using System;
using System.Collections.Generic;
using System.Text;

namespace org.geekwisdom
{
    public interface GWRowInterface
    {
        void Set(string key, string value);
        string Get(string key);

        Dictionary<string, string> ToArray();


        Dictionary<string, string> ToRawArray();

        //KeyValuePair<string, string> entry in item)
        List<KeyValuePair<string, string>> entrySet();
        

    }
}
