﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management_System.Models.Class
{
    public class LeaveHistory
    {
        [Key]
        public int leave_id { get; set; }
    }
}