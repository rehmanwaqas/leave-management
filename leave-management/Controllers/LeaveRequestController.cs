using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using leave_management.Data;
using leave_management.Models;
using leave_management.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace leave_management.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestRepository _leaveRequestRepo;
        private readonly ILeaveTypeRepository _leaveTypeRepo;
        private readonly ILeaveAllocationRepository _leaveAllocationRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<Employee> _userManager;

        public LeaveRequestController(ILeaveRequestRepository leaveRequestRepo,
                                        ILeaveTypeRepository leaveTypeRepo,
                                        ILeaveAllocationRepository leaveAllocationRepo,
                                        IMapper mapper, 
                                        UserManager<Employee> userManager) {

            _leaveRequestRepo = leaveRequestRepo ;
            _leaveTypeRepo = leaveTypeRepo;
            _leaveAllocationRepo = leaveAllocationRepo ;
            _mapper = mapper ;
            _userManager = userManager ;
        }

        [Authorize(Roles = "Administrator")]
        // GET: LeaveRequest
        public ActionResult Index()
        {
            var leaveRequests = _leaveRequestRepo.FindAll() ;
            var leaveRequestsModel = _mapper.Map<List<LeaveRequestVM>>(leaveRequests);
            var model = new AdminLeaveRequestViewVM { TotalRequests = leaveRequestsModel.Count,
                                                        ApprovedRequests = leaveRequestsModel.Count(q => q.Approved == true),
                                                        PendingRequests = leaveRequestsModel.Count(q => q.Approved == null),
                                                        RejectedRequests = leaveRequestsModel.Count(q => q.Approved == false),
                                                        LeaveRequests = leaveRequestsModel} ; 
            return View(model) ;
        }

        // GET: LeaveRequest/Details/5
        public ActionResult Details(int id)
        {
            var leaveRequest = _leaveRequestRepo.FindById(id) ;
            var model = _mapper.Map<LeaveRequestVM>(leaveRequest);
            return View(model);
        }

        public IActionResult MyLeave()
        {
            var user = _userManager.GetUserAsync(User).Result ;
            var employeeLeaveAllocations = _leaveAllocationRepo.GetLeaveAllocationsByEmployee(user.Id);
            var employeeLeaveRequests = _leaveRequestRepo.GetLeaveRequestsByEmployee(user.Id);
        
            var model = new EmployeeLeaveRequestViewVM
            {
                LeaveAllocations = _mapper.Map<List<LeaveAllocationVM>>(employeeLeaveAllocations),
                LeaveRequests = _mapper.Map<List<LeaveRequestVM>>(employeeLeaveRequests)
            } ;

            return View(model);
        }

        public ActionResult CancelRequest(int id) 
        {
            var leaveRequest = _leaveRequestRepo.FindById(id);
            leaveRequest.Cancelled = true;
            _leaveRequestRepo.Update(leaveRequest);
            return RedirectToAction("MyLeave");
        }

        public ActionResult ApproveRequest(int id)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).Result;

                var leaveRequest = _leaveRequestRepo.FindById(id) ;
                
                leaveRequest.Approved = true;
                var allocation = _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);

                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays ;

                allocation.NumberOfDays -= daysRequested ;

                leaveRequest.ApprovedById = user.Id ;
                leaveRequest.DateActioned = DateTime.Now;
                _leaveRequestRepo.Update(leaveRequest) ;
                _leaveAllocationRepo.Update(allocation) ;
                // ModelState.AddModelError("", "You do not have sufficient days for this request.");
                return RedirectToAction(nameof(Index), "LeaveRequest") ;
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong.");
                return RedirectToAction(nameof(Index), "LeaveRequest");
            }
        }

        public ActionResult RejectRequest(int id)
        {
            try
            {
                var leaveRequest = _leaveRequestRepo.FindById(id);
                leaveRequest.Approved = false;

                var user = _userManager.GetUserAsync(User).Result;
                leaveRequest.ApprovedById = user.Id;
                leaveRequest.DateActioned = DateTime.Now;

                var isSuccess = _leaveRequestRepo.Update(leaveRequest);
                // ModelState.AddModelError("", "You do not have sufficient days for this request.");
                return RedirectToAction(nameof(Index), "LeaveRequest");
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong.");
                return RedirectToAction(nameof(Index), "LeaveRequest");
            }
        }




        // GET: LeaveRequest/Create
        public ActionResult Create()
        {
            var leaveTypes = _leaveTypeRepo.FindAll();
            var leaveTypeItems = leaveTypes.Select(q => new SelectListItem {Text = q.Name, Value = q.Id.ToString()});
            var model = new CreateLeaveRequestVM{ LeaveTypes = leaveTypeItems };
            return View(model);
        }

        // POST: LeaveRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateLeaveRequestVM model)
        {
            try
            {
                var startDate = Convert.ToDateTime(model.StartDate) ;
                var endDate = Convert.ToDateTime(model.EndDate);
                
                var leaveTypes = _leaveTypeRepo.FindAll();
                var leaveTypeItems = leaveTypes.Select(q => new SelectListItem { Text = q.Name, Value = q.Id.ToString() });
                model.LeaveTypes = leaveTypeItems;
                
                if (!ModelState.IsValid) {
                    return View(model);
                }

                if (DateTime.Compare(startDate, endDate) > 1) 
                {
                    ModelState.AddModelError("", "Start date cannot be further in the future than the End Date");
                    return View(model) ;
                }
                
                var employee = _userManager.GetUserAsync(User).Result ;
                var allocation = _leaveAllocationRepo.GetLeaveAllocationsByEmployeeAndType(employee.Id, model.LeaveTypeId) ;

                int daysRequested = (int) (endDate.Date - startDate.Date).TotalDays ;

                if (daysRequested > allocation.NumberOfDays)
                {
                    ModelState.AddModelError("", "You do not have sufficient days for this request.") ;
                    return View(model);
                }

                var leaveRequestModel = new LeaveRequestVM 
                {   RequestingEmployeeId = employee.Id,
                    LeaveTypeId = model.LeaveTypeId, 
                    StartDate = startDate,
                    EndDate = endDate,
                    Approved = null,
                    DateRequested = DateTime.Now,
                    DateActioned = DateTime.Now,
                    RequestComments = model.RequestComments
                } ;

                var leaveRequest = _mapper.Map<LeaveRequest>(leaveRequestModel) ;
                var isSuccess = _leaveRequestRepo.Create(leaveRequest);

                if (!isSuccess) 
                {
                    ModelState.AddModelError("", "Something went wrong with submitting your request.") ;
                    return View(model) ;
                }


                return RedirectToAction(nameof(Index), "Home");
            }
            catch
            {
                ModelState.AddModelError("", "Something went wrong");
                return View(model);
            }
        }

        // GET: LeaveRequest/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LeaveRequest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LeaveRequest/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LeaveRequest/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}