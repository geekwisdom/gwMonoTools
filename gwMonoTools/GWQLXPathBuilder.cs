using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace org.geekwisdom
{
    class GWQLXPathBuilder : GWQLCmdBuilderInterface
    {
        private Dictionary<string, string> mymap = new Dictionary<string, string>();
        private ArrayList Params;
        private string commandPart = "";
        private string runningString = "";

        public GWQLXPathBuilder()
        {
            mymap.Add("OR", "or");
            mymap.Add("AND", "and");
            mymap.Add("OPENBRACKET", "(");
            mymap.Add("CLOSEBRACKET", ")");
            Params = new ArrayList();
        }

        public void buildString(string inputstr, ArrayList substs, Dictionary<string, string> allowedFields)
        {
            Boolean b1 = inputstr.Contains("[");
            Boolean b2 = inputstr.Contains("]");
            if (!(b1 || b2)) throw new GWException("ERROR: MISSING OPENING/CLOSING BRACKET", 99);
            string OP = "";
            string newstring = inputstr.Replace("[", "");
            newstring = newstring.Replace("]", "");
            string bracketstring = "[ " + newstring + " ]";

            GWParsedCommand mycmd = new GWParsedCommand(bracketstring);
            if (mycmd.getOperator() == "_EQ_")  OP = "=";
            if (mycmd.getOperator() == "_LT_") OP = "<";
            if (mycmd.getOperator() == "_LE_") OP = "<=";
            if (mycmd.getOperator() == "_GT_") OP = ">";
            if (mycmd.getOperator() == "_GE_") OP = ">=";
            if (mycmd.getOperator() == "_NE_") OP = "!=";
            if (mycmd.getOperator() == "_LIKE_") OP = "CONTAINS";
            if (OP == "") throw new GWException("ERROR: MISSING/INCORRECT OPERATOR", 98);
            string value = mycmd.getValue().Trim();
            string part = "";
            string field = mycmd.getField();
            if (allowedFields.Count > 0)
            {
                if (allowedFields.ContainsKey(field)) field = allowedFields[field];
                else { throw new GWException("INVALID FIELD: " + field, 95); }
            }

            if (OP == "CONTAINS")
            {
                part = "contains(" + field + "," + value + ")";
            }
            else
                part = field + OP + value;
            string replace = inputstr.Replace(newstring.Trim(), part);
            runningString = runningString + replace;
            runningString.Replace(newstring, part);
            //remember rplace value with ? and add value to params!
            commandPart = commandPart + part;
            if (substs != null) for (int i = 0; i < substs.Count; i++) runningString = runningString + " " + mymap[substs[i].ToString()]+ " ";
        }

        public ArrayList getParams()
        {
            // TODO Auto-generated method stub
            return Params;
        }

        public string getFinalCmd()
        {
            commandPart = runningString.Trim();
            if (commandPart.Substring(0, 1).Equals("[")) commandPart = commandPart.Substring(1);
            if (commandPart.Substring(commandPart.Length - 1, 1).Equals("]")) commandPart = commandPart.Substring(0, commandPart.Length - 2);
            //System.out.println(commandPart);
            return commandPart;
        }
    }
}
