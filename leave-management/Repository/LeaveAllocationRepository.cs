using System;
using System.Collections.Generic;
using System.Linq;
using leave_management.Data;
using leave_management.Contracts;
using Microsoft.EntityFrameworkCore;

namespace leave_management.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db) {
            _db = db;
        }
        public LeaveAllocation GetLeaveAllocationsByEmployeeAndType(string employeeId, int leaveTypeId)
        {
            return FindAll()
                .FirstOrDefault(q => (q.EmployeeId == employeeId) && (q.Period == DateTime.Now.Year) && (q.LeaveTypeId == leaveTypeId)) ;
        }

        public bool CheckAllocation(int leaveTypeId, string employeeId)
        {
            var period = DateTime.Now.Year;
            return FindAll()
                    .Where(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId && q.Period == period)
                    .Any() ;
        }

        public ICollection<LeaveAllocation> GetLeaveAllocationsByEmployee(string id) 
        {
            return FindAll()
                    .Where(q => q.EmployeeId == id && q.Period == DateTime.Now.Year)
                    .ToList() ;    
        }

        public bool Create(LeaveAllocation entity) {
            _db.LeaveAllocations.Add(entity);
            return Save();
        }

        public bool Delete(LeaveAllocation entity) {
            _db.LeaveAllocations.Remove(entity);
            return Save();
        }

        public ICollection<LeaveAllocation> FindAll() {
            return _db.LeaveAllocations
                    .Include(q => q.LeaveType)
                    .Include(q => q.Employee)
                    .ToList() ;
        }

        public LeaveAllocation FindById(int id) {
            return _db.LeaveAllocations
                    .Include(q => q.LeaveType)
                    .Include(q => q.Employee)
                    .FirstOrDefault(q => q.Id == id) ;
        }

        public bool Update(LeaveAllocation entity) {
            _db.LeaveAllocations.Update(entity);
            return Save();
        }

        public bool Save() {
            return (_db.SaveChanges() > 0);
        }

        public bool IsExists(int id)
        {
            return (_db.LeaveAllocations.Any(q => q.Id == id));
        }
    }
}
