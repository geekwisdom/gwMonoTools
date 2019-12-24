using System;
using System.Collections.Generic;
using Newtonsoft.Json;

using System.Text;

namespace org.geekwisdom
{
    public class GWDataIO:GWDataIOInterface
    {
        protected GWDataTable dataTable;
        protected String _configFile;
        protected String defaultObj = "org.geekwisdom.GWDataRow";

        public GWDataIO(string configfile,string defObj)
        {
            _configFile = configfile;
            defaultObj = defObj;
        }

        public GWDataIO(string configfile)
        {
            _configFile = configfile;
        }

        public GWDataIO()
        {
            //hmm what happens here?
        }

        
        public GWDataIOInterface getInstance(string configfile)
        {
            GWSettings settingsManger = new GWSettings();
            string objType = settingsManger.GetSetting(configfile, "IOTYPE", "test", "");
            try
            {
                Type t = Type.GetType(objType);
                GWDataIOInterface retval = (GWDataIOInterface)Activator.CreateInstance(t,configfile,defaultObj);
                return retval;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return null;
        }
        public GWDataIOInterface getInstance()
        {
            return getInstance(_configFile);
        }
        public virtual string Insert(string JSONROW, string configfile)
        {
            
            return getInstance(configfile).Insert(JSONROW);
        }
        public virtual string Insert(string JSONROW)
        {
            return Insert(JSONROW, _configFile);
        }
        public virtual string Update(string JSONROW, string configfile)
        {
            return getInstance(configfile).Update(JSONROW);
        }
        public virtual string Update(string JSONROW)
        {
            return Update(JSONROW, _configFile);
        }
        public virtual GWDataTable Search(string whereclause, string configfile)
        {
            return getInstance(configfile).Search(whereclause);
        }
        public virtual GWDataTable Search(string whereclause)
        {
            return Search(whereclause, _configFile);
        }
        public virtual string Delete(string id, string configfile)
        {
            return getInstance(configfile).Delete(id);
        }
        public virtual string Delete(string id)
        {
            return Delete(id, _configFile);
        }
        public virtual string Lock(string id, string configfile)
        {
            return getInstance(configfile).Lock(id);
        }
        public virtual string Lock(string id)
        {
            return Lock(id, _configFile);
        }
        public virtual string Unlock(string id, string configfile)
        {
            return getInstance(configfile).Unlock(id);
        }
        public virtual string Unlock(string id)
        {
            return Unlock(id, _configFile);
        }
        public virtual void Open(string configfile)
        {
            getInstance(configfile).Open(configfile);
            return;
        }
        public virtual void Open()
        {
            Open(_configFile);
            return;
        }
        public virtual void Save(string configfile)
        {
            getInstance(configfile).Save(configfile);
            return;
        }
        public virtual void Save()
        {
            Save(_configFile);
            return;
        }

        protected Dictionary<string,string> translate (string InputJSON)
        {
            Dictionary<string,string> obj = JsonConvert.DeserializeObject<Dictionary<string, string>>(InputJSON);
            return obj;
        }

    }
}
