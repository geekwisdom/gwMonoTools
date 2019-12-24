using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace org.geekwisdom
{
    class GWDataFileIO:GWDataIO
    {
        public GWDataFileIO(string configflie,string defObj):base(configflie,defObj)
        {

        }
        public override string Insert(string jsonrow,string configfile)
        {
            Dictionary<string, string> rowobj = base.translate(jsonrow);
            if (dataTable == null) loadData(configfile);
            dataTable.Add(rowobj);
            saveData(configfile);
            return "SUCCESS";
        }

        public override GWDataTable Search(string whereclause,string configfile)
        {
            loadData(configfile);
            GWDataTable ret = dataTable.find(whereclause);
            return ret;

        }

        private void loadData(string configfile)
        {
            GWSettings settingsManager = new GWSettings();
            string configFile = settingsManager.GetSetting(configfile, "connectionInfo", "");
            if (File.Exists(configFile))
            {
                string xmlData = File.ReadAllText(configFile);
                dataTable = new GWDataTable("", "root", defaultObj);
                dataTable.loadXml(xmlData);
            }
            return;
        }
        private void saveData(string configfile)
        {
            GWSettings settingsManager = new GWSettings();
            string configFile = settingsManager.GetSetting(configfile, "connectionInfo", "");
            string xmloutput = dataTable.toXml();
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(configFile))
            {
                file.WriteLine(xmloutput);
            }
        }
    }
}
