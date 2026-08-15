using ASP.helpers;
using ASP.helpers;
using ASP.models;
using ASP.models;
using SqlKata.Execution;

namespace ASP.services;

public class DataBaseService : IDBService
{
    private readonly DbContextManager _dbManager;

    public DataBaseService(DbContextManager dbManager)
    {
        _dbManager = dbManager;
    }

    

    public ApiResponse<List<student>> GenerateRandomStudents(int count)
    {
        
        List<student> students = new List<student>();
        while (students.Count < count)
        {
            student student = StudentGenerator.GenerateRandomStudent();
            AddStudent(student);
            students.Add(student);
        }
        return new ApiResponse<List<student>>(true, $"{students.Count} random students generated successfully.", students);
    }

    public ApiResponse<List<student>> getAll()
    {
        using var db = _dbManager.GetQueryFactory();
        var students = db.Query("student").Get<student>().ToList();
        return new ApiResponse<List<student>>(true, "Students retrieved successfully.", students);
    }

    public ApiResponse<student> GetStudentByStudentNumber(string studentnumber)
    {
        using var db = _dbManager.GetQueryFactory();
        var student = db.Query("student")
                        .Where("studentnumber", studentnumber)
                        .FirstOrDefault<student>();

        if (student == null)
        {
            return new ApiResponse<student>(false, "Student not found.", null);
        }

        return new ApiResponse<student>(true, "Student retrieved successfully.", student);
    }

    public ApiResponse<string> AddStudent(student student)
    {
        using var db = _dbManager.GetQueryFactory();
        var exists = db.Query("student")
                       .Where("studentnumber", student.studentnumber)
                       .Exists();

        if (exists)
        {
            return new ApiResponse<string>(false, "already exists", null);
        }

        db.Query("student").Insert(student);
        return new ApiResponse<string>(true, "Student added successfully.", "student added");
    }

    public ApiResponse<string> UpdateStudent(string studentnumber , student student)
    {

        if (!studentnumber.Equals(student.studentnumber))
        {
            return new ApiResponse<string>(false, "Changing student number is not allowed." , null) ;
        }
        using var db = _dbManager.GetQueryFactory();
        var exists = db.Query("student")
                       .Where("studentnumber", studentnumber)
                       .Exists();

        if (exists)
        {
            db.Query("student")
              .Where("studentnumber", student.studentnumber)
              .Update(student);

            return new ApiResponse<string>(true, "Student updated successfully.", "updated successfully");
        }

        return new ApiResponse<string>(false, "does not exist", null);
    }

    public ApiResponse<string> DeleteStudent(string studentnumber)
    {
        using var db = _dbManager.GetQueryFactory();

        if (studentnumber.Equals("*"))
        {
            db.Query("student").Delete();
            return new ApiResponse<string>(true, "All students deleted successfully.", "deleted all successfully");
        }

        var exists = db.Query("student")
                       .Where("studentnumber", studentnumber)
                       .Exists();

        if (exists)
        {
            db.Query("student")
              .Where("studentnumber", studentnumber)
              .Delete();

            return new ApiResponse<string>(true, "Student deleted successfully.", "deleted successfully");
        }

        return new ApiResponse<string>(false, "does not exist", null);
    }
}