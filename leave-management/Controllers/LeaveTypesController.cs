﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using leave_management.Contracts ;
using leave_management.Repository ;
using leave_management.Data;
using leave_management.Models;
using AutoMapper ;
namespace leave_management.Controllers
{
    public class LeaveTypesController : Controller
    {
        private readonly ILeaveTypeRepository _repo ;
        private readonly IMapper _mapper ;

        public LeaveTypesController(ILeaveTypeRepository repo, IMapper mapper) {
            _repo = repo ;
            _mapper = mapper ;
        }

        // GET: LeaveTypes
        public ActionResult Index() {
            var leaveTypes = _repo.FindAll().ToList() ;
            var model = _mapper.Map<List<LeaveType>, List<LeaveTypeVM>>(leaveTypes) ;
            return View(model) ;
        }

        // GET: LeaveTypes/Details/5
        public ActionResult Details(int id) {
            if (!_repo.IsExists(id)) {
                return NotFound() ;
            }

            var leaveType = _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            return View(model);
        }

        // GET: LeaveTypes/Create
        public ActionResult Create() {
            return View();
        }

        // POST: LeaveTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LeaveTypeVM model) {
            try
            {
                if (!ModelState.IsValid) {
                    return View(model);
                }

                var leaveType = _mapper.Map<LeaveType>(model) ;
                leaveType.DateCreated = DateTime.Now ;
                var isSuccess = _repo.Create(leaveType) ;
                
                if (!isSuccess) {
                    ModelState.AddModelError("", "Something Went Wrong...") ;
                    return View(model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something Went Wrong...");
                return View(model);
            }
        }

        // GET: LeaveTypes/Edit/5
        public ActionResult Edit(int id) {
            if (!_repo.IsExists(id)) {
                return NotFound();
            }
            var leaveType = _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            return View(model);
        }

        // POST: LeaveTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LeaveTypeVM model) {
            try
            {
                if (!ModelState.IsValid) {
                    return View(model) ;
                }
                var leaveType = _mapper.Map<LeaveType>(model);
                var isSuccess = _repo.Update(leaveType);

                if (!isSuccess) {
                    ModelState.AddModelError("", "Something Went Wrong...");
                    return View(model);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Something Went Wrong...");
                return View(model);
            }
        }

        // GET: LeaveTypes/Delete/5
        public ActionResult Delete(int id) {

            var _leaveType = _repo.FindById(id);

            if (_leaveType == null) {
                return NotFound();
            }
            
            var isSuccess = _repo.Delete(_leaveType);
            
            if (!isSuccess) {
                return BadRequest() ;    
            }

            return RedirectToAction(nameof(Index));
            /*
            if (!_repo.IsExists(id)) {
                return NotFound();
            }
            var leaveType = _repo.FindById(id);
            var model = _mapper.Map<LeaveTypeVM>(leaveType);
            return View(model);
            */
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, LeaveTypeVM leaveType) {
            var _leaveType = _repo.FindById(id);

            if (_leaveType == null) {
                return NotFound();
            }

            try {
                var isSuccess = _repo.Delete(_leaveType) ;
                if (!isSuccess)
                {
                    ModelState.AddModelError("", "Something Went Wrong...");
                    return View(_leaveType);
                }
                return RedirectToAction(nameof(Index));
            }
            catch {
                ModelState.AddModelError("", "Something Went Wrong...");
                return View(_leaveType) ;
            }
        }
    }
}