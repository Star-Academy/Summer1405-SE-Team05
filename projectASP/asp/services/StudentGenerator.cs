using asp.models;

namespace asp.helpers;


public static class StudentGenerator
{
    private static readonly Random _random = new Random();

    private static readonly List<string> FirstNames = new List<string>
    {
        "علی", "محمد", "حسین", "رضا", "امیر", "مهدی", "سارینا", "مریم",
        "زهرا", "فاطمه", "نیلوفر", "پویا", "آرش", "کیارش", "دانیال", "سینا",
        "مهرنوش", "یلدا", "سارا", "مهدیه"
    };

    private static readonly List<string> LastNames = new List<string>
    {
        "احمدی", "رضایی", "کریمی", "محمدی", "حسینی", "قاسمی", "ابراهیمی", "شریفی",
        "صادقی", "اکبری", "نوری", "حمیدی", "وفایی", "نجفی", "کاملی", "موسوی",
        "زارع", "راد", "حیدری", "موسوی"
    };

    public static student GenerateRandomStudent()
    {
        // English Comment: Generate a random 8-digit student number starting with 99 or 400
        string prefix = _random.Next(0, 2) == 0 ? "9911" : "4001";
        string randomSuffix = _random.Next(1000, 9999).ToString();
        string studentNumber = prefix + randomSuffix;

        // English Comment: Randomly pick name and last name
        string firstName = FirstNames[_random.Next(FirstNames.Count)];
        string lastName = LastNames[_random.Next(LastNames.Count)];

        // English Comment: Random grade between 8.00 and 20.00 with 2 decimal places
        float grade = (float)Math.Round(_random.NextDouble() * (20.0 - 8.0) + 8.0, 2);

        // English Comment: Random gender
        bool isMale = _random.Next(0, 2) == 1;

        // English Comment: Random left units count between 0 and 120
        int leftUnitsCount = _random.Next(0, 121);

        // English Comment: Random birth date between years 1998 and 2004
        DateTime startBirthDate = new DateTime(1998, 1, 1);
        int rangeDays = (new DateTime(2004, 12, 31) - startBirthDate).Days;
        DateTime dateOfBirth = startBirthDate.AddDays(_random.Next(rangeDays));

        return new student
        {
            studentnumber = studentNumber,
            firstname = firstName,
            lastname = lastName,
            grade = grade,
            ismale = isMale,
            leftunitscount = leftUnitsCount,
            dateofbirth = dateOfBirth
        };
    }
}