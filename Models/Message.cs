﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaCallCenter.Models
{
    public class Message
    {
        public string Title{ get; set; }

        public string Author { get; set; }

        public DateTime Created { get; set; }
    }
}
