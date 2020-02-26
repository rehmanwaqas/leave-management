﻿using System;
using System.Collections.Generic;
using System.Linq;
using leave_management.Data;
using leave_management.Contracts;

namespace leave_management.Repository
{
    public class LeaveAllocationRepository : ILeaveAllocationRepository
    {
        private readonly ApplicationDbContext _db;

        public LeaveAllocationRepository(ApplicationDbContext db) {
            _db = db;
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
            return _db.LeaveAllocations.ToList() ;
        }

        public LeaveAllocation FindById(int id) {
            return _db.LeaveAllocations.Find(id) ;
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
