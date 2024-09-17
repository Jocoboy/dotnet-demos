using AspNetCoreDemo.Common;
using AspNetCoreDemo.Common.Extensions;
using AspNetCoreDemo.Common.Extensions.Excel;
using AspNetCoreDemo.Model.Dtos;
using AspNetCoreDemo.Model.EFCore.Entity;
using AspNetCoreDemo.Model.Enums;
using AspNetCoreDemo.Model.ViewModels;
using AspNetCoreDemo.Service.IService;
using AutoMapper;
using MathNet.Numerics.Distributions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.POIFS.Properties;
using NPOI.SS.Formula.Functions;
using System.Collections.Generic;

namespace AspNetCoreDemo.Web.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    [Authorize]
    public class StudentController : ControllerBase
    {
        readonly IStudentService _student;
        readonly IMapper _map;

        public StudentController(IStudentService student, IMapper map)
        {
            _student = student;
            _map = map;
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

            if(student == null) return ResultHelper<string>.GetResult(ErrorType.DataError, null, "该学生不存在!");

            student.Name = model.Name;
            student.StudentLevel = model.StudentLevel;

            _student.Update(student);

            return ResultHelper<string>.GetResult(ErrorType.Success, null, EnumExtension.GetRemark(ErrorType.Success));
        }

        /// <summary>
        /// 导出学生名单
        /// </summary>
        [HttpPost]
        public FileResult StudentExportExcel(StudentSearchDto dto)
        {
            var list = _student.GetStudentData(out long count, dto);
            var bytes = ExcelHelper.OutputExcel(list);

            return File(bytes, "application/octet-stream", $"学生名单_{DateTime.Now.ToShortDateString()}.xlsx");
        }

        /// <summary>
        /// 导入学生名单
        /// </summary>
        [HttpPost]
        public MessageDto<string> StudentImportExcel(ImportType importType, IFormCollection form = null)
        {
            var students = new List<StudentDataDto>();
            using (Stream stream = CustomHttpContext.Current.Request.Form.Files[0].OpenReadStream())
            {
                students = ExcelHelper.ReadExcelToEntity<StudentDataDto>(stream);
            }

            if (students.Count == 0)
            {
                return ResultHelper<string>.GetResult(ErrorType.DataError, null, "未读取到数据，请检查Excel数据格式是否符合模板要求");
            }

            return _student.ImportStudentData(_map.Map<List<Student>>(students), importType);
        }
    }
}
