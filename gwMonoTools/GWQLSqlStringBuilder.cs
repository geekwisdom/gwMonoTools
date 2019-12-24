using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace org.geekwisdom
{
    public class GWQLSqlStringBuilder : GWQLCmdBuilderInterface
    {
        Dictionary<string, string> mymap = new Dictionary<string, string>();
        ArrayList Params = new ArrayList();
        string commandPart = "";
        string runningString = "";
        public GWQLSqlStringBuilder()
        {

            mymap.Add("OR", "OR");
            mymap.Add("AND", "AND");
            mymap.Add("OPENBRACKET", "(");
            mymap.Add("CLOSEBRACKET", ")");


        }

        public void buildString(string inputstr, ArrayList substs, Dictionary<string, string> allowedFields)
        {


            string OP = "";
            bool b1 = inputstr.Contains("[");
            bool b2 = inputstr.Contains("]");
            if (!(b1 || b2)) throw new GWException("ERROR: MISSING OPENING/CLOSING BRACKET", 99);

            string newstring = inputstr.Replace("[", "");
            newstring = newstring.Replace("]", "");
            string bracketstring = "[ " + newstring + " ]";
            GWParsedCommand mycmd = new GWParsedCommand(bracketstring);
            if (mycmd.getOperator().Equals("_EQ_")) OP = "=";
            if (mycmd.getOperator().Equals("_LT_")) OP = "<";
            if (mycmd.getOperator().Equals("_LE_")) OP = "<=";
            if (mycmd.getOperator().Equals("_GT_")) OP = ">";
            if (mycmd.getOperator().Equals("_GE_")) OP = ">=";
            if (mycmd.getOperator().Equals("_NE_")) OP = "!=";
            if (mycmd.getOperator().Equals("_LIKE_")) OP = " LIKE ";
            if (OP.Equals("")) throw new GWException("ERROR: MISSING/INCORRECT OPERATOR", 98);
            String value = mycmd.getValue().Replace("\"", "");
            value = value.Replace("'", "");

            String field = mycmd.getField();
            if (allowedFields.Count > 0)
            {
                if (allowedFields.ContainsKey(field)) field = allowedFields[field];
                else { throw new GWException("INVALID FIELD: " + field, 95); }
            }

            string part = field + OP + "?";
			Params.Add(value);
            string replace = inputstr.Replace(newstring.Trim(), part);
            runningString = runningString + replace;
            runningString.Replace(newstring, part);
            //remember rplace value with ? and add value to params!
            commandPart = commandPart + part;
            if (substs != null) for (int i = 0; i < substs.Count; i++) runningString = runningString + " " + mymap[substs[i].ToString()] + " ";



            //commandPart =commandPart + " " + inputstr;
            return;

        }
        public ArrayList getParams()
        {
            // TODO Auto-generated method stub
            return Params;
        }

        public string getFinalCmd()
        {
            commandPart = runningString.Replace("[", mymap["OPENBRACKET"]);
            commandPart = commandPart.Replace("]", mymap["CLOSEBRACKET"]);
            if (commandPart.Substring(0, 1).Equals("(")) commandPart = commandPart.Substring(1);
            if (commandPart.Substring(commandPart.Length - 1, 1).Equals(")")) commandPart = commandPart.Substring(0, commandPart.Length - 2);
            return commandPart;

        }
    }
}
