using ABPDemo.Enums;
using ABPDemo.StudentManagement;
using ABPDemo.StudentManagement.Dtos;
using ABPDemo.SystemManagement;
using ABPDemo.SystemManagement.Dtos;
using ABPDemo.UserManagement.Dtos;
using AutoMapper;
using System.Linq;
using Volo.Abp.Identity;

namespace ABPDemo;

public class ABPDemoApplicationAutoMapperProfile : Profile
{
    public ABPDemoApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<OperationLog, OperationLogDto>()
            .ForMember(p => p.Operation, b => b.MapFrom(s => s.GetOperationData().GetOperationName()))
            .ForMember(p => p.Summary, b => b.MapFrom(s => s.GetOperationData().ToSummary()))
            .ForMember(p => p.Detail, b => b.MapFrom(s => s.GetOperationData().ToDetail()));
        CreateMap<Course, CourseDto>();
        CreateMap<StudentScore, StudentScoreDto>();
        CreateMap<Student, StudentDto>()
            .ForMember(d => d.Courses, p => p.MapFrom(s => s.StudentCourses.Select(x => x.Course)));
        CreateMap<Student, StudentSimpleDto>();
    }
}
