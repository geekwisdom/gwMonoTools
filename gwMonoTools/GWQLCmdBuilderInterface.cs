using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

namespace org.geekwisdom
{
    public interface GWQLCmdBuilderInterface
    {
        void buildString(string inputstr, ArrayList substs, Dictionary<String, String> allowedFields);
        string getFinalCmd();
    }
}
