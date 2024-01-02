using System;
using System.Collections.Generic;

namespace Individuellt_databasprojekt123.Models
{
    public partial class staff
    {
        public int StaffId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? FkpositionId { get; set; }
        public int? FkdepartmentId { get; set; }
        public DateTime? StartDate { get; set; }
    }
}
