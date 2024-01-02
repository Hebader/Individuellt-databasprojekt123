using System;
using System.Collections.Generic;

namespace Individuellt_databasprojekt123.Models
{
    public partial class Grade
    {
        public int GradeId { get; set; }
        public string? Teacher { get; set; }
        public string? Date { get; set; }
        public string? Grade1 { get; set; }
        public int? FkcourseId { get; set; }
        public int? FkstudentId { get; set; }
    }
}
