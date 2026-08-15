using asp.models;

namespace asp.services;

public interface IDBService
{
    ApiResponse<List<student>> getAll();
    ApiResponse<student> getStudentByStudentNumber(string studentnumber);
    ApiResponse<string> addStudent(student student);
    ApiResponse<string> updateStudent(student student);
    ApiResponse<string> deleteStudent(string studentnumber);
    ApiResponse<List<student>> generateRandomStudents(int count);
}