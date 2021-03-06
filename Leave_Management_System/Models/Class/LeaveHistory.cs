﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management_System.Models.Class
{
    public class LeaveHistory
    {
        [Key]
        public int leave_id { get; set; }


        public string LeaveStatus { get; set; }
        public string HODApproveStatus { get; set; }
        public string DeanApproveStatus { get; set; }
        public string RegistrarApproveStatus { get; set; }
        [DataType(DataType.Text)]
        public int NoOfDay  { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Leave Reason")]
        public string LeaveReason { get; set;}
        [DataType(DataType.Date)]
        [Display(Name = "Leave Start Frome")]
        public DateTime StartFrome { get; set;}
        [DataType(DataType.Date)]
        [Display(Name = "Leave End Till")]
        public DateTime EndTill { get; set; }



        public int Registrar_id { get; set; }
        [ForeignKey("Registrar_id")]
        public Registrar ragistrar { get; set; }

        public int HOD_id { get; set; }
        [ForeignKey("HOD_id")]
        public HOD hod { get; set; }

        public int Faculty_id { get; set; }
        [ForeignKey("Faculty_id")]
        public Faculty faculty { get; set; }

        public int Dean_id { get; set; }
        [ForeignKey("Dean_id")]
        public Dean dean { get; set; }



    }
}
