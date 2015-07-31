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

namespace BugTracker_V2.Controllers
{
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
            var devId = db.Roles.First(r => r.Name == "Developer").Id;

            ViewBag.ProjectId = projectId;
            ViewBag.AssignedToId = new SelectList(db.Users.Where(u => u.Roles.Any(r => r.RoleId == devId)), "Id", "FirstName");

            //ViewBag.OwnedById = new SelectList(db.ApplicationUsers, "Id", "FirstName");
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
        public async Task<ActionResult> Create([Bind(Include = "Id,ProjectId,Title,Description,Created,Updated,TicketPriorityId,TicketStatusId,TicketTypeId,OwnedById")] Ticket ticket, int projectId)
        {
            if (ModelState.IsValid)
            {
                ticket.OwnedById = User.Identity.GetUserId();
                ticket.Created = DateTimeOffset.Now;
                
                db.Tickets.Add(ticket);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
        [Route("Projects/{projectId}/Tickets/{id}/Edit")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProjectId,Title,Description,Created,Updated,TicketPriorityId,TicketStatusId,TicketTypeId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Updated = DateTimeOffset.Now;
                
                db.Entry(ticket).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }


        // ==============================================
           //COMMENTS - CREATE
        // ============================================== 

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateComment([Bind(Include = "Id,TicketId,Body")] TicketComment ticketComment)
        {
            if (ModelState.IsValid)
            {
                ticketComment.Created = System.DateTimeOffset.Now;
                ticketComment.AuthorId = User.Identity.GetUserId();
                db.TicketComment.Add(ticketComment);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticketComment.AuthorId);
            ViewBag.TicketId = new SelectList(db.Tickets, "Id", "Title", ticketComment.TicketId);
            return View(ticketComment);
        }

        // ==============================================
            //ATTACHMENTS - CREATE
        // ============================================== 

        //POST: TicketAttchments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Projects/{projectId}/Tickets/{ticketId}/AddAttachment")]
        public async Task<ActionResult> AddAttachment([Bind(Include="Id,TicketId,Description,MediaURL")] TicketAttachment ticketAttachment, HttpPostedFileBase file, int projectId, int ticketId)
        {
            //Check if the file selected by the user isn't empty
            if (file != null && file.ContentLength > 0)
            {
                //check the file ext to make sure we allow it
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".pdf" && ext != ".doc" && ext != ".ppt" && ext != ".xls" && ext != ".xlsx" && ext != ".zip")
                {
                    ModelState.AddModelError("file", "Invalid Format");
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
                    

                db.TicketAttchment.Add(ticketAttachment);
                await db.SaveChangesAsync();

                return RedirectToAction("Details", new { projectId, id = ticketId });

            }

            return RedirectToAction("Details", new {projectId, id = ticketId});
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
