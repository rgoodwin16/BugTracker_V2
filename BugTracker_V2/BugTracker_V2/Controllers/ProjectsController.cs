using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using BugTracker_V2.Models;

namespace BugTracker_V2.Controllers
{
    [RequireHttps]
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles="Admin")]
        // GET: Projects
        public async Task<ActionResult> Index(string search)
        {
            var projectList = from str in db.Projects
                              select str;
            if (search == null)
            {
                return View(await db.Projects.OrderByDescending(p => p.Created).ToListAsync());
            }

            else
            {
                ViewBag.search = search;
                projectList = db.Projects.Where(s=> s.Title.Contains(search) || 
                    s.Description.Contains(search) || 
                    //s.ProjectManager.UserName.Contains(search) ||
                    //s.ProjectManager.DisplayName.Contains(search) ||
                    s.Tickets.Any(t=> t.Title.Contains(search)) 
                    || s.Tickets.Any(t=> t.Description.Contains(search)) 
                    || s.Tickets.Any(t=> t.Comments.Any(c=> c.Body.Contains(search))) ||
                    s.Tickets.Any(t=> t.TicketPriority.Name.Contains(search)) ||
                    s.Tickets.Any(t=> t.TicketStatus.Name.Contains(search)) ||
                    s.Tickets.Any(t=> t.TicketType.Name.Contains(search)) ||
                    s.Tickets.Any(t=> t.AssignedTo.UserName.Contains(search)) ||
                    s.Tickets.Any(t=> t.AssignedTo.DisplayName.Contains(search))
                    );
            }

            return View(await projectList.OrderByDescending(p=> p.Created).ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles="Admin,ProjectManager")]
        public ActionResult Create()
        {

            ViewBag.ProjectUserIds = new MultiSelectList(db.Users.ToList(), "Id", "UserName");//This is how we create the dropdown box that has a list of every user in the db.
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin,ProjectManager")]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Description,Created,Updated")] Project project, string[] ProjectUserIds)
        {
            if (ModelState.IsValid)
            {
                project.Created = DateTimeOffset.Now;
               
                foreach (var userId in ProjectUserIds)
                {
                    var fakeUser = new ApplicationUser() { Id = userId };
                    db.Users.Attach(fakeUser);
                    project.Users.Add(fakeUser);
                    
                }

                db.Projects.Add(project);
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
 
            return View(project);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles="Admin,ProjectManager")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProjectUserIds = new MultiSelectList(db.Users.ToList(), "Id", "UserName");//This is how we create the dropdown box that has a list of every user in the db.
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Admin,ProjectManager")]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Created,Updated")] Project project, string [] ProjectUserIds)
        {
            if (ModelState.IsValid)
            {
                //var properties = new List<string>() { "Updated", "Description" };
                var existing = db.Projects.Find(project.Id);

                

                existing.Users.Clear();
                foreach (var userId in ProjectUserIds)
                {
                   
                   existing.Users.Add(db.Users.Find(userId));

                }
                
                //set existing project properties to same as posted project
                
                //properties.AddRange(new string[] { "ProjectUserIds" });

                project.Description = existing.Description;
                project.Updated = DateTimeOffset.Now;

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Project project = await db.Projects.FindAsync(id);
            db.Projects.Remove(project);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
