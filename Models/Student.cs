using System;
using System.Collections.Generic;

namespace Individuellt_databasprojekt123.Models
{
    public partial class Student
    {
        public int StudentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? FkclassId { get; set; }
        public string? BirthDate { get; set; }
    }
}
