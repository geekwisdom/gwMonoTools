/* *************************************************************************************
' Script Name: GWQL.CS
' **************************************************************************************
' @(#)    Purpose:
' @(#)    This is the GEEK WISDOM QUERY LANGUAGE. It is designed to be a language
' @(#)    independant query language for use with SQL, JSON, and the GeekWisdom
' @(#)    table object.
' **************************************************************************************
'  Written By: Brad Detchevery
			   2274 RTE 640, Hanwell NB
'
' Created:     2019-07-23 - Initial Architecture
' **************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace org.geekwisdom
{
    public class GWQL
    {
        private String Clause = "";
        private ArrayList Params;
        private Dictionary<string, string> allowedFields = new Dictionary<string, string>();
        private Dictionary<string, Boolean> cFlags = new Dictionary<string, Boolean>();


        public GWQL(string clause)
        {
            this.Clause = setFlags(clause);
            Params = new ArrayList();
        }

        public void setClause(String newclause)
        {
            this.Clause = setFlags(newclause);
        }

        private string setFlags(string clause_input)
        {
            //remove all flags from the last ] (if exist)
            int last = clause_input.LastIndexOf("]");
            if (last == -1) return clause_input;
            String retval = clause_input.Substring(0, last + 1);
            //System.out.println("Last is " + last);
            //System.out.println("Len is " + clause_input.length());
            if (last + 1 < clause_input.Length)
            {
                string flagsStr = clause_input.Substring(last + 2);
                //System.out.println(flagsStr);
                string[] flags = flagsStr.Split(' ');
                for (int i = 0; i < flags.Length; i++)
                {
                    bool v = true;
                    string flagname = flags[i];
                    if (flagname.Substring(0, 1) == "!")
                    {
                        v = false;
                        flagname = flagname.Substring(1, flagname.Length - 1);

                    }
                    cFlags.Add(flagname.Trim(), v);
                }
            }
            //System.out.println(retval);
            return retval;
        }

        public bool getFlag(String flagname)
        {
            if (!(cFlags.ContainsKey(flagname))) return false;
            else return cFlags[flagname];
        }

        public void setAllowedFields(Dictionary<String, String> Fields)
        {
            this.allowedFields = Fields;
        }

        public string getCommand(GWQLCmdBuilderInterface cmdObj) 
        {
            //parse through the clause each time calling the cmdObj BuildString method to 
            //swtch the final string together
            build_outer_string(this.Clause, cmdObj);
	        return cmdObj.getFinalCmd();
        }


        private bool build_outer_string(String inputstr, GWQLCmdBuilderInterface cmdObj)
        {
            //Reminder: $arry is substs
            bool retval=false;
            //$v = preg_split("/[\[\]]+/", $inputstr, -1, PREG_SPLIT_NO_EMPTY);

            String regExp = "[\\[\\]]+";
            String LHS="";
            String RHS="";
            String comp="";
            Regex rgx = new Regex(regExp);
            string equation_sides = "";
            int itemCount = -1;
            foreach (Match match in rgx.Matches(inputstr))
            {
                //Console.WriteLine("Found '{0}' at position {1}",
                //        match.Value, match.Index);
                equation_sides = match.Value.Trim();
                if ((equation_sides.Equals("[")  || equation_sides.Equals("]")  || equation_sides.Equals(" ") || equation_sides.Equals("(") || equation_sides.Equals(")") || equation_sides.Equals("")))
                {
            //do nothing IGNORE!
                }
		 else
		 {
			 itemCount++;
  		    if (itemCount == 0) LHS=equation_sides;
			if (itemCount == 1) comp=equation_sides;
			if (itemCount == 2) RHS=equation_sides;
		  }
}		   
		if (comp.Trim().Equals("_AND_"))
		{
		ArrayList LHS_SIDE = new ArrayList();
        ArrayList RHS_SIDE = new ArrayList();
        LHS_SIDE.Add("AND");
        rec_build_string(LHS, cmdObj, LHS_SIDE);
        rec_build_string(RHS, cmdObj, null);
		return true;
		}
	 
	 if (itemCount == -1) return rec_build_string(inputstr, cmdObj,null);
return retval;
}

private bool rec_build_string(string cmp, GWQLCmdBuilderInterface cmdObj, ArrayList substs) 
        {
            bool and_test = cmp.Contains(" _AND_ ");
	    if (!and_test)
	    {
    		bool or_test = cmp.Contains(" _OR_ ");
		    if (!or_test)
		    {
			cmdObj.buildString(cmp, substs, allowedFields);
			return true;
		}
		else
		{
                    string[] parts = cmp.Split(new[] { "_OR_" }, StringSplitOptions.RemoveEmptyEntries);
                    ArrayList the_or = new ArrayList();
                    the_or.Add("OR");
			        rec_build_string(parts[0], cmdObj, the_or);
                     rec_build_string(parts[1], cmdObj,null);
			         return true;
		}
	}
	else
	{
                string[] parts = cmp.Split(new[] { "_AND_" }, StringSplitOptions.RemoveEmptyEntries);
                ArrayList the_and = new ArrayList();
                the_and.Add("AND");
		        rec_build_string(parts[0], cmdObj, the_and);
                rec_build_string(parts[1], cmdObj,null);
		        return true;
	}	
//	return true;
}
        public ArrayList getParams()
        {
            return Params;
        }


    }



}
