using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TaskBoard.Server.Services;

namespace TaskBoard.ServerTest
{
    internal class TestTimeProvider:ITimeProvider
    {
        DateTime Now;
        public DateTime UtcNow()
        {
            return Now;
        }
        public void SetTime(DateTime dateTime)
        {
            Now = dateTime;
        }
    }
}
