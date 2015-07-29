using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker_V2.Models
{
    public class Ticket
    {
        public Ticket()
        {
            this.Comments = new HashSet<TicketComment>();
            this.Attatchments = new HashSet<Attachment>();
        }

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set;}
        [AllowHtml]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public System.DateTimeOffset Created { get; set; }
        public Nullable<System.DateTimeOffset> Updated { get; set; }
        public int Status { get; set; }
        public int Priority { get; set; }
        public int Type { get; set; }
        public string CreatedBy { get; set; }


        public virtual Project Project { get; set; }

        public virtual ICollection<TicketComment> Comments { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }


    }
}