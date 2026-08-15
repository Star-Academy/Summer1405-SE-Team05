using ASP.models;

namespace ASP.services;

public interface IDBService
{
    ApiResponse<List<student>> getAll();
    ApiResponse<student> GetStudentByStudentNumber(string studentnumber);
    ApiResponse<string> AddStudent(student student);
    ApiResponse<string> UpdateStudent(string studentnumber , student student);
    ApiResponse<string> DeleteStudent(string studentnumber);
    ApiResponse<List<student>> GenerateRandomStudents(int count);
}