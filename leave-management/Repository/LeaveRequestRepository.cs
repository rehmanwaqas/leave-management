﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leave_management.Contracts;
using leave_management.Data;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Repository
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveRequestRepository(ApplicationDbContext db) {
            _db = db;
        }

        public bool Create(LeaveRequest entity) {
            _db.LeaveRequests.Add(entity) ;
            return Save() ;
        }

        public bool Delete(LeaveRequest entity) {
            _db.LeaveRequests.Remove(entity) ;
            return Save();
        }

        public ICollection<LeaveRequest> FindAll() {
            return _db.LeaveRequests
                        .Include(q => q.RequestingEmployee)
                        .Include(q => q.ApprovedBy)
                        .Include(q => q.LeaveType)
                        .ToList() ;
        }

        public LeaveRequest FindById(int id) {
            return _db.LeaveRequests
                        .Include(q => q.RequestingEmployee)
                        .Include(q => q.ApprovedBy)
                        .Include(q => q.LeaveType)
                        .FirstOrDefault(q => q.Id == id) ;
        }

        public bool Update(LeaveRequest entity) {
            _db.LeaveRequests.Update(entity);
            return Save();
        }

        public bool Save() {
            return (_db.SaveChanges() > 0) ;
        }

        public bool IsExists(int id) {
            return (_db.LeaveRequests.Any(q => q.Id == id));
        }

        public ICollection<LeaveRequest> GetLeaveRequestsByEmployee(string employeeId) 
        {
            return FindAll().Where(q => q.RequestingEmployeeId == employeeId).ToList();
        }
    }
}