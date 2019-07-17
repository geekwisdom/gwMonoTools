using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.geekwisdom;

namespace gwUnitTests
{
    class StudentService : GWDataIO
    {
        public StudentService() : base("/tmp/DataIOTest.config", "Student")
        {
        }
    }
}
