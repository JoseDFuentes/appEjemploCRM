using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace appEjemploCRM
{
    public class Contact
    {

        public string GUID { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fax { get; set; }
        public int familystatuscode { get; set; }
        public decimal creditlimit { get; set; }
        public bool creditonhold { get; set; }
        public DateTime birthdate { get; set; }
        public string jobphone { get; set; }
        public string email { get; set; }
    }
}
