﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper ;
using leave_management.Models;
using leave_management.Data;

namespace leave_management.Mappings 
{
    public class Maps : Profile
    {
        public Maps() 
        {
            CreateMap<LeaveType,        LeaveTypeVM>().ReverseMap() ;
            CreateMap<LeaveAllocation,  LeaveAllocationVM>().ReverseMap();
            CreateMap<LeaveAllocation,  EditLeaveAllocationVM>().ReverseMap();
            CreateMap<LeaveRequest,     LeaveRequestVM>().ReverseMap();
            CreateMap<Employee,         EmployeeVM>().ReverseMap();
        }
    }
}
