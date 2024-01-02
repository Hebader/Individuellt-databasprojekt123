using Individuellt_databasprojekt123.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Formats.Asn1;
using System.Globalization;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Individuellt_databasprojekt123
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool Meny = true;

            while (Meny) // Meny körs så länge Meny = true
            {
                Console.WriteLine("Welcome, choose one of the options.");
                Console.WriteLine("1.Display information of all staff.");
                Console.WriteLine("2.Add staff.");
                Console.WriteLine("3.Save student and grade.");
                Console.WriteLine("4.Check how many teachers in each department.");
                Console.WriteLine("5.Show all students and infotmation.");
                Console.WriteLine("6.Show if the courses are active.");
                Console.WriteLine("7.Check monthly salary for the departments.");
                Console.WriteLine("8.Check average salary for the departments");
                Console.WriteLine("9.Show stored procedure.");
                Console.WriteLine("10.Set grade through transaction.");
                Console.WriteLine("11.Exit program.");

                string userInput = Console.ReadLine(); // Användaren matar in en siffra från Menyn

                switch (userInput) //Lägger in siffran från användaren i switch satsen
                {

                    case "1":
                        GetAllStaff();
                        break;
                    case "2":
                        AddStaff();
                        break;
                    case "3":
                        SaveStudentsAndGrades();
                        break;
                    case "4":
                        GetTeachersDepertment();
                        break;
                    case "5":
                        StudentInfo();
                        break;
                    case "6":
                        IsCourseActive();
                        break;
                    case "7":
                        DepartmentSalary();
                        break;
                    case "8":
                        GetAverageSalaryPerDepartment();
                        break;
                    case "9":
                        ShowProcedure();
                        break;
                    case "10":
                        UpdateGradeByTransaction();
                        break;
                    case "11":
                        Meny = false; // Avslutar programmet när meny blir false
                        break;

                    default:
                        Console.WriteLine("incorecct option. Try again.");
                        break;
                }
            }
        }

        static public void GetAllStaff()
        {
            //Skapa anslutningssträng
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=School;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString)) // Skapa anslutning 
            {
                connection.Open(); //Öppna anslutning mot databasen

                //Skapa sql-query för att skriva sql-frågor
                string selectQuery = @"
            SELECT 
                s.FirstName, 
                s.LastName, 
                p.PositionName AS Positions, 
                d.DepartmentName AS Departments,
                DATEDIFF(YEAR, s.StartDate, GETDATE()) AS YearsWorked
            FROM 
                Staff s
            JOIN 
                Positions p ON s.FKPositionID = p.PositionId
            JOIN 
                Departments d ON s.FKdepartmentID = d.DepartmentID"; //Hämtar info från staff, samt joinar en annan tabellerna Positions och Departments

                using (SqlCommand selectCommand = new SqlCommand(selectQuery, connection)) // Skapar sqlcommand
                {
                    using (SqlDataReader reader = selectCommand.ExecuteReader()) //Skapar reader för att skriva ut alla reslutat i sql-queryn, hämtar och kör kommandot
                    {
                        while (reader.Read()) //skriver ut alla reslutat så länge det finns fler rader i reslutatet
                        {
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();
                            string position = reader["Positions"].ToString();
                            string department = reader["Departments"].ToString();
                            int yearsWorked = Convert.ToInt32(reader["YearsWorked"]);

                            Console.WriteLine($"Name: {firstName} {lastName}, Position: {position}, Years worked: {yearsWorked}, Department: {department}");
                        }
                    }
                }

            }
        }
        public static void AddStaff() //Metod för att lägga till användare
        {

            Console.Write("Type in staff info.\nEnter firstname: ");
            string firstName = Console.ReadLine();

            Console.Write("Enter lastname: ");
            string lastName = Console.ReadLine();

            Console.Write("Enter start date (YYYY-MM-DD): ");
            DateTime startDate;
            // Om användaren skriver datumet i fel format
            if (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            { //Konvertarar sträng till DateTime
                Console.WriteLine("Invalid date format. Please enter date in YYYY-MM-DD format.");
                return;
            }

            Console.Write("Enter position ID: ");
            int FKpositionID;
            if (!int.TryParse(Console.ReadLine(), out FKpositionID)) //Om det inmatade talet inte är en siffra
            {
                Console.WriteLine("Invalid position ID. Please enter a valid number ID.");
                return;
            }

            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=School;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //Skapar en query som lägger till ny rad till tabellen Staff
                string addQuery = @" 
            INSERT INTO Staff (FirstName, LastName, StartDate, FKPositionID)
            VALUES (@FirstName, @LastName, @StartDate, @FKPositionID)";

                using (SqlCommand addCommand = new SqlCommand(addQuery, connection))
                {
                    //Kopplar värderna i sql-queryn till de faktiska värderna
                    addCommand.Parameters.AddWithValue("@FirstName", firstName); 
                    addCommand.Parameters.AddWithValue("@LastName", lastName);
                    addCommand.Parameters.AddWithValue("@StartDate", startDate);
                    addCommand.Parameters.AddWithValue("@FKPositionID", FKpositionID);

                    int rowsAffected = addCommand.ExecuteNonQuery(); //lägger till i databasen

                    if (rowsAffected > 0) //om fler än noll rader påverkats i tabellen
                    {
                        Console.WriteLine("New staff added successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to add new staff.");
                    }
                }
            }
        }

        static void SaveStudentsAndGrades()
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=School;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                //Skapar en query som hämtar info om alla studenter, joinar tabllen Classes
                string studentsQuery = @"
             SELECT s.StudentID, s.FirstName, s.LastName, c.ClassName
             FROM Students s
             JOIN Classes c ON s.FKClassID = c.ClassId";

                //Skapar en sqlCommand som innehåller databasen och sql-queryn
                using (SqlCommand command1 = new SqlCommand(studentsQuery, connection))
                   
                {
                    using (SqlDataReader reader = command1.ExecuteReader()) //Exekverar alla reslutat så länge det finns nya rader
                    {
                        Console.WriteLine("All students: ");
                        while (reader.Read())
                        {

                            string studentid = reader["StudentID"].ToString();
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();
                            string className = reader["ClassName"].ToString();

                            Console.WriteLine($"StudentId: {studentid}, Student: {firstName} {lastName}, Class: {className}");
                        }
                    }
                }

                //Query för att lägga till betyg
                string insertGradeQuery = @"
                INSERT INTO Grades (FKStudentID, FKCourseID, Teacher, Grade, Date)
                VALUES (3, 2, 'Daniel', '5', '2023-12-25')"; //Sätter alla värden

                using (SqlCommand command2 = new SqlCommand(insertGradeQuery, connection))
                {

                    int rowsAffected = command2.ExecuteNonQuery(); //Exekverar till tabellen i databasen

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Grade inserted successfully.\n");

                    }
                    else
                    {
                        Console.WriteLine("Failed to insert grade.");
                    }
                }
            }
        }
        static public void GetTeachersDepertment()
        {
            using (var context = new SchoolContext()) //Skapar en instans av SchoolContext
            {
                var teachersDepartment = context.staff
                    .GroupBy(t => t.FkdepartmentId) //Grupperar staff beroende på avdelnings id
                    .Select(g => new //Skapar en ny annonuym variabel "g"
                    {
                        DepartmentId = g.Key, //Få tillgång till nyckeln departmentid som är nyckeln i tabllen
                        TeacherCount = g.Count() //Hämtar alla varje nyckel (departmentid) som skapats av Groupby
                    })
                    .Join( // Joinar Departmens i  i staff
                        context.Departments,
                        p => p.DepartmentId, //id för antal lärare per avdelning i staff
                        d => d.DepartmentId, //id för tabellen Dpertamens
                        (p, d) => new //Annonyma varibaler
                        {
                            DepartmentId = p.DepartmentId, //Tilldelar värdet
                            DepartmentName = d.DepartmentName,
                            TeacherCount = p.TeacherCount
                        }
                    )
                    .ToList(); //Lägger till i listan

                foreach (var department in teachersDepartment) //Skriver ut antal lärare per avdelning
                {
                    Console.WriteLine($"DepartmentId: {department.DepartmentId}, DepartmentName: {department.DepartmentName}, Teachers: {department.TeacherCount}");
                }
            }

        }
        public static void StudentInfo() // Metod för info om strudenter
        {
            using (var context = new SchoolContext()) //Skapar en instans
            {
                var allStudents = context.Students.ToList(); //Skapar en varibael för alla studenter som läggs till i listan

                foreach (var student in allStudents)
                    //Skriver ut all info som finns om stundeterna
                {
                    Console.WriteLine($"Student ID: {student.StudentId}, Name: {student.FirstName} {student.LastName}, BirthDate: {student.BirthDate}, ClassId: {student.FkclassId}");
                }
            }
        }

        public static void IsCourseActive()
        {

            using (var context = new SchoolContext())
            {
                var activeCourses = context.Courses //Skapar vaiabel för tabellen Courses
                    .Where(course => course.IsActive ?? false) //Där course är aktiv/inaktiv skrivs antingen true/false
                    .ToList(); //Lägger till i listan

                foreach (var course in activeCourses) //Skriver ut alla kurser och info
                {
                    Console.WriteLine($"Course ID: {course.CourseId}, Course: {course.CourseName}, Active: {course.IsActive ?? false}");
                }
            }
        }

        public static void DepartmentSalary()
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=School;Integrated Security=True";
            //Skapar anslutningssträng
            using (SqlConnection connection = new SqlConnection(connectionString)) // SKapar anslutning till databasen
            {
                //Skapar en query
                string query = @"
                SELECT
                    d.DepartmentID,
                    d.DepartmentName,
                    SUM(s.Salary) AS TotalMonthSalary
                FROM
                    Departments d
                JOIN
                    Staff s ON d.DepartmentID = s.FKDepartmentID
                GROUP BY
                    d.DepartmentID, d.DepartmentName"; //Summerar hur mycket som betlas ut till de anställda i varje avdelning

                SqlCommand command = new SqlCommand(query, connection); //Skapar en command

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"DepartmentID: {reader["DepartmentID"]}, DepartmentName: {reader["DepartmentName"]}, TotalMonthSalary: {reader["TotalMonthSalary"]}");
                }

                reader.Close();


            }
        }
        static void GetAverageSalaryPerDepartment()
        {
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=School;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                SELECT
                    d.DepartmentID,
                    d.DepartmentName,
                    AVG(s.Salary) AS AverageSalary
                FROM
                    Departments d
                JOIN
                    Staff s ON d.DepartmentID = s.FKDepartmentID
                GROUP BY
                    d.DepartmentID, d.DepartmentName";

                SqlCommand command = new SqlCommand(query, connection); //Skapar sommand och lägger till query och databasen

                SqlDataReader reader = command.ExecuteReader(); //Hämtar alla nya rader som skapas
                Console.WriteLine("Average salary for department:");
                while (reader.Read()) //Så länge det finns rader med resultat
                {
                    Console.WriteLine($"DepartmentID: {reader["DepartmentID"]}, DepartmentName: {reader["DepartmentName"]}, AverageSalary: {reader["AverageSalary"]}");
                }
            }

        }
        public static void ShowProcedure()
        {

            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=School;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open(); // Öpnnar anslutning till databasen

                using (SqlCommand command = new SqlCommand("GetStudentInfo", connection)) //Hämtar procedure som finns i databasen
                {

                    command.CommandType = CommandType.StoredProcedure; // Bestämmer att det är en lagrad procedure som ska köras

                    command.Parameters.AddWithValue("@StudentID", 2); //Lägger till bestämda värden

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command)) // Hämtar data från tidigare skapad command
                    {

                        DataSet dataSet = new DataSet(); //Skapar dataset för att ge möjlighet att fylla på dataset med resultat


                        adapter.Fill(dataSet); //Fyller data set med resultat

                        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0) //Om det är men än 0 tabeller som påverkas, samt om det läggs in resultat för men än 0 tabller
                        {
                            DataRow student = dataSet.Tables[0].Rows[0]; //Hämtar första raden i tabllen 
                            Console.WriteLine($"StudentID: {student["StudentID"]}, " + //Hämtar info om studenten
                                $"Name: {student["FirstName"]} {student["LastName"]}, " +
                                $"BirthDate: {student["BirthDate"]}, " +
                                $"ClassID: {student["FKClassID"]}\n");
                        }
                        else
                        {
                            Console.WriteLine("No student found with the provided ID.");
                        }
                    }
                }
            }

        }
        public static void UpdateGradeByTransaction()
        {
            //Skapar anslutningsträng för att ansluta till databasen i Sql
            string connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=School;Integrated Security=True";

            using (SqlConnection connection = new SqlConnection(connectionString)) //Skapar en sql connection
            {
                connection.Open(); //Öpnnar sql connection

                SqlTransaction transaction = connection.BeginTransaction(); //Påbröjar en transaktion 

                try // Har en try/catch ifall transktionen misslyckas
                {
                    SqlCommand command = connection.CreateCommand(); //Skapar ett Sqlcommand objekt som kopplas till connection
                    command.Transaction = transaction; //Kopplar command till det skapade transaktion objektet

                    int studentId = 1; // Sätter värde på önskad student
                    int courseId = 2;

                 
                    //Kommandot som ska köras när koden exekveras, skapar en sql-fråga
                    command.CommandText = @" 
                            UPDATE Grades
                            SET Grade = '3'
                            WHERE FKStudentID = @StudentID
                            AND FKCourseID = @CourseID";

                    command.Parameters.AddWithValue("@StudentID", studentId); // Tilldelar värden till sql-frågan
                    command.Parameters.AddWithValue("@CourseID", courseId);

                    Console.WriteLine($"Do you want to set grade for studentId: {studentId} in courseId: {courseId}? Enter yes/no.");
                    string answer = Console.ReadLine();
                    int rowsAffected = command.ExecuteNonQuery(); // exekverar mot databasen och reslutat av antal rader som påverkats
                    if (rowsAffected == 0 || answer == "no")
                    {
                        Console.WriteLine("The student is not registered in the specified course.");
                    }
                    else if (answer == "yes" && rowsAffected > 0)
                    {
                        transaction.Commit(); //Om minst en rad påervkats
                        Console.WriteLine($"Setingt grade for studentid:{studentId} in courseid:{courseId}. ");
                        Console.WriteLine("Grade updated successfully.\n"); //Bekräftar transaktionen
                    }

                }
                catch (Exception ex)
                {

                    transaction.Rollback(); //Om transaktionen misslyckas går den tillbaks till ursprunget
                    Console.WriteLine("An error occurred. Transaction rolled back.");
                }
            }
        }

    }

}

    
