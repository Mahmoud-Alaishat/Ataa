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
using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace Ataa.Views
{
    [Authorize(Roles = "User")]

    public class RequestFormsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestFormsController(ApplicationDbContext context)
        {
            _context = context;
        }
      
        // GET: RequestForms
        public async Task<IActionResult> Index()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Request = await _context.RequestForm.ToListAsync();
            mymodel.User = await _context.User.ToListAsync();
            var RequestUser = from r in _context.RequestForm
                              join u in _context.User on r.UserId equals u.Id into ru
                              from u in ru.DefaultIfEmpty()
                              select new RequestUser { ReqForm = r, user=u};
            RequestUser = RequestUser.Where(r => r.ReqForm.UserId != User.FindFirst(ClaimTypes.NameIdentifier).Value && (r.ReqForm.StatusId == 2 || r.ReqForm.StatusId == 5));
            ViewBag.haveServices = true;

            if (RequestUser.ToArray().Length == 0)
            {
                ViewBag.haveServices = false;
            }
            Status();                     
            return View(RequestUser);
        }
        public async Task<IActionResult> MyServices()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Request = await _context.RequestForm.ToListAsync();
            mymodel.Status = await _context.Status.ToListAsync();
            var RequestStatus = from r in _context.RequestForm
                                join s in _context.Status on r.StatusId equals s.Id into rs
                                from s in rs.DefaultIfEmpty()
                                select new RequestStatus { reqForm = r, status = s };
            ViewData["Requests"] = RequestStatus.Count();

            var Users = from u in _context.User
                        select u;
            RequestStatus = RequestStatus.Where(r => r.reqForm.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);
            Status();
            return View(RequestStatus);
        }

        public async void Status()
        {
            string date = DateTime.Now.GetDateTimeFormats('d')[0];
            string time = DateTime.Now.GetDateTimeFormats('t')[0];

           
                var request = from m in _context.RequestForm
                        select m;
                var ids = request.Select(b => b.Id);

                foreach (var id in ids)
                {
                    var isMeetDateP = false;
                    var isMeetDateTimeD = false;

                    var r = request.Where(fi => fi.Id == id);
                    var r1 = r.First();
                    var servSDate = r1.StartDate.GetDateTimeFormats('d')[0];
                    var servEDate = r1.EndDate.GetDateTimeFormats('d')[0];
                    var servSTime = r1.StartTime.GetDateTimeFormats('t')[0];
                    var servETime = r1.EndTime.GetDateTimeFormats('t')[0];
                    var sta = r1.StatusId;


                    if (sta == 2 || sta == 5)
                    {
                        if ((DateTime.Parse(date, CultureInfo.InvariantCulture) >= (DateTime.Parse(servSDate, CultureInfo.InvariantCulture))) && (DateTime.Parse(date, CultureInfo.InvariantCulture) < (DateTime.Parse(servEDate, CultureInfo.InvariantCulture))))
                        {
                        if ((DateTime.Parse(time) >= DateTime.Parse(servSTime)) && (DateTime.Parse(time) <= DateTime.Parse(servETime)))
                        {
                            isMeetDateP = true;
                        }
                        }

                        else if ((DateTime.Parse(date, CultureInfo.InvariantCulture) == (DateTime.Parse(servEDate, CultureInfo.InvariantCulture))))
                        {
                            if ((DateTime.Parse(time) >= DateTime.Parse(servSTime)) && (DateTime.Parse(time) <= DateTime.Parse(servETime)))
                            {
                                isMeetDateP = true;
                            }
                            else if((DateTime.Parse(time) > DateTime.Parse(servETime)))
                            {
                            isMeetDateTimeD = true;
                            }
                        }
                        else
                        {
                            if (DateTime.Parse(date) > DateTime.Parse(servEDate))
                            {
                                isMeetDateTimeD = true;
                            }
                            
                        }
                    }
                    if (isMeetDateP)
                    {
                        r1.StatusId = 5;
                    }
                    if (isMeetDateTimeD)
                    {
                        r1.StatusId = 4;
                    }
                }
                _context.SaveChanges();
                await _context.SaveChangesAsync();
            

          
        }
        public async Task<IActionResult> MyRequests()
        {
            dynamic mymodel = new ExpandoObject();
            mymodel.Request = await _context.RequestForm.ToListAsync();
            mymodel.Status = await _context.Status.ToListAsync();
            var RequestStatus = from r in _context.RequestForm
                                join s in _context.Status on r.StatusId equals s.Id into rs
                                from s in rs.DefaultIfEmpty()
                                select new RequestStatus { reqForm = r, status = s };
            ViewData["Requests"] = RequestStatus.Count();

            var Users = from u in _context.User
                        select u;
            RequestStatus = RequestStatus.Where(r => r.reqForm.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Status();

            return View(RequestStatus);
        }


        // GET: RequestForms/Details/5
        public async Task<IActionResult> SDetails(int? id)
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
            var re = _context.Attendancelist.Where(re => re.FormId == id);
            ViewBag.AttCount = re.Count();

            return View(requestForm);
        }
        public async Task<IActionResult> RDetails(int? id)
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
            var re = _context.Attendancelist.Where(re => re.FormId == id);
            ViewBag.AttCount = re.Count();

            return View(requestForm);
        }


        public async Task<IActionResult> Grantees(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            dynamic mymodell = new ExpandoObject();
            mymodell.at = await _context.Attendancelist.ToListAsync();
            mymodell.us = await _context.User.ToListAsync();

            var usersatt = from a in _context.Attendancelist
                           join u in _context.User on a.UserId equals u.Id into au
                           from u in au.DefaultIfEmpty()
                           select new AttendUser { attlist = a, user = u };
            var attendance = usersatt.Where(m => m.attlist.FormId == id);
            

            if (attendance.ToArray().Length > 0)
            {
                var idf = attendance.First(n => n.attlist.FormId == id);
            }
            ViewBag.formId = id;

            return View(attendance);
        }


        public IActionResult Apply(int id)
        {
            ViewBag.isApplied = true;

            Attendancelist model = new Attendancelist();
            if (ModelState.IsValid)
            {
                var atReq = _context.Attendancelist.Where(r => r.FormId == id && r.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var atReqCount = atReq.Count();

                var req = _context.RequestForm.First(requ => requ.Id == id);
                var nOfGrantees = req.NumberOfGrantee;
                var re = _context.Attendancelist.Where(re => re.FormId == id);
                var AttCount = re.Count();
                if (atReqCount == 0 && AttCount <= nOfGrantees)
                {
                    model.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    model.FormId = id;
                    _context.Add(model);

                    _context.SaveChanges();
                }
                ViewBag.isApplied = false;
            }
            return Redirect("https://localhost:44395/RequestForms/ServiceDetails/"+ id.ToString());
        }

        public async Task<IActionResult> ServiceDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
          

            var users = from u in _context.User
                        select u;

            var v = await _context.RequestForm.FirstOrDefaultAsync(m => m.Id == id);
            var userID = v.UserId;
            var user= users.First(us => us.Id == userID);
            var fname= user.FirstName;
            var lname = user.LastName;
            ViewBag.fullname = fname + " " + lname;

            if (v == null)
            {
                return NotFound();
            }
               
                ViewBag.isApplied = true;
                var atReq = _context.Attendancelist.Where(r => r.FormId == id && r.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var atReqCount = atReq.Count();
                var req = _context.RequestForm.First(requ => requ.Id == id);
                var nOfGrantees = req.NumberOfGrantee;
                var re = _context.Attendancelist.Where(re => re.FormId == id);
                ViewBag.AttCount = re.Count();
            if (atReqCount == 0)
                {
                    ViewBag.isApplied = true;

                }
                else
                {
                    ViewBag.isApplied = false;
                }

            return View(v);

        }



        // GET: RequestForms/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RequestForms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FormId,StartDate,EndDate,StartTime,EndTime,NumberOfGrantee,Service,UserId,JobId,StatusId,Location")] RequestForm requestForm)
        {

            if (ModelState.IsValid)
            {     
                
               var us=from u in _context.User
                select u.Id;
              
                requestForm.StatusId = 1;
                requestForm.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _context.Add(requestForm);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MyRequests));
            }
            return View(requestForm);
        }

       
        // GET: RequestForms/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

        // POST: RequestForms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,StartTime,EndTime,NumberOfGrantee,Service,UserId,StatusId,Location,Feedback,IsCalculated")] RequestForm requestForm)
        {
            if (id != requestForm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    requestForm.StatusId = 1;
                    requestForm.Feedback = "";
                    _context.Update(requestForm);
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
                return RedirectToAction(nameof(MyRequests));
            }
            return View(requestForm);
        }

        // GET: RequestForms/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: RequestForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requestForm = await _context.RequestForm.FindAsync(id);
            _context.RequestForm.Remove(requestForm);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestFormExists(int id)
        {
            return _context.RequestForm.Any(e => e.Id == id);
        }
        public IActionResult Profile()
        {
            var Users = from u in _context.User
                        select u;
            var user= Users.First(u => u.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var job = from j in _context.Job
                      where j.Id == user.JobId
                      select j.JobName;
            ViewBag.Job = job.First();
            var fname = user.FirstName;
            var lname = user.LastName;
            ViewBag.fName = fname;
            ViewBag.fullName = fname + " " + lname;
            var request = from r in _context.RequestForm
                      select r;
            var req = request.Where((r => r.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && (r.StatusId == 2  || r.StatusId == 4 || r.StatusId == 5)));
            ViewBag.requestCounts = req.Count();
            var formIds= req.Select(s => s.Id);
            var attend = from a in _context.Attendancelist
            select a;
            var att = attend.Where(at=>at.FormId != null &&
                formIds.Contains(at.FormId));
            ViewBag.GranteesN = att.Count();
            var finshedreq = request.Where((r => r.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value &&  r.StatusId == 4 ));
            var ids = finshedreq.Select(f => f.Id);
            var donatedTimeh = 0;
            var donatedTimem = 0;

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
                    donatedTimeh += numOfHours.Hours;
                    donatedTimem += numOfHours.Minutes;
                }
                else 
                {
                    donatedTimeh = numOfHours.Hours * numOFDays.Days;
                    donatedTimem= numOfHours.Minutes * numOFDays.Days;
                }
            }
            ViewBag.donatedTimeh = donatedTimeh;
            ViewBag.donatedTimem = donatedTimem;
            ClockCoin();
            return View(user);
        }
        public void ClockCoin()
        {
            var Users = from u in _context.User
                        select u;
            var user = Users.First(u => u.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var clockCoin = user.ClockCoin;

            var request = from m in _context.RequestForm
                          select m;
            request = request.Where(r => r.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && r.StatusId == 4);
            var ids = request.Select(b => b.Id);
            var nOfServices = request.ToArray().Length;

            foreach (var id in ids)
            {

                var r = request.Where(fi => fi.Id == id && fi.IsCalculated == 0);
                if (r.ToArray().Length >= 1)
                {
                    var r1 = r.First();
                    var servSDate = r1.StartDate;
                    var servEDate = r1.EndDate;
                    var servSTime = r1.StartTime;
                    var servETime = r1.EndTime;
                    var days = (servEDate - servSDate).Days;
                    var hours = (servETime - servSTime).Hours;
                    var minutes = (servETime - servSTime).Minutes;
                    r1.IsCalculated = 1;
                    if (nOfServices >= 0 && nOfServices < 10)
                    {
                        if (days == 0)
                        {
                            if (hours == 0)
                            {
                                user.ClockCoin = (user.ClockCoin + 0);
                            }
                            else if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 3);
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 6);
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 9);
                            }
                        }
                        else if (days >= 1 && days < 4)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 2);
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 3);
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 5);

                            }
                        }
                        else if (days >= 4 && days < 7)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 4);
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 8);
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 12);
                            }
                        }
                        else if (days >= 7 && days <= 10)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 8);
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 16);
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 22);
                            }
                        }
                        else if (days > 10)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 8);
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 16);
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 22);
                            }
                        }
                    }
                    else if (nOfServices >= 10 && nOfServices < 25)
                    {
                        if (days == 0)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + 5;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + 5;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + 5;
                            }
                        }
                        else if (days >= 1 && days < 4)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 2) + 5;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 3) + 5;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 5) + 5;

                            }
                        }
                        else if (days >= 4 && days < 7)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 4) + 5;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 8) + 5;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 12) + 5;
                            }
                        }
                        else if (days >= 7 && days <= 10)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 8) + 5;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 16) + 5;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 22) + 5;
                            }
                        }
                        else if (days > 10)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 8) + 5;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 16) + 5;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 22) + 5;
                            }
                        }
                    }
                    else if (nOfServices >= 25 && nOfServices < 50)
                    {
                        if (days == 0)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + 10;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + 10;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + 10;
                            }
                        }
                        else if (days >= 1 && days < 4)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 2) + 10;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 3) + 10;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 5) + 10;

                            }
                        }
                        else if (days >= 4 && days < 7)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 4) + 10;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 8) + 10;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 12) + 10;
                            }
                        }
                        else if (days >= 7 && days <= 10)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 8) + 10;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 16) + 10;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 22) + 10;
                            }
                        }
                        else if (days > 10)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 8) + 10;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 16) + 10;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 22) + 10;
                            }
                        }
                    }
                    else if (nOfServices >= 50 && nOfServices < 75)
                    {
                        if (days == 0)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 4) + 15;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 8) + 15;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 12) + 15;
                            }
                        }
                        else if (days >= 1 && days < 4)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 2) + 15;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 3) + 15;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 5) + 15;

                            }
                        }
                        else if (days >= 4 && days < 7)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 4) + 15;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 8) + 15;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 12) + 15;
                            }
                        }
                        else if (days >= 7 && days <= 10)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 8) + 15;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 16) + 15;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 22) + 15;
                            }
                        }
                        else if (days > 10)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 8) + 15;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 16) + 15;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 22) + 10;
                            }
                        }

                    }
                    else
                    {
                        if (days == 0)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 4) + 30;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 8) + 30;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 12) + 30;
                            }
                        }
                        else if (days >= 1 && days < 4)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 2) + 30;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 3) + 30;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 3) + (days + 5) + 30;

                            }
                        }
                        else if (days >= 4 && days < 7)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 4) + 30;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 8) + 30;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 6) + (days + 12) + 30;
                            }
                        }
                        else if (days >= 7 && days <= 10)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 8) + 30;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 16) + 30;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 9) + (days + 22) + 30;
                            }
                        }
                        else if (days > 10)
                        {
                            if (hours >= 1 && hours <= 2)
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 8) + 30;
                            }
                            else if (hours >= 3 && hours <= 5)
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 16) + 30;
                            }
                            else
                            {
                                user.ClockCoin = (user.ClockCoin + 12) + (days + 22) + 30;
                            }
                        }
                    }
                }
            }
            _context.SaveChanges();
        }

    }
}
