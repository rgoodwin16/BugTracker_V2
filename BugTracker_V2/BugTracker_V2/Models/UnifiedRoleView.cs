﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker_V2.Models
{
    public class UnifiedRoleView
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        
        public MultiSelectList AddedUsers { get; set; }
        public MultiSelectList DeletedUsers { get; set; }
        public MultiSelectList Users { get; set; }

        public string[] AddedSelect { get; set; }
        public string[] DeletedSelect { get; set; }
        public string[] Selected { get; set; }
        public Project Project { get; set; }
    }
}