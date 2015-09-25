using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker_V2.Models;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Text;
using System.Web.Security;



namespace BugTracker_V2.Controllers
{
    [RequireHttps]
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        [Route("Projects/{projectId}/Tickets")]
        public async Task<ActionResult> Index(int projectId)
        {
            ViewBag.ProjectId = projectId;
            var tickets = db.Tickets.Include(t => t.OwnedBy).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            return View(await tickets.ToListAsync());
        }

        // GET: Tickets/Details/5
        [BugTracker_V2.Models.CustomAttributes.ImportModelStateFromTempData]
        [Route("Projects/{projectId}/Tickets/{id}")]
        public async Task<ActionResult> Details(int projectId, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = await db.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
                        
            return View(ticket);
        }

        // GET: Tickets/Create
        [Route("Projects/{projectId}/Tickets/Create")]
        public ActionResult Create(int projectId)
        {

            ViewBag.ProjectId = projectId;
            
            var devId = db.Roles.First(r => r.Name == "Developer").Id;
            
            ViewBag.AssignedToId = new SelectList(db.Users.Where(u => u.Roles.Any(r => r.RoleId == devId)), "Id", "DisplayName");
                       
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name");

            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Projects/{projectId}/Tickets/Create")]
        public async Task<ActionResult> Create([Bind(Include = "Id,ProjectId,Title,Description,Created,Updated,TicketPriorityId,TicketStatusId,TicketTypeId,OwnedById,AssignedToId")] Ticket ticket, int projectId)
        {
            if (ModelState.IsValid)
            {
                if (ticket.TicketPriorityId == 0)
                {
                    ticket.TicketPriorityId = db.TicketPriority.First(tp => tp.Name == "Essential").Id;
                }

                if (ticket.TicketStatusId == 0)
                {
                    ticket.TicketStatusId = db.TicketStatus.First(ts => ts.Name == "New").Id;
                }


                var newUser = db.Users.Find(ticket.AssignedToId);

                ticket.OwnedById = User.Identity.GetUserId();
                ticket.Created = DateTimeOffset.Now;
                
                db.Tickets.Add(ticket);
                await db.SaveChangesAsync();

                if (newUser != null)
                {
                    var mailer = new EmailService();
                    var Notification = newUser != null ? new IdentityMessage()
                    {
                        Subject = "You have a new Notification",
                        Destination = newUser.Email,
                        Body = "You have been assigned the ticket:  " + ticket.Title + "!"
                    } : null;

                    if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
                    {
                        await mailer.SendAsync(Notification);
                    }
                }

                
                return RedirectToAction("Index","DashBoard");
            }

            var devId = db.Roles.First(r => r.Name == "Developer").Id;

            ViewBag.ProjectId = projectId;
            ViewBag.AssignedToId = new SelectList(db.Users.Where(u=> u.Roles.Any(r=> r.RoleId == devId)), "Id", "FirstName", ticket.OwnedById);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles=("Admin, ProjectManager,Developer"))]
        [Route("Projects/{projectId}/Tickets/{id}/Edit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = await db.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            var devId = db.Roles.First(r => r.Name == "Developer").Id;

            ticket.OwnedById = User.Identity.GetUserId();
            ViewBag.AssignedToId = new SelectList(db.Users.Where(u=> u.Roles.Any(r=> r.RoleId == devId)), "Id", "FirstName", ticket.OwnedById);

            ViewBag.OwnedById = new SelectList(db.Users, "Id", "FirstName", ticket.OwnedById);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ("Admin, ProjectManager,Developer"))]
        [Route("Projects/{projectId}/Tickets/{id}/Edit")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProjectId,Title,Description,Created,Updated,TicketPriorityId,TicketStatusId,TicketTypeId,AssignedToId, OwnedById")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var properties = new List<string>() { "Updated", "Description", "TicketTypeId","TicketStatusId" };
                var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

                var EditId = Guid.NewGuid().ToString();
                var UserId = User.Identity.GetUserId();


                if(User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
                {
                 
                    //Change TicketStatus from New to Assigned Automagically
                    //if (ticket.TicketStatusId == (db.TicketStatus.First(ts=> ts.Name == "New").Id) && ticket.AssignedToId == null)
                    //{
                    //    ticket.TicketStatusId = db.TicketStatus.FirstOrDefault(ts=> ts.Name == "Open").Id;
                    //}

                    //Check TicketPriority
                    if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                    {
                        var newUser = db.Users.Find(ticket.AssignedToId);

                        properties.Add("TicketPriorityId");

                        var ChangedPriority = new TicketHistory
                        {
                            TicketId = ticket.Id,
                            UserId = UserId,
                            Property = "Ticket Priority",
                            OldValue = db.TicketPriority.FirstOrDefault(p => p.Id == oldTicket.TicketPriorityId).Name,
                            NewValue = db.TicketPriority.FirstOrDefault(p => p.Id == ticket.TicketPriorityId).Name,
                            Changed = System.DateTimeOffset.Now,
                        };

                        db.TicketHistories.Add(ChangedPriority);

                        if (newUser != null)
                        {
                            var mailer = new EmailService();
                            var Notification = newUser != null ? new IdentityMessage()
                            {
                                Subject = "You have a new Notification",
                                Destination = newUser.Email,
                                Body = "The priority for your assigned ticket: " + ticket.Title + " has been changed.\n" + Environment.NewLine +
                                       " The new priority is: " + db.TicketPriority.FirstOrDefault(p => p.Id == ticket.TicketPriorityId).Name
                            } : null;

                            await mailer.SendAsync(Notification);
                        }
                    }

                    //Check if AssignedToId has changed
                    if (oldTicket.AssignedToId != ticket.AssignedToId)
                    {
                        
                        properties.Add("AssignedToId");
                        var newUser = db.Users.FirstOrDefault(u => u.Id == ticket.AssignedToId) ?? null;
                        var AssignedHistory = new TicketHistory
                        
                        {
                            TicketId = ticket.Id,
                            UserId = UserId,
                            Property = "Assigned To",
                            OldValue = (oldTicket.AssignedToId == null ? "Not Yet Assigned" : db.Users.FirstOrDefault(u => u.Id == oldTicket.AssignedToId).DisplayName),
                            NewValue = newUser != null ? newUser.DisplayName : "Unassigned",
                            Changed = System.DateTimeOffset.Now,
                        };

                        db.TicketHistories.Add(AssignedHistory);
                        
                        if (newUser != null)
                        {
                            var mailer = new EmailService();
                            var Notification = newUser != null ? new IdentityMessage()
                            {
                                Subject = "You have a new Notification",
                                Destination = newUser.Email,
                                Body = "You have been assigned the ticket:  " + ticket.Title + "!"
                            } : null;

                            
                            await mailer.SendAsync(Notification);
                        }

                    }

                    properties.AddRange(new string[] { "TicketPriorityId", "AssignedToId" });
                }


                //Check Description
                if (oldTicket.Description != ticket.Description)
                {

                    var newUser = db.Users.Find(ticket.AssignedToId);

                    var ChangedDescription = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        UserId = UserId,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = ticket.Description,
                        Changed = System.DateTimeOffset.Now,
                    };

                    db.TicketHistories.Add(ChangedDescription);

                    if (newUser != null)
                    {
                        var mailer = new EmailService();
                        var Notification = newUser != null ? new IdentityMessage()
                        {
                            Subject = "You have a new Notification",
                            Destination = newUser.Email,
                            Body = "The description for your assigned ticket: " + ticket.Title + " has been changed.  The new description is: " + ticket.Description
                        } : null;

                        
                        await mailer.SendAsync(Notification);
                    }

                }

                //Check TicketType
                if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    
                    var ChangedType = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        UserId = UserId,
                        Property = "Ticket Type",
                        OldValue = db.TicketType.FirstOrDefault(p => p.Id == oldTicket.TicketTypeId).Name,
                        NewValue = db.TicketType.FirstOrDefault(p => p.Id == ticket.TicketTypeId).Name,
                        Changed = System.DateTimeOffset.Now,
                    };
                    
                    db.TicketHistories.Add(ChangedType);

                }

                //Check TicketStatus
                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    var newUser = db.Users.Find(ticket.AssignedToId);
                    
                    properties.Add("TicketStatusId");
                    var ChangedStatus = new TicketHistory
                    {
                        TicketId = ticket.Id,
                        UserId = UserId,
                        Property = "Ticket Status",
                        OldValue = db.TicketStatus.FirstOrDefault(p => p.Id == oldTicket.TicketStatusId).Name,
                        NewValue = db.TicketStatus.FirstOrDefault(p => p.Id == ticket.TicketStatusId).Name,
                        Changed = System.DateTimeOffset.Now,
                    };

                    db.TicketHistories.Add(ChangedStatus);

                    if (newUser != null)
                    {
                        var mailer = new EmailService();
                        var Notification = newUser != null ? new IdentityMessage()
                        {
                            Subject = "You have a new Notification",
                            Destination = newUser.Email,
                            Body = "The priority for your assigned ticket: " + ticket.Title + " has been changed.\n" + Environment.NewLine +
                                   " The new priority is: " + db.TicketStatus.FirstOrDefault(p => p.Id == ticket.TicketStatusId).Name
                        } : null;


                        await mailer.SendAsync(Notification);
                    }

                }

                ticket.Updated = DateTimeOffset.Now;
                ticket.OwnedById = ticket.OwnedById;

                db.Update(ticket, properties.ToArray());
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { projectId = ticket.ProjectId, id = ticket.Id });
            }

            var devId = db.Roles.First(r => r.Name == "Developer").Id;

            ViewBag.AssignedToId = new SelectList(db.Users.Where(u => u.Roles.Any(r => r.RoleId == devId)), "Id", "FirstName", ticket.OwnedById);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriority, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatus, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketType, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = await db.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Ticket ticket = await db.Tickets.FindAsync(id);
            db.Tickets.Remove(ticket);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "DashBoard");
        }


        // ==============================================
           //COMMENTS - CREATE
        // ============================================== 

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Projects/{projectId}/Tickets/{ticketId}/CreateComment")]
        public async Task<ActionResult> CreateComment([Bind(Include = "Id,TicketId,Body")] TicketComment ticketComment, int projectId, int ticketId)
        {
            if (ModelState.IsValid)
            {
                ticketComment.Created = System.DateTimeOffset.Now;
                ticketComment.AuthorId = User.Identity.GetUserId();
                db.TicketComment.Add(ticketComment);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { projectId, id = ticketId });
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticketComment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            
            //return RedirectToAction("Details", new { projectId, id = ticketId });
            return Redirect(Url.Action("Details", new { projectId, id = ticketId }) + "#comments");
        }

        // ==============================================
            //ATTACHMENTS - CREATE
        // ============================================== 

        //POST: TicketAttchments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [BugTracker_V2.Models.CustomAttributes.ExportModelStateToTempData]
        [Route("Projects/{projectId}/Tickets/{ticketId}/AddAttachment")]
        public async Task<ActionResult> AddAttachment([Bind(Include="Id,TicketId,Description,MediaURL,AuthorId,Title")] TicketAttachment ticketAttachment, HttpPostedFileBase file, int projectId, int ticketId)
        {
            //Check if the file selected by the user isn't empty
            if (file != null && file.ContentLength > 0)
            {
                //check the file ext to make sure we allow it
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".pdf" && ext != ".doc" && ext != ".docx" && ext != ".ppt" && ext != ".xls" && ext != ".xlsx" && ext != ".zip" && ext != ".txt")
                {
                    ModelState.AddModelError("file", "Only .PNG, .JPG, .JPEG, .GIF, .PDF, .DOC, .PPT, .XLS, .XLSX, .ZIP or .TXT files are allowed.");
                }
                else
                {
                    ticketAttachment.Type = ext;
                }
            }

            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    //relative path
                    var filePath = "/Uploads/";
                    //path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    Directory.CreateDirectory(absPath);
                    //media url for relative path
                    ticketAttachment.MediaURL = filePath + file.FileName;
                    //save file
                    file.SaveAs(Path.Combine(absPath, file.FileName));
                }

                ticketAttachment.Created = DateTimeOffset.Now;
                ticketAttachment.AuthorId = User.Identity.GetUserId();
                ticketAttachment.Type = ticketAttachment.Type;

                if (ticketAttachment.Title == null)
                {
                    ticketAttachment.Title = "(No Title)";
                }
                else
                {
                    ticketAttachment.Title = ticketAttachment.Title;
                }

                db.TicketAttchment.Add(ticketAttachment);
                await db.SaveChangesAsync();

                //return RedirectToAction("Details", new { projectId, id = ticketId });
                return Redirect(Url.Action("Details", new { projectId, id = ticketId }) + "#attachments");
            }

            //return RedirectToAction("Details", new {projectId, id = ticketId});
            return Redirect(Url.Action("Details", new { projectId, id = ticketId }) + "#attachments");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        

    }
}
