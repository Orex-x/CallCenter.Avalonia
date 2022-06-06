using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaCallCenter.Models
{
    public enum Status
    {
        NEW,
        NO_ANSWER,
        NO_DATE,
        CONFIRMED,
        TRANSFERRED,
        BLOCKED
    }

    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public Status Status { get; set; }
        public string Description { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<Event> Events { get; set; }
    }
}
