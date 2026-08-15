using asp.models;
using asp.services;
using Microsoft.AspNetCore.Mvc;

namespace asp.controllers
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
            return _service.getStudentByStudentNumber(studentnumber);
        }

        [HttpPost("students")]
        public ApiResponse<string> Add([FromBody] student student)
        {
            return _service.addStudent(student);
        }

        [HttpPut("students")]
        public ApiResponse<string> Update([FromBody] student student)
        {
            return _service.updateStudent(student);
        }

        [HttpPost("students/random/{count:int}")]
        public ApiResponse<List<student>> AddRandomStudent(int count)
        {
            return _service.generateRandomStudents(count);
        }

        [HttpDelete("students/{studentnumber}")]
        public ApiResponse<string> Delete(string studentnumber)
        {
            return _service.deleteStudent(studentnumber);
        }
    }
}