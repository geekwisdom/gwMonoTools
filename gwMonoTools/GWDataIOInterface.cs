/* *************************************************************************************
' Script Name: GWDataIOInterface.cs
' **************************************************************************************
' @(#)    Purpose:
' @(#)    This is a shared component available to all JAVA applications. It allows a common 
' @(#)    object that can CREATE (INSERT), RETRIEVE (SEARCH) UPDATE, AND DELETE from a variety of IO
' @(#)    sources. Specifically using the GWDataTable / GWDataRow format
' **************************************************************************************
'  Written By: Brad Detchevery
			   2274 RTE 640, Hanwell NB
'
' Created:     2019-07-23 - Initial Architecture
' 
' **************************************************************************************
'Note: GWDataIOInterface is the interface for GWDataIO. The actual heavy lifting is done by FileIO or 
'DataBaseIO which extend this class for the specific IO ability
'getInstance(FlieName) to return the approperiate type from the file. 
'This class defines those protected methods common to all extended children
' **************************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;

namespace org.geekwisdom
{
    public interface GWDataIOInterface
    {
        GWDataIOInterface getInstance(string configfile);
        GWDataIOInterface getInstance();
        string Insert(string JSONROW, string configfile);
        string Insert(string JSONROW);
        string Update(string JSONROW, string configfile);
        string Update(string JSONROW);
        GWDataTable Search(string whereclause, string configfile);
        GWDataTable Search(string whereclause);
        string Delete(string id, string configfile);
        string Delete(string id);
        string Lock(string id, string configfile);
	    string Lock(string id);
	    string Unlock(string id, string configfile);
        string Unlock(string id);
        void Open(string configfile);
        void Open();
        void Save(string configfile);
        void Save();
    }
}
