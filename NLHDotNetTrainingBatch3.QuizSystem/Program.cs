using Dapper;
using Microsoft.Data.SqlClient;
using NLHDotNetTrainingBatch3.QuizSystem.Models;
using System;
using System.Data;
using System.Runtime.InteropServices;

namespace NLHDotNetTrainingBatch3.QuizSystem
{
    internal class Program
    {
        static readonly SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder()
        {
            DataSource = "LAPTOP-7EKI2OO3\\SQLEXPRESS",
            InitialCatalog = "QuizDatabase",
            UserID = "mylogin",
            Password = "nyan123",
            TrustServerCertificate = true
        };

        static IDbConnection Connection() => new SqlConnection(sqlConnectionStringBuilder.ConnectionString);

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Quiz App");
            User currentUser = null;

            while (currentUser == null)
            {
                Console.WriteLine("1) Create account");
                Console.WriteLine("2) Login");
                Console.Write("Choose: ");
                var accountChoice = Console.ReadLine()?.Trim();
                if (accountChoice == "1")
                {
                    currentUser = CreateAccount();
                }
                else if (accountChoice == "2")
                {
                    currentUser = LoginAccount();
                }
                else
                {
                    Console.WriteLine("Invalid option. Please choose 1 or 2.");
                }
            }

            string choice;
            while(true)
            {
                Console.WriteLine("1) Attempt quiz");
                Console.WriteLine("2) See the result");
                Console.WriteLine("3) Exit");
                Console.Write("Choose: ");
                choice = Console.ReadLine()?.Trim();
                Console.WriteLine();
                if (choice == "1") AttemptQuiz(currentUser);
                else if (choice == "2") SeeTheResult(currentUser);
                else if (choice == "3") break;
                else
                {
                    Console.WriteLine("Invalid");
                }
            }
        }

        static User CreateAccount()
        {
            Console.Write("Email: ");
            var userEmail = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(userEmail))
            {
                Console.WriteLine("Email required");
                return null;
            }
            var existing = GetUserByEmail(userEmail);
            if (existing != null)
            {
                Console.WriteLine("User with this email already exists.");
                return null;
            }
            Console.Write("Password: ");
            var userPassword = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(userPassword))
            {
                Console.WriteLine("Password required");
                return null;
            }

            int rowAffected = CreateUser(userEmail, userPassword);
            if (rowAffected > 0)
            {
                Console.WriteLine("Successfully created");
                return GetUserByEmail(userEmail);
            }
            else
            {
                Console.WriteLine("Failed to create");
                return null;
            }
        }

        static User LoginAccount()
        {
            Console.Write("Email: ");
            var userEmail = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(userEmail))
            {
                Console.WriteLine("Email required");
                return null;
            }
            var existingAccount = GetUserByEmail(userEmail);
            if (existingAccount == null)
            {
                Console.WriteLine("User with this email does not exist");
                return null;
            }
            Console.Write("Password: ");
            var userPassword = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(userPassword))
            {
                Console.WriteLine("Password required");
                return null;
            }
            if (existingAccount.Password != userPassword)
            {
                Console.WriteLine("Incorrect Password");
                return null;
            }

            return existingAccount;
        }

        static void AttemptQuiz(User user)
        {
            using var db = Connection();
            Subject getSubject = null;
            while (getSubject == null)
            {
                Console.WriteLine("Here are the subjects you can choose");
                foreach (var subject in GetAllSubjects())
                {
                    Console.WriteLine(subject.Name);
                }
                Console.Write("Type a subject name: ");
                var choice = Console.ReadLine()?.Trim();
                getSubject = GetSubjectId(choice);
                if(getSubject is null) Console.WriteLine("Subject not found. Check spelling");
            }

            var questionList = GetAllQuestions(getSubject.SubjectId);

            int score = 0;

            for(int x = 0; x < questionList.Count; x++)
            {
                var currentQuestion = questionList[x];
                Console.WriteLine(currentQuestion.QuestionText);
                Console.WriteLine($"A. {currentQuestion.OptionA}");
                Console.WriteLine($"B. {currentQuestion.OptionB}");
                Console.WriteLine($"C. {currentQuestion.OptionC}");
                Console.WriteLine($"D. {currentQuestion.OptionD}");
                Console.Write("Choose: ");
                var option = Console.ReadLine()?.Trim();
                var charOption = Char.Parse(option.ToLower());
                if (charOption == currentQuestion.CorrectOption)
                {
                    Console.WriteLine("Correct");
                    score++;
                }
                else
                {
                    Console.WriteLine("Incorrect");
                }
                Console.WriteLine(currentQuestion.Explanation);
                Console.WriteLine();
            }

            Console.WriteLine($"You got {score} questions correct.");
            Console.WriteLine();
            SaveResult(user.UserId, getSubject.SubjectId, score);
        }

        static void SeeTheResult(User user)
        {
            using var db = Connection();
            string query = "select * from Results where UserId = @UserId";
            var resultList = db.Query<Result>(query, new { UserId = user.UserId}).ToList();
            if(resultList.Count == 0)
            {
                Console.WriteLine("You have no attempt of quiz.");
                return;
            }
            foreach(var result in resultList)
            {
                Subject subject = GetSubjectName(result.SubjectId);
                Console.WriteLine($"Subject: {subject.Name}, Score: {result.Score}, AttemptedAt: {result.AttemptedAt}");
            }
        }

        static User GetUserByEmail(string email)
        {
            using var db = Connection();
            return db.QueryFirstOrDefault<User>("SELECT * FROM [Users] WHERE Email = @Email", new { Email = email });
        }
        static Subject GetSubjectId(string subject)
        {
            using var db = Connection();
            return db.QueryFirstOrDefault<Subject>("select * from Subjects where Name = @Subject", new {Subject = subject});
        }

        static Subject GetSubjectName(int subjectId)
        {
            using var db = Connection();
            return db.QueryFirstOrDefault<Subject>("select * from Subjects where SubjectId = @SubjectId", new { SubjectId = subjectId });
        }
        static List<Subject> GetAllSubjects()
        {
            using var db = Connection();
            return db.Query<Subject>("SELECT * FROM Subjects ORDER BY Name").ToList();
        }

        static List<Question> GetAllQuestions(int subjectId)
        {
            using var db = Connection();
            string query = "select * from Questions where SubjectId = @SubjectId;";
            return db.Query<Question>(query, new{SubjectId = subjectId}).ToList();
            
        }

        static int CreateUser(string email, string password)
        {
            using var db = Connection();
            var sql = "INSERT INTO [Users] (Email, Password) VALUES (@Email, @Password);";
            return db.Execute(sql, new { Email = email, Password = password });
        }

        static void SaveResult(int UserId, int SubjectId, int Score)
        {
            using var db = Connection();
            var sql = @"INSERT INTO [dbo].[Results]
           ([UserId]
           ,[SubjectId]
           ,[Score]
           ,[AttemptedAt])
     VALUES(@UserId, @SubjectId, @Score, @AttemptedAt)";
            db.Execute(sql, new {UserId = UserId, SubjectId = SubjectId, Score = Score, AttemptedAt = DateTime.Now});
        }
    }
}