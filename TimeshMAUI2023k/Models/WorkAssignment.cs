﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeshMAUI2023k.Models
{
    internal class WorkAssignment
    {
        public int IdWorkAssignment { get; set; }
        public int IdCustomer { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime? Deadline { get; set; }
        public bool InProgress { get; set; }
        public DateTime? WorkStartedAt { get; set; }
        public bool? Completed { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool Active { get; set; }
    }
}
