using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.geekwisdom;
namespace gwUnitTests
{
    class Student:GWDataRow
    {
        public Student(Dictionary<string,string> i) : base(i)
        {
            
        }
        public string getName()
        {
            return base.Get("Student.Name");
        }
    }
}
