using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ataa.Data;
using Ataa.Models;
using System.Dynamic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace Ataa.Controllers
{
    [Authorize(Roles = "Admin")]
    public class Admin : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> roleManager;

        public Admin(ApplicationDbContext context , RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.roleManager = roleManager;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            var Requests = from r in _context.RequestForm
                           select r;
            ViewData["Requests"] = Requests.Count();

            var Users = from u in _context.User
                        select u;
            ViewData["Users"] = Users.Count();

            var request = from r in _context.RequestForm
                          select r;

            var finshedreq = request.Where(r=> r.StatusId == 4);
            var ids = finshedreq.Select(f => f.Id);
            var donatedTime = 0.0;
            foreach (var id in ids)
            {

                var r = finshedreq.Where(fi => fi.Id == id);
                var r1 = r.First();
                var servSDate = r1.StartDate;
                var servEDate = r1.EndDate;
                var servSTime = r1.StartTime;
                var servETime = r1.EndTime;
                var numOFDays = (servEDate - servSDate);
                var numOfHours = (servETime - servSTime);
                if (numOFDays.Days == 0)
                {
                    donatedTime += (numOfHours.Hours + (0.01 * numOfHours.Minutes));
                }
                else
                {
                    donatedTime += (numOfHours.Hours + (0.01 * numOfHours.Minutes)) * numOFDays.Days;
                }
            }
            if (donatedTime > 0)
            {
                if (donatedTime.ToString().Length > 5)
                {
                    ViewBag.donatedTime = (donatedTime + 0.82).ToString().Substring(0, 5);
                    var minutes = (donatedTime + 0.82).ToString().Substring(3, 5);
                    if (Int16.Parse(minutes) >= 60)
                    {
                      ViewBag.donatedTime = Math.Round(ViewBag.donatedTime);
                    }
                    else
                    {
                        ViewBag.donatedTime = ViewBag.donatedTime;
                    }
                }
                else
                {
                    ViewBag.donatedTime = Math.Round((donatedTime + 0.82)).ToString();
                    if ((donatedTime + 0.82).ToString().Length < 5)
                    {
                        ViewBag.donatedTime = (donatedTime + 0.82).ToString() + "0";
                    }
                }
            }
            else
            {
                ViewBag.donatedTime = 0;
            }

            return View(await _context.RequestForm.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestForm = await _context.RequestForm
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestForm == null)
            {
                return NotFound();
            }

            return View(requestForm);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,StartTime,EndTime,NumberOfGrantee,Service,UserId,JobId,StatusId")] RequestForm requestForm)
        {
            if (ModelState.IsValid)
            {
                _context.Add(requestForm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(requestForm);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Approve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestForm = await _context.RequestForm.FindAsync(id);
            if (requestForm == null)
            {
                return NotFound();
            }
            return View(requestForm);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, [Bind("Id,StartDate,EndDate,StartTime,EndTime,NumberOfGrantee,Service,UserId,JobId,StatusId")] RequestForm requestForm)
        {
            if (id != requestForm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var req = _context.RequestForm.First(a => a.Id == id);
                    req.StatusId = 2;
                    _context.SaveChanges();
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestFormExists(requestForm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Requests));
            }
            return View(requestForm);
        }


        public async Task<IActionResult> Reject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestForm = await _context.RequestForm.FindAsync(id);
            if (requestForm == null)
            {
                return NotFound();
            }
            return View(requestForm);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, [Bind("Id,StartDate,EndDate,StartTime,EndTime,NumberOfGrantee,Service,UserId,JobId,StatusId")] RequestForm requestForm)
        {
            if (id != requestForm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var req = _context.RequestForm.First(a => a.Id == id);
                    req.StatusId = 3;
                    _context.SaveChanges();
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestFormExists(requestForm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Requests));
            }
            return View(requestForm);
        }

        private bool RequestFormExists(int id)
        {
            return _context.RequestForm.Any(e => e.Id == id);
        }

        public async Task<IActionResult> StatisticsAsync()
        {
           return View(await _context.RequestForm.ToListAsync());

        }
        public async Task<IActionResult> Requests()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Request = await _context.RequestForm.ToListAsync();
            mymodel.Status = await _context.Status.ToListAsync();
            var RequestStatus = from r in _context.RequestForm join s in _context.Status on r.StatusId equals s.Id into rs
                                from s in rs.DefaultIfEmpty()
                                select new RequestStatus { reqForm = r, status= s } ;
            RequestStatus = RequestStatus.Where(r => r.reqForm.StatusId == 1 || r.reqForm.StatusId == 6);
            var Users = from u in _context.User
                        select u;
            return View(RequestStatus);

        }
      
        public async Task<IActionResult> Job()
        {
            return View(await _context.Job.ToListAsync());
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Detailsj(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Jobs/Create
        public IActionResult Createj()
        {
            List<Section> sl = new List<Section>();
            sl = (from s in _context.Section select s).ToList();
            sl.Insert(0, new Section { Id = 0, SectionName = "--Select Section Name--" });
            ViewBag.message = sl;
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Createj([Bind("FormId,JobName,SectionId")] Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Job));
            }
            return View(job);
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Editj(int? id)
        {
            List<Section> sl = new List<Section>();
            sl = (from s in _context.Section select s).ToList();
            sl.Insert(0, new Section { Id = 0, SectionName = "--Select Section Name--" });
            ViewBag.message = sl;
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editj(int id, [Bind("Id,JobName,SectionId")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Job));
            }
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Deletej(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Job
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedj(int id)
        {
            var job = await _context.Job.FindAsync(id);
            _context.Job.Remove(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return _context.Job.Any(e => e.Id == id);
        }

        public async Task<IActionResult> UsersListAsync()
        {
            return View(await _context.User.ToListAsync());

        }
        public async Task<IActionResult> AdminsAsync()
        {
            return View(await _context.User.ToListAsync());

        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                               
             IdentityResult result = await roleManager.CreateAsync(identityRole);
                
            if (result.Succeeded)
            {
                    return RedirectToAction("CreateRole", "Admin");
            }
            foreach(IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> FeedBack(int ?id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var re = await _context.RequestForm.FindAsync(id);
            if (re == null)
            {
                return NotFound();
            }
            ViewBag.feedID = id;
            return View(re);
        }
        
        [HttpPost]
        public async Task<IActionResult> FeedBack(int id , [Bind("Id,StartDate,EndDate,StartTime,EndTime,NumberOfGrantee,Service,UserId,StatusId,Location,Feedback,IsCalculated")] RequestForm requestForm)
        {
            if (id != requestForm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    requestForm.StatusId = 6;
                    _context.Update(requestForm);
                    //_context.SaveChanges();
                    await _context.SaveChangesAsync();                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestFormExists(requestForm.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Requests));
            }
            return View(requestForm);
        }


    }
}
