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
            CreateMap<LeaveType, DetailsLeaveTypeVM>().ReverseMap() ;
            CreateMap<LeaveType, CreateLeaveTypeVM>().ReverseMap();

            CreateMap<LeaveAllocation, LeaveAllocationVM>().ReverseMap();
            CreateMap<LeaveHistory, LeaveHistoryVM>().ReverseMap();
            CreateMap<Employee, EmployeeVM>().ReverseMap();

        }
    }
}
