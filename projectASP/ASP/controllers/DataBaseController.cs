using ASP.models;
using ASP.services;
using Microsoft.AspNetCore.Mvc;

namespace ASP.controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataBaseController : ControllerBase
    {
        private readonly IDBService _service;

        public DataBaseController(IDBService service)
        {
            _service = service;
        }
        
        [HttpGet("students")]
        public ApiResponse<List<student>> GetAll()
        {
            return _service.getAll();
        }

        [HttpGet("students/{studentnumber}")]
        public ApiResponse<student> Getspecific(string studentnumber)
        {
            return _service.GetStudentByStudentNumber(studentnumber);
        }

        [HttpPost("students")]
        public ApiResponse<string> Add([FromBody] student student)
        {
            return _service.AddStudent(student);
        }

        [HttpPut("students")]
        public ApiResponse<string> Update([FromBody] student student)
        {
            return _service.UpdateStudent(student);
        }

        [HttpPost("students/random/{count:int}")]
        public ApiResponse<List<student>> AddRandomStudent(int count)
        {
            return _service.GenerateRandomStudents(count);
        }

        [HttpDelete("students/{studentnumber}")]
        public ApiResponse<string> Delete(string studentnumber)
        {
            return _service.DeleteStudent(studentnumber);
        }
    }
}