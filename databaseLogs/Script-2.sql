-- ۱. ساخت جدول Student
CREATE TABLE Student (
    StudentNumber VARCHAR(8) NOT NULL PRIMARY KEY,
    Grade FLOAT,
    FirstName VARCHAR(20) NOT NULL,
    LastName VARCHAR(20) NOT NULL,
    IsMale BOOLEAN NOT NULL,
    DateOfBirth TIMESTAMP NOT NULL,
    LeftUnitsCount INT NOT NULL
);

-- ۲. ساخت جدول Enrollment
CREATE TABLE Enrollment (
    CourseName VARCHAR(20),
    ParticipantStudentNumber VARCHAR(8),
    CONSTRAINT studentNumberFK FOREIGN KEY (ParticipantStudentNumber)
    REFERENCES Student(StudentNumber)
);