using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaCallCenter.Models
{

    public class User
    {
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string lastname { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int countCalls { get; set; }
        public int countTransferred{ get; set; }
        public int countBlocked{ get; set; }
    }
}
