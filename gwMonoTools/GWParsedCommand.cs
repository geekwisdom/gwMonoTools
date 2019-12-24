using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace org.geekwisdom
{
    public class GWParsedCommand
    {
        private string Field;
        private string Operator;
        private string Value;

        public GWParsedCommand(string thecommand)
        {
            doParse(thecommand);
        }

        private void doParse(string thecommand) 
        {
            //actual parse
            string tmpField="";
            string tmpOp = "";
            string tmpValue="";
            string regExp = "\"(?:\\\\\\\\.|[^\\\\\\\\\"])*\"|\\S+";
            Regex rgx = new Regex(regExp);
            int i = 0;
            int bracketCount = 0;
            string cmdItem = "";
            foreach (Match match in rgx.Matches(thecommand))
            {
                //Console.WriteLine("Found '{0}' at position {1}",
                //        match.Value, match.Index);
                cmdItem = match.Value;
		  if (cmdItem == "[")
		  {
			  bracketCount++;
			  if (i != 0 ) throw new GWException("GWQL SYNTAX ERROR MISSING OPEN BRACKET [  "+ thecommand ,23);
          }
		  
		  else if (cmdItem == "]")
		  {
			  bracketCount++;
			  if (i != 3 ) throw new GWException("GWQL SYNTAX ERROR MISSING CLOSE BRACKET ] "+ thecommand ,23);
}
		  
		  else if ((cmdItem== "(" || cmdItem== ")" ) || cmdItem == "")
		  {
			  //ignore it
		  }
		  else
				{
				 
		   if (i == 0) tmpField=cmdItem;
		   if (i == 1) tmpOp = cmdItem;
		  if (i == 2) tmpValue = cmdItem;		  
		    i++;
				}
	
		    }

	 
	 if (bracketCount != 2) throw new GWException("GWQL SYNTAX ERROR MISSING BRACKETS "+ thecommand ,23);
	 if (i > 3) throw new GWException("GWQL SYNTAX ERROR at "+ cmdItem,23);
	 if (tmpField == "")  throw new GWException("GWQL SYNTAX ERROR MISSING FIELD IN " + thecommand,24);
	 if (tmpOp == "" ) throw new GWException("GWQL SYNTAX ERROR MISSING OPERATOR IN " + thecommand,25);
	 if (tmpValue == "" ) throw new GWException("GWQL SYNTAX ERROR MISSING VALUE IN " + thecommand,26);
     this.Field = tmpField;
     this.Operator = tmpOp;
     this.Value = tmpValue;
         //System.out.println(m.group());
         //allMatches.add(m.group());
}

        public string getField() { return Field; }
        public string getOperator() { return Operator; }
        public string getValue() { return Value; }
    }
}
