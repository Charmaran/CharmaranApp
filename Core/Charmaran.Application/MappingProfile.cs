using AutoMapper;
using Charmaran.Domain.Entities.AttendanceTracker;
using Charmaran.Shared.AttendanceTracker;

namespace Charmaran.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, EmployeeDetailDto>().ReverseMap();
            CreateMap<AttendanceEntry, AttendanceEntryDto>().ReverseMap();
            CreateMap<Holiday, HolidayDto>().ReverseMap();
        }
    }
}