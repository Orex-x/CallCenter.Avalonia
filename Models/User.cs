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
        public string name { get; set; }
        public string login { get; set; }
        public string password { get; set; }
    }
}
