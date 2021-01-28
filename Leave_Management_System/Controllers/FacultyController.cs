﻿using Leave_Management_System.Models.Class;
using Leave_Management_System.Models.Context;
using Leave_Management_System.Models.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Leave_Management_System.Controllers
{
    public class FacultyController : Controller
    {
        private readonly LeaveDbContext _context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        public FacultyController(LeaveDbContext context, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult LeaveRequest()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LeaveRequest(LeaveRequest leaveRequest)
        {
            string username = User.Identity.Name;
            var singleUser = _context.AllUser.Where(x => x.Email == username).FirstOrDefault();
            var leaveHistory = new LeaveHistory
            {
                StartFrome = leaveRequest.LeaveStartFrome,
                EndTill = leaveRequest.LeaveEndTill,
                LeaveReason = leaveRequest.LeaveReason,
                NoOfDay = (int)((leaveRequest.LeaveEndTill - leaveRequest.LeaveStartFrome).TotalDays),
                DeanApproveStatus = "Pending",
                HODApproveStatus= "Pending",
                RegistrarApproveStatus= "Pending",
                LeaveStatus= "Pending",
                id = singleUser.id,
            };
            _context.LeaveHistory.Add(leaveHistory);
            _context.SaveChangesAsync();
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> MyLeave()
        {
            string curentUser = User.Identity.Name;
            var userlevelist = _context.LeaveHistory.Where(x => x.AllUser.Email == curentUser).ToList();
            List<MyLeave> myLeaves = new List<MyLeave>();
            foreach(var temp in userlevelist)
            {
                int status = DateTime.Compare(temp.StartFrome, DateTime.Now);
                var singleLeave = new MyLeave
                {
                    EndTill = temp.EndTill,
                    LeaveReason = temp.LeaveReason,
                    LeaveStatus = temp.LeaveStatus,
                    leave_id = temp.leave_id,
                    NoOfDay = temp.NoOfDay,
                    StartFrome = temp.StartFrome,
                    changeable = (status > 0) ? 1 : 0,
                };
                myLeaves.Add(singleLeave);
            }
            return View(myLeaves);
        }
        
        [HttpGet]
        public async Task<IActionResult> UpdateLeave(int leave_id)
        {
            if (leave_id == 0)
                return NotFound();
            var leaveHistory = _context.LeaveHistory.Where(m => m.leave_id == leave_id && m.AllUser.Email == User.Identity.Name).FirstOrDefault();
            //
            if (leaveHistory == null)
            {
                //return RedirectToAction("MyLeave", "Faculty");

                return NotFound();
            }
            int status = DateTime.Compare(leaveHistory.StartFrome, DateTime.Now);
            if (status <= 0)
            {
                return RedirectToAction("MyLeave", "Faculty");
            }
            var leareqyest = new LeaveRequest
            {
                id = leaveHistory.leave_id,
                LeaveEndTill = leaveHistory.EndTill,
                LeaveReason = leaveHistory.LeaveReason,
                LeaveStartFrome = leaveHistory.StartFrome
            };
            //ViewBag.leave = leave_id;
            return View(leareqyest);
            
        }
        [HttpPost]
        public async Task<IActionResult> UpdateLeave(int id, LeaveRequest leaveRequest)
        {
            int leave_id = id;
            if (leave_id == 0)
                return NotFound();
            var leaveHistory = _context.LeaveHistory.Where(m => m.leave_id == leave_id && m.AllUser.Email == User.Identity.Name).FirstOrDefault();
            if (leaveHistory == null)
            {
                return NotFound();
                //return RedirectToAction("MyLeave", "Faculty");
            }
            int status = DateTime.Compare(leaveHistory.StartFrome, DateTime.Now);
            if (status <= 0)
            {
                return RedirectToAction("MyLeave", "Faculty");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    leaveHistory.LeaveReason = leaveRequest.LeaveReason;
                    leaveHistory.LeaveStatus = "Pending";
                    leaveHistory.HODApproveStatus = "Pending";
                    leaveHistory.EndTill = leaveRequest.LeaveEndTill;
                    leaveHistory.StartFrome = leaveRequest.LeaveStartFrome;
                    leaveHistory.NoOfDay = (int)((leaveRequest.LeaveEndTill - leaveRequest.LeaveStartFrome).TotalDays);
                    _context.Update(leaveHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                        throw;
                   
                }
                return RedirectToAction("MyLeave", "Faculty");
            };
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> DeleteLeave(int leave_id)
        {
            if (leave_id == 0)
                return RedirectToAction("MyLeave", "Faculty");
            //return NotFound();
            var leaveHistory = _context.LeaveHistory.Where(m => m.leave_id == leave_id && m.AllUser.Email == User.Identity.Name).FirstOrDefault();
            if (leaveHistory == null)
            {
                return NotFound();
            }
            int status = DateTime.Compare(leaveHistory.StartFrome, DateTime.Now);
            if (status <= 0)
            {
                return RedirectToAction("MyLeave", "Faculty");
            }
            DeleteLeave deleteLeave = new DeleteLeave
            {
                LeaveEndTill = leaveHistory.EndTill,
                LeaveStatus = leaveHistory.LeaveStatus,
                LeaveReason = leaveHistory.LeaveReason,
                LeaveStartFrome = leaveHistory.StartFrome,
                leave_id = leaveHistory.leave_id,
                NoOfDay= leaveHistory.NoOfDay,
            };
            ViewBag.id = leave_id;
            return View(deleteLeave);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteLeave(int leave_id, DeleteLeave deleteLeave)
        {
            //var leaveHistory = await _context.LeaveHistory.FindAsync(leave_id && m.AllUser.Email == User.Identity.Name);
            var leaveHistory = await _context.LeaveHistory
                .Include(l => l.AllUser)
                .FirstOrDefaultAsync(m => m.leave_id == leave_id && m.AllUser.Email == User.Identity.Name);

            if (leaveHistory == null)
                return NotFound();
            
            _context.LeaveHistory.Remove(leaveHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyLeave", "Faculty");
        }

    }
}
