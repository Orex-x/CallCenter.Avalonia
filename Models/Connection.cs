using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaCallCenter.Models
{
    [DataContract]
    [Serializable]
    public class Connection
    {
        public int Id { get; set; }
        public string connectionID { get; set; }

        public string hostName { get; set; }
    }
}
