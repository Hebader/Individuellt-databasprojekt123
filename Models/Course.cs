using System;
using System.Collections.Generic;

namespace Individuellt_databasprojekt123.Models
{
    public partial class Course
    {
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public int? FkstaffId { get; set; }
        public bool? IsActive { get; set; }
    }
}
