using AspNetCoreDemo.Common;
using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.Enums;
using AspNetCoreDemo.Model.ViewModels;
using AspNetCoreDemo.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreDemo.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
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
        public PageResultViewModel<StudentDataDto> GetStudentList(StudentSearchDto dto)
        {
            return new PageResultViewModel<StudentDataDto>()
            {
                List = _student.GetStudentData(out long count, dto),
                Count = count
            };
        }

        /// <summary>
        /// 修改学生信息
        /// </summary>
        [HttpPost]
        public MessageDto<string> UpdateStudentInfo(StudentUpdateViewModel model)
        {
            var student = _student.GetSingleById(model.Id);

            if(student == null) return ResultHelper<string>.GetResult(ErrorEnum.DataError, null, "该学生不存在!");

            student.Name = model.Name;
            student.StudentLevel = model.StudentLevel;

            _student.Update(student);

            return ResultHelper<string>.GetResult(ErrorEnum.Success, null, EnumExtension.GetRemark(ErrorEnum.Success));
        }
    }
}
