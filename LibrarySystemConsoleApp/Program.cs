// See https://aka.ms/new-console-template for more information
using System;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace LibrarySystemConsoleApp
{
    internal class Program
    {
        // Adjust these to match your SQL Server and database
        private static readonly SqlConnectionStringBuilder Csb = new()
        {
            DataSource = "LAPTOP-7EKI2OO3\\SQLEXPRESS",
            InitialCatalog = "LibrarySystem",
            UserID = "mylogin",
            Password = "nyan123",
            TrustServerCertificate = true
        };

        private const string StatusBorrowed = "Borrowed";
        private const string StatusReturned = "Returned";

        private static IDbConnection CreateConnection() => new SqlConnection(Csb.ConnectionString);

        static void Main(string[] args)
        {
            Console.WriteLine("Library System (Console + Dapper)");
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Main Menu");
                Console.WriteLine("1) Student");
                Console.WriteLine("2) Book");
                Console.WriteLine("3) Borrow");
                Console.WriteLine("4) Return");
                Console.WriteLine("0) Exit");
                Console.Write("Choose: ");
                var choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        StudentMenu();
                        break;
                    case "2":
                        BookMenu();
                        break;
                    case "3":
                        BorrowMenu();
                        break;
                    case "4":
                        ReturnFlow();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
        }

        // ===== Menus =====

        private static void StudentMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Student Menu");
            Console.WriteLine("1) Create new student");
            Console.WriteLine("2) Get student info");
            Console.WriteLine("3) Update student info");
            Console.WriteLine("0) Back");
            Console.Write("Choose: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    CreateStudentFlow();
                    break;
                case "2":
                    GetStudentFlow();
                    break;
                case "3":
                    UpdateStudentFlow();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private static void BookMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Book Menu");
            Console.WriteLine("1) Create new book");
            Console.WriteLine("2) Get book info");
            Console.WriteLine("3) Update book info");
            Console.WriteLine("0) Back");
            Console.Write("Choose: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    CreateBookFlow();
                    break;
                case "2":
                    GetBookFlow();
                    break;
                case "3":
                    UpdateBookFlow();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        private static void BorrowMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Borrow Menu");
            Console.WriteLine("1) Create borrow (Student borrows a book)");
            Console.WriteLine("2) Get borrow info");
            Console.WriteLine("3) Update borrow info");
            Console.WriteLine("0) Back");
            Console.Write("Choose: ");
            var choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    CreateBorrowFlow();
                    break;
                case "2":
                    GetBorrowFlow();
                    break;
                case "3":
                    UpdateBorrowFlow();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        // ===== Student flows =====

        private static void CreateStudentFlow()
        {
            Console.Write("Name: ");
            var name = ReadRequiredString();
            Console.Write("Contact Number: ");
            var contact = ReadRequiredString();
            Console.Write("Email: ");
            var email = ReadRequiredString();

            using var db = CreateConnection();
            var sql = @"
INSERT INTO [Student] (Name, ContactNumber, Email, HasBorrowed)
VALUES (@Name, @ContactNumber, @Email, 0);
SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = db.ExecuteScalar<int>(sql, new { Name = name, ContactNumber = contact, Email = email });
            Console.WriteLine($"Student created with StudentId = {id}");
        }

        private static void GetStudentFlow()
        {
            var studentId = ReadInt("StudentId: ");
            using var db = CreateConnection();
            var student = db.QueryFirstOrDefault<Student>("SELECT * FROM [Student] WHERE StudentId = @Id", new { Id = studentId });
            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }
            PrintStudent(student);
        }

        private static void UpdateStudentFlow()
        {
            var studentId = ReadInt("StudentId: ");
            using var db = CreateConnection();
            var student = db.QueryFirstOrDefault<Student>("SELECT * FROM [Student] WHERE StudentId = @Id", new { Id = studentId });
            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }
            Console.WriteLine("Leave blank to keep current value.");
            Console.Write($"Name ({student.Name}): ");
            var name = Console.ReadLine();
            Console.Write($"Contact Number ({student.ContactNumber}): ");
            var contact = Console.ReadLine();
            Console.Write($"Email ({student.Email}): ");
            var email = Console.ReadLine();

            name = string.IsNullOrWhiteSpace(name) ? student.Name : name.Trim();
            contact = string.IsNullOrWhiteSpace(contact) ? student.ContactNumber : contact.Trim();
            email = string.IsNullOrWhiteSpace(email) ? student.Email : email.Trim();

            var rows = db.Execute(
                "UPDATE [Student] SET Name=@Name, ContactNumber=@ContactNumber, Email=@Email WHERE StudentId=@Id",
                new { Name = name, ContactNumber = contact, Email = email, Id = studentId });

            Console.WriteLine(rows > 0 ? "Student updated." : "No changes applied.");
        }

        // ===== Book flows =====

        private static void CreateBookFlow()
        {
            Console.Write("Title: ");
            var title = ReadRequiredString();
            Console.Write("Author: ");
            var author = ReadRequiredString();
            var qty = ReadInt("Quantity: ", min: 0);

            using var db = CreateConnection();
            var sql = @"
INSERT INTO [Book] (Title, Author, Quantity)
VALUES (@Title, @Author, @Quantity);
SELECT CAST(SCOPE_IDENTITY() as int);";
            var id = db.ExecuteScalar<int>(sql, new { Title = title, Author = author, Quantity = qty });
            Console.WriteLine($"Book created with BookId = {id}");
        }

        private static void GetBookFlow()
        {
            var bookId = ReadInt("BookId: ");
            using var db = CreateConnection();
            var book = db.QueryFirstOrDefault<Book>("SELECT * FROM [Book] WHERE BookId = @Id", new { Id = bookId });
            if (book == null)
            {
                Console.WriteLine("Book not found.");
                return;
            }
            PrintBook(book);
        }

        private static void UpdateBookFlow()
        {
            var bookId = ReadInt("BookId: ");
            using var db = CreateConnection();
            var book = db.QueryFirstOrDefault<Book>("SELECT * FROM [Book] WHERE BookId = @Id", new { Id = bookId });
            if (book == null)
            {
                Console.WriteLine("Book not found.");
                return;
            }
            Console.WriteLine("Leave blank to keep current value.");
            Console.Write($"Title ({book.Title}): ");
            var title = Console.ReadLine();
            Console.Write($"Author ({book.Author}): ");
            var author = Console.ReadLine();
            Console.Write($"Quantity ({book.Quantity}): ");
            var qtyStr = Console.ReadLine();

            var qty = book.Quantity;
            if (!string.IsNullOrWhiteSpace(qtyStr) && int.TryParse(qtyStr, out var parsed) && parsed >= 0)
                qty = parsed;

            title = string.IsNullOrWhiteSpace(title) ? book.Title : title.Trim();
            author = string.IsNullOrWhiteSpace(author) ? book.Author : author.Trim();

            var rows = db.Execute(
                "UPDATE [Book] SET Title=@Title, Author=@Author, Quantity=@Quantity WHERE BookId=@Id",
                new { Title = title, Author = author, Quantity = qty, Id = bookId });

            Console.WriteLine(rows > 0 ? "Book updated." : "No changes applied.");
        }

        // ===== Borrow flows =====

        private static void CreateBorrowFlow()
        {
            var studentId = ReadInt("StudentId: ");
            var bookId = ReadInt("BookId: ");

            using var db = CreateConnection();
            db.Open();
            using var tx = db.BeginTransaction();

            try
            {
                var student = db.QueryFirstOrDefault<Student>(
                    "SELECT * FROM [Student] WHERE StudentId=@StudentId",
                    new { StudentId = studentId }, tx);

                if (student == null)
                {
                    Console.WriteLine("Student not found.");
                    tx.Rollback();
                    return;
                }
                if (student.HasBorrowed)
                {
                    Console.WriteLine("This student already has an active borrow and cannot borrow another book.");
                    tx.Rollback();
                    return;
                }

                var book = db.QueryFirstOrDefault<Book>(
                    "SELECT * FROM [Book] WHERE BookId=@BookId",
                    new { BookId = bookId }, tx);

                if (book == null)
                {
                    Console.WriteLine("Book not found.");
                    tx.Rollback();
                    return;
                }
                if (book.Quantity <= 0)
                {
                    Console.WriteLine("No copies available for this book.");
                    tx.Rollback();
                    return;
                }

                // Decrement quantity with safety check to avoid negative
                var decRows = db.Execute(
                    "UPDATE [Book] SET Quantity = Quantity - 1 WHERE BookId = @BookId AND Quantity > 0",
                    new { BookId = bookId }, tx);

                if (decRows == 0)
                {
                    Console.WriteLine("Failed to decrement quantity (no available stock).");
                    tx.Rollback();
                    return;
                }

                var now = DateTime.Now;

                var borrowId = db.ExecuteScalar<int>(@"
INSERT INTO [Borrow] (BookId, StudentId, BorrowDate, ReturnDate, Status)
VALUES (@BookId, @StudentId, @BorrowDate, NULL, @Status);
SELECT CAST(SCOPE_IDENTITY() as int);",
                    new
                    {
                        BookId = bookId,
                        StudentId = studentId,
                        BorrowDate = now,
                        Status = StatusBorrowed
                    }, tx);

                var stuRows = db.Execute(
                    "UPDATE [Student] SET HasBorrowed = 1 WHERE StudentId = @StudentId",
                    new { StudentId = studentId }, tx);

                if (stuRows == 0)
                {
                    Console.WriteLine("Failed to update student state.");
                    tx.Rollback();
                    return;
                }

                tx.Commit();
                Console.WriteLine($"Borrow created. BorrowId = {borrowId}. Book quantity decremented and student marked as borrowed.");
            }
            catch (Exception ex)
            {
                tx.Rollback();
                Console.WriteLine($"Error creating borrow: {ex.Message}");
            }
        }

        private static void GetBorrowFlow()
        {
            Console.WriteLine("Lookup by:");
            Console.WriteLine("1) BorrowId");
            Console.WriteLine("2) Active borrow by StudentId");
            Console.Write("Choose: ");
            var mode = Console.ReadLine()?.Trim();

            using var db = CreateConnection();

            if (mode == "1")
            {
                var borrowId = ReadInt("BorrowId: ");
                var borrow = db.QueryFirstOrDefault<Borrow>(
                    "SELECT * FROM [Borrow] WHERE BorrowId=@BorrowId",
                    new { BorrowId = borrowId });

                if (borrow == null)
                {
                    Console.WriteLine("Borrow record not found.");
                    return;
                }

                PrintBorrowDetails(db, borrow);
            }
            else if (mode == "2")
            {
                var studentId = ReadInt("StudentId: ");
                var borrow = db.QueryFirstOrDefault<Borrow>(
                    "SELECT TOP (1) * FROM [Borrow] WHERE StudentId=@StudentId AND Status=@Status ORDER BY BorrowDate DESC",
                    new { StudentId = studentId, Status = StatusBorrowed });

                if (borrow == null)
                {
                    Console.WriteLine("No active borrow found for this student.");
                    return;
                }

                PrintBorrowDetails(db, borrow);
            }
            else
            {
                Console.WriteLine("Invalid choice.");
            }
        }

        private static void UpdateBorrowFlow()
        {
            var borrowId = ReadInt("BorrowId: ");
            using var db = CreateConnection();
            db.Open();
            using var tx = db.BeginTransaction();

            try
            {
                var borrow = db.QueryFirstOrDefault<Borrow>(
                    "SELECT * FROM [Borrow] WHERE BorrowId=@BorrowId",
                    new { BorrowId = borrowId }, tx);

                if (borrow == null)
                {
                    Console.WriteLine("Borrow record not found.");
                    tx.Rollback();
                    return;
                }

                Console.WriteLine("Leave blank to keep current value.");
                Console.Write($"Status ({borrow.Status}) [Borrowed/Returned]: ");
                var status = Console.ReadLine();
                status = string.IsNullOrWhiteSpace(status) ? borrow.Status : status.Trim();

                Console.Write($"BorrowDate ({borrow.BorrowDate:yyyy-MM-dd HH:mm}): ");
                var borrowDateStr = Console.ReadLine();

                Console.Write($"ReturnDate ({(borrow.ReturnDate.HasValue ? borrow.ReturnDate.Value.ToString("yyyy-MM-dd HH:mm") : "null")}): ");
                var returnDateStr = Console.ReadLine();

                var newBorrowDate = borrow.BorrowDate;
                if (!string.IsNullOrWhiteSpace(borrowDateStr) && DateTime.TryParse(borrowDateStr, out var bd))
                    newBorrowDate = bd;

                DateTime? newReturnDate = borrow.ReturnDate;
                if (!string.IsNullOrWhiteSpace(returnDateStr))
                {
                    if (returnDateStr.Equals("null", StringComparison.OrdinalIgnoreCase))
                        newReturnDate = null;
                    else if (DateTime.TryParse(returnDateStr, out var rd))
                        newReturnDate = rd;
                }

                // If status is toggled, maintain invariants for Student.HasBorrowed and Book.Quantity
                if (!string.Equals(borrow.Status, status, StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals(status, StatusReturned, StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(borrow.Status, StatusReturned, StringComparison.OrdinalIgnoreCase))
                    {
                        // Moving to Returned: ensure student flag off and quantity increment
                        var rowsBorrow = db.Execute(
                            "UPDATE [Borrow] SET Status=@Status, ReturnDate = COALESCE(@ReturnDate, GETDATE()), BorrowDate=@BorrowDate WHERE BorrowId=@BorrowId",
                            new { Status = StatusReturned, ReturnDate = newReturnDate ?? DateTime.Now, BorrowDate = newBorrowDate, BorrowId = borrowId }, tx);

                        var rowsStudent = db.Execute(
                            "UPDATE [Student] SET HasBorrowed = 0 WHERE StudentId = @StudentId",
                            new { StudentId = borrow.StudentId }, tx);

                        var rowsBook = db.Execute(
                            "UPDATE [Book] SET Quantity = Quantity + 1 WHERE BookId = @BookId",
                            new { BookId = borrow.BookId }, tx);

                        tx.Commit();
                        Console.WriteLine($"Borrow updated to Returned. Rows: borrow={rowsBorrow}, student={rowsStudent}, book={rowsBook}");
                        return;
                    }
                    else if (string.Equals(status, StatusBorrowed, StringComparison.OrdinalIgnoreCase) &&
                             !string.Equals(borrow.Status, StatusBorrowed, StringComparison.OrdinalIgnoreCase))
                    {
                        // Moving to Borrowed: ensure constraints (student must not already have one, book stock > 0)
                        var student = db.QueryFirstOrDefault<Student>(
                            "SELECT * FROM [Student] WHERE StudentId=@Id", new { Id = borrow.StudentId }, tx);
                        if (student == null)
                        {
                            Console.WriteLine("Student not found.");
                            tx.Rollback();
                            return;
                        }
                        if (student.HasBorrowed)
                        {
                            Console.WriteLine("Student already has an active borrow; cannot set another borrow to Borrowed.");
                            tx.Rollback();
                            return;
                        }

                        var decRows = db.Execute(
                            "UPDATE [Book] SET Quantity = Quantity - 1 WHERE BookId = @BookId AND Quantity > 0",
                            new { BookId = borrow.BookId }, tx);

                        if (decRows == 0)
                        {
                            Console.WriteLine("No stock available to set this record back to Borrowed.");
                            tx.Rollback();
                            return;
                        }

                        var rowsBorrow = db.Execute(
                            "UPDATE [Borrow] SET Status=@Status, ReturnDate = NULL, BorrowDate=@BorrowDate WHERE BorrowId=@BorrowId",
                            new { Status = StatusBorrowed, BorrowDate = newBorrowDate, BorrowId = borrowId }, tx);

                        var rowsStudent = db.Execute(
                            "UPDATE [Student] SET HasBorrowed = 1 WHERE StudentId = @StudentId",
                            new { StudentId = borrow.StudentId }, tx);

                        tx.Commit();
                        Console.WriteLine($"Borrow updated to Borrowed. Rows: borrow={rowsBorrow}, student={rowsStudent}, bookDec={decRows}");
                        return;
                    }
                }

                // If status unchanged, just update dates/fields
                var rows = db.Execute(
                    "UPDATE [Borrow] SET BorrowDate=@BorrowDate, ReturnDate=@ReturnDate, Status=@Status WHERE BorrowId=@BorrowId",
                    new { BorrowDate = newBorrowDate, ReturnDate = newReturnDate, Status = status, BorrowId = borrowId }, tx);

                tx.Commit();
                Console.WriteLine(rows > 0 ? "Borrow updated." : "No changes applied.");
            }
            catch (Exception ex)
            {
                tx.Rollback();
                Console.WriteLine($"Error updating borrow: {ex.Message}");
            }
        }

        private static void ReturnFlow()
        {
            var studentId = ReadInt("StudentId: ");

            using var db = CreateConnection();
            db.Open();
            using var tx = db.BeginTransaction();

            try
            {
                // Find active borrow
                var borrow = db.QueryFirstOrDefault<Borrow>(
                    "SELECT TOP (1) * FROM [Borrow] WHERE StudentId=@StudentId AND Status=@Status ORDER BY BorrowDate DESC",
                    new { StudentId = studentId, Status = StatusBorrowed }, tx);

                if (borrow == null)
                {
                    Console.WriteLine("No active borrow found for this student.");
                    tx.Rollback();
                    return;
                }

                var now = DateTime.Now;

                var rowsBorrow = db.Execute(
                    "UPDATE [Borrow] SET ReturnDate=@ReturnDate, Status=@Status WHERE BorrowId=@BorrowId",
                    new { ReturnDate = now, Status = StatusReturned, BorrowId = borrow.BorrowId }, tx);

                var rowsStudent = db.Execute(
                    "UPDATE [Student] SET HasBorrowed = 0 WHERE StudentId = @StudentId",
                    new { StudentId = studentId }, tx);

                var rowsBook = db.Execute(
                    "UPDATE [Book] SET Quantity = Quantity + 1 WHERE BookId = @BookId",
                    new { BookId = borrow.BookId }, tx);

                tx.Commit();
                Console.WriteLine($"Book returned. Rows: borrow={rowsBorrow}, student={rowsStudent}, book={rowsBook}");
            }
            catch (Exception ex)
            {
                tx.Rollback();
                Console.WriteLine($"Error processing return: {ex.Message}");
            }
        }

        // ===== Helpers =====

        private static int ReadInt(string prompt, int? min = null, int? max = null)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (int.TryParse(input, out int value))
                {
                    if (min.HasValue && value < min.Value) { Console.WriteLine($"Must be >= {min}"); continue; }
                    if (max.HasValue && value > max.Value) { Console.WriteLine($"Must be <= {max}"); continue; }
                    return value;
                }
                Console.WriteLine("Invalid number.");
            }
        }

        private static string ReadRequiredString()
        {
            while (true)
            {
                var s = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                Console.Write("Value required. Try again: ");
            }
        }

        private static void PrintStudent(Student s)
        {
            Console.WriteLine($"StudentId: {s.StudentId}");
            Console.WriteLine($"Name: {s.Name}");
            Console.WriteLine($"ContactNumber: {s.ContactNumber}");
            Console.WriteLine($"Email: {s.Email}");
            Console.WriteLine($"HasBorrowed: {s.HasBorrowed}");
        }

        private static void PrintBook(Book b)
        {
            Console.WriteLine($"BookId: {b.BookId}");
            Console.WriteLine($"Title: {b.Title}");
            Console.WriteLine($"Author: {b.Author}");
            Console.WriteLine($"Quantity: {b.Quantity}");
        }

        private static void PrintBorrowDetails(IDbConnection db, Borrow borrow)
        {
            var student = db.QueryFirstOrDefault<Student>("SELECT * FROM [Student] WHERE StudentId=@Id", new { Id = borrow.StudentId });
            var book = db.QueryFirstOrDefault<Book>("SELECT * FROM [Book] WHERE BookId=@Id", new { Id = borrow.BookId });

            Console.WriteLine($"BorrowId: {borrow.BorrowId}");
            Console.WriteLine($"StudentId: {borrow.StudentId} {(student != null ? $"({student.Name})" : "")}");
            Console.WriteLine($"BookId: {borrow.BookId} {(book != null ? $"({book.Title})" : "")}");
            Console.WriteLine($"BorrowDate: {borrow.BorrowDate:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"ReturnDate: {(borrow.ReturnDate.HasValue ? borrow.ReturnDate.Value.ToString("yyyy-MM-dd HH:mm") : "null")}");
            Console.WriteLine($"Status: {borrow.Status}");
        }
    }

    // ===== Models (match table columns) =====

    internal class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = "";
        public string ContactNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public bool HasBorrowed { get; set; }
    }

    internal class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int Quantity { get; set; }
    }

    internal class Borrow
    {
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public int StudentId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } = "";
    }
}
