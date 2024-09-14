using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.ViewModels;
using AspNetCoreDemo.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreDemo.Web.Controllers
{
    [ApiController]
    [Route("student/")]
    //[Authorize]
    public class StudentController : ControllerBase
    {
        readonly IStudentService _student;

        public StudentController(IStudentService student)
        {
            _student = student;
        }

        /// <summary>
        /// 获取学生分页列表
        /// </summary>
        [HttpPost]
        [Route("list")]
        public PageResultViewModel<StudentDataDto> GetStudentList(StudentSearchDto dto)
        {
            return new PageResultViewModel<StudentDataDto>()
            {
                List = _student.GetStudentData(out long count, dto),
                Count = count
            };
        }
    }
}
