create database "starAcademy";
CREATE TABLE Student (
    StudentNumber VARCHAR(8) NOT NULL PRIMARY KEY,
    Grade FLOAT,
    FirstName VARCHAR(20) NOT NULL,
    LastName VARCHAR(20) NOT NULL,
    IsMale BOOLEAN NOT NULL,
    DateOfBirth TIMESTAMP NOT NULL,
    LeftUnitsCount INT NOT NULL
);