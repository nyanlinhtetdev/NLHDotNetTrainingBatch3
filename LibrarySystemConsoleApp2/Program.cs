// See https://aka.ms/new-console-template for more information

using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace LibrarySystemConsoleApp2
{
    internal class Program
    {
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

        private static SqlConnection CreateConnection()
        {
            var conn = new SqlConnection(Csb.ConnectionString);
            return conn;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Library System (Console + ADO.NET)");
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
                    case "1": StudentMenu(); break;
                    case "2": BookMenu(); break;
                    case "3": BorrowMenu(); break;
                    case "4": ReturnFlow(); break;
                    case "0": return;
                    default: Console.WriteLine("Invalid choice."); break;
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
                case "1": CreateStudentFlow(); break;
                case "2": GetStudentFlow(); break;
                case "3": UpdateStudentFlow(); break;
                case "0": return;
                default: Console.WriteLine("Invalid choice."); break;
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
                case "1": CreateBookFlow(); break;
                case "2": GetBookFlow(); break;
                case "3": UpdateBookFlow(); break;
                case "0": return;
                default: Console.WriteLine("Invalid choice."); break;
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
                case "1": CreateBorrowFlow(); break;
                case "2": GetBorrowFlow(); break;
                case "3": UpdateBorrowFlow(); break;
                case "0": return;
                default: Console.WriteLine("Invalid choice."); break;
            }
        }

        // ===== Student Flows =====
        private static void CreateStudentFlow()
        {
            Console.Write("Name: ");
            var name = ReadRequiredString();
            Console.Write("Contact Number: ");
            var contact = ReadRequiredString();
            Console.Write("Email: ");
            var email = ReadRequiredString();

            using var conn = CreateConnection();
            conn.Open();
            var sql = @"INSERT INTO [Student] (Name, ContactNumber, Email, HasBorrowed)
                        VALUES (@Name, @ContactNumber, @Email, 0);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@ContactNumber", contact);
            cmd.Parameters.AddWithValue("@Email", email);
            var id = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Student created with StudentId = {id}");
        }

        private static void GetStudentFlow()
        {
            var id = ReadInt("StudentId: ");
            var student = GetStudentById(id);
            if (student == null)
            {
                Console.WriteLine("Student not found.");
                return;
            }
            PrintStudent(student);
        }

        private static void UpdateStudentFlow()
        {
            var id = ReadInt("StudentId: ");
            var student = GetStudentById(id);
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

            using var conn = CreateConnection();
            conn.Open();
            using var cmd = new SqlCommand("UPDATE [Student] SET Name=@Name, ContactNumber=@ContactNumber, Email=@Email WHERE StudentId=@Id", conn);
            cmd.Parameters.AddWithValue("@Name", name);
            cmd.Parameters.AddWithValue("@ContactNumber", contact);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Id", id);
            var rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "Student updated." : "No changes applied.");
        }

        // ===== Book Flows =====
        private static void CreateBookFlow()
        {
            Console.Write("Title: ");
            var title = ReadRequiredString();
            Console.Write("Author: ");
            var author = ReadRequiredString();
            var qty = ReadInt("Quantity: ", min: 0);

            using var conn = CreateConnection();
            conn.Open();
            var sql = @"INSERT INTO [Book] (Title, Author, Quantity)
                        VALUES (@Title, @Author, @Quantity);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);";
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Author", author);
            cmd.Parameters.AddWithValue("@Quantity", qty);
            var id = (int)cmd.ExecuteScalar();
            Console.WriteLine($"Book created with BookId = {id}");
        }

        private static void GetBookFlow()
        {
            var id = ReadInt("BookId: ");
            var book = GetBookById(id);
            if (book == null)
            {
                Console.WriteLine("Book not found.");
                return;
            }
            PrintBook(book);
        }

        private static void UpdateBookFlow()
        {
            var id = ReadInt("BookId: ");
            var book = GetBookById(id);
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

            using var conn = CreateConnection();
            conn.Open();
            using var cmd = new SqlCommand("UPDATE [Book] SET Title=@Title, Author=@Author, Quantity=@Quantity WHERE BookId=@Id", conn);
            cmd.Parameters.AddWithValue("@Title", title);
            cmd.Parameters.AddWithValue("@Author", author);
            cmd.Parameters.AddWithValue("@Quantity", qty);
            cmd.Parameters.AddWithValue("@Id", id);
            var rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "Book updated." : "No changes applied.");
        }

        // ===== Borrow Flows =====
        private static void CreateBorrowFlow()
        {
            var studentId = ReadInt("StudentId: ");
            var bookId = ReadInt("BookId: ");

            using var conn = CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                var student = GetStudentById(studentId, conn, tx);
                if (student == null)
                {
                    Console.WriteLine("Student not found.");
                    tx.Rollback();
                    return;
                }
                if (student.HasBorrowed)
                {
                    Console.WriteLine("Student already has an active borrow.");
                    tx.Rollback();
                    return;
                }

                var book = GetBookById(bookId, conn, tx);
                if (book == null)
                {
                    Console.WriteLine("Book not found.");
                    tx.Rollback();
                    return;
                }
                if (book.Quantity <= 0)
                {
                    Console.WriteLine("No copies available.");
                    tx.Rollback();
                    return;
                }

                using (var decCmd = new SqlCommand("UPDATE [Book] SET Quantity = Quantity - 1 WHERE BookId=@BookId AND Quantity > 0", conn, tx))
                {
                    decCmd.Parameters.AddWithValue("@BookId", bookId);
                    var decRows = decCmd.ExecuteNonQuery();
                    if (decRows == 0)
                    {
                        Console.WriteLine("Stock changed; book unavailable.");
                        tx.Rollback();
                        return;
                    }
                }

                var now = DateTime.Now;
                int borrowId;
                using (var insCmd = new SqlCommand(
                    @"INSERT INTO [Borrow] (BookId, StudentId, BorrowDate, ReturnDate, Status)
                      VALUES (@BookId, @StudentId, @BorrowDate, NULL, @Status);
                      SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx))
                {
                    insCmd.Parameters.AddWithValue("@BookId", bookId);
                    insCmd.Parameters.AddWithValue("@StudentId", studentId);
                    insCmd.Parameters.AddWithValue("@BorrowDate", now);
                    insCmd.Parameters.AddWithValue("@Status", StatusBorrowed);
                    borrowId = (int)insCmd.ExecuteScalar();
                }

                using (var stuCmd = new SqlCommand("UPDATE [Student] SET HasBorrowed = 1 WHERE StudentId=@Id", conn, tx))
                {
                    stuCmd.Parameters.AddWithValue("@Id", studentId);
                    var stuRows = stuCmd.ExecuteNonQuery();
                    if (stuRows == 0)
                    {
                        Console.WriteLine("Failed to update student borrow flag.");
                        tx.Rollback();
                        return;
                    }
                }

                tx.Commit();
                Console.WriteLine($"Borrow created. BorrowId = {borrowId}.");
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

            using var conn = CreateConnection();
            conn.Open();

            Borrow borrow = null;
            if (mode == "1")
            {
                var bid = ReadInt("BorrowId: ");
                borrow = GetBorrowById(bid, conn);
            }
            else if (mode == "2")
            {
                var sid = ReadInt("StudentId: ");
                using var cmd = new SqlCommand(
                    @"SELECT TOP (1) * FROM [Borrow] 
                      WHERE StudentId=@StudentId AND Status=@Status 
                      ORDER BY BorrowDate DESC", conn);
                cmd.Parameters.AddWithValue("@StudentId", sid);
                cmd.Parameters.AddWithValue("@Status", StatusBorrowed);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    borrow = MapBorrow(reader);
            }
            else
            {
                Console.WriteLine("Invalid choice.");
                return;
            }

            if (borrow == null)
            {
                Console.WriteLine("Borrow record not found.");
                return;
            }

            PrintBorrowDetails(conn, borrow);
        }

        private static void UpdateBorrowFlow()
        {
            var borrowId = ReadInt("BorrowId: ");
            using var conn = CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                var borrow = GetBorrowById(borrowId, conn, tx);
                if (borrow == null)
                {
                    Console.WriteLine("Borrow not found.");
                    tx.Rollback();
                    return;
                }

                Console.WriteLine("Leave blank to keep current value.");
                Console.Write($"Status ({borrow.Status}) [Borrowed/Returned]: ");
                var status = Console.ReadLine();
                status = string.IsNullOrWhiteSpace(status) ? borrow.Status : status.Trim();

                Console.Write($"BorrowDate ({borrow.BorrowDate:yyyy-MM-dd HH:mm}): ");
                var bdStr = Console.ReadLine();
                Console.Write($"ReturnDate ({(borrow.ReturnDate.HasValue ? borrow.ReturnDate.Value.ToString("yyyy-MM-dd HH:mm") : "null")}): ");
                var rdStr = Console.ReadLine();

                var newBorrowDate = borrow.BorrowDate;
                if (!string.IsNullOrWhiteSpace(bdStr) && DateTime.TryParse(bdStr, out var bdParsed))
                    newBorrowDate = bdParsed;

                DateTime? newReturnDate = borrow.ReturnDate;
                if (!string.IsNullOrWhiteSpace(rdStr))
                {
                    if (rdStr.Equals("null", StringComparison.OrdinalIgnoreCase))
                        newReturnDate = null;
                    else if (DateTime.TryParse(rdStr, out var rdParsed))
                        newReturnDate = rdParsed;
                }

                bool statusChanged = !string.Equals(borrow.Status, status, StringComparison.OrdinalIgnoreCase);

                if (statusChanged)
                {
                    if (string.Equals(status, StatusReturned, StringComparison.OrdinalIgnoreCase) &&
                        !string.Equals(borrow.Status, StatusReturned, StringComparison.OrdinalIgnoreCase))
                    {
                        // Return transition
                        using var updBorrow = new SqlCommand(
                            @"UPDATE [Borrow] SET Status=@Status, ReturnDate=COALESCE(@ReturnDate, GETDATE()), BorrowDate=@BorrowDate 
                              WHERE BorrowId=@BorrowId", conn, tx);
                        updBorrow.Parameters.AddWithValue("@Status", StatusReturned);
                        updBorrow.Parameters.AddWithValue("@ReturnDate", (object?)newReturnDate ?? DBNull.Value);
                        updBorrow.Parameters.AddWithValue("@BorrowDate", newBorrowDate);
                        updBorrow.Parameters.AddWithValue("@BorrowId", borrowId);
                        var rowsBorrow = updBorrow.ExecuteNonQuery();

                        using var updStudent = new SqlCommand("UPDATE [Student] SET HasBorrowed=0 WHERE StudentId=@StudentId", conn, tx);
                        updStudent.Parameters.AddWithValue("@StudentId", borrow.StudentId);
                        var rowsStudent = updStudent.ExecuteNonQuery();

                        using var updBook = new SqlCommand("UPDATE [Book] SET Quantity = Quantity + 1 WHERE BookId=@BookId", conn, tx);
                        updBook.Parameters.AddWithValue("@BookId", borrow.BookId);
                        var rowsBook = updBook.ExecuteNonQuery();

                        tx.Commit();
                        Console.WriteLine($"Borrow updated to Returned. Rows: borrow={rowsBorrow}, student={rowsStudent}, book={rowsBook}");
                        return;
                    }
                    else if (string.Equals(status, StatusBorrowed, StringComparison.OrdinalIgnoreCase) &&
                             !string.Equals(borrow.Status, StatusBorrowed, StringComparison.OrdinalIgnoreCase))
                    {
                        // Borrow transition
                        var student = GetStudentById(borrow.StudentId, conn, tx);
                        if (student == null)
                        {
                            Console.WriteLine("Student not found.");
                            tx.Rollback();
                            return;
                        }
                        if (student.HasBorrowed)
                        {
                            Console.WriteLine("Student already has active borrow.");
                            tx.Rollback();
                            return;
                        }

                        using var decCmd = new SqlCommand("UPDATE [Book] SET Quantity = Quantity - 1 WHERE BookId=@BookId AND Quantity > 0", conn, tx);
                        decCmd.Parameters.AddWithValue("@BookId", borrow.BookId);
                        var decRows = decCmd.ExecuteNonQuery();
                        if (decRows == 0)
                        {
                            Console.WriteLine("No stock available.");
                            tx.Rollback();
                            return;
                        }

                        using var updBorrow = new SqlCommand(
                            @"UPDATE [Borrow] SET Status=@Status, ReturnDate=NULL, BorrowDate=@BorrowDate 
                              WHERE BorrowId=@BorrowId", conn, tx);
                        updBorrow.Parameters.AddWithValue("@Status", StatusBorrowed);
                        updBorrow.Parameters.AddWithValue("@BorrowDate", newBorrowDate);
                        updBorrow.Parameters.AddWithValue("@BorrowId", borrowId);
                        var rowsBorrow = updBorrow.ExecuteNonQuery();

                        using var updStudent = new SqlCommand("UPDATE [Student] SET HasBorrowed=1 WHERE StudentId=@StudentId", conn, tx);
                        updStudent.Parameters.AddWithValue("@StudentId", borrow.StudentId);
                        var rowsStudent = updStudent.ExecuteNonQuery();

                        tx.Commit();
                        Console.WriteLine($"Borrow updated to Borrowed. Rows: borrow={rowsBorrow}, student={rowsStudent}, bookDec={decRows}");
                        return;
                    }
                }

                // No status change
                using var updateCmd = new SqlCommand(
                    @"UPDATE [Borrow] SET BorrowDate=@BorrowDate, ReturnDate=@ReturnDate, Status=@Status WHERE BorrowId=@BorrowId",
                    conn, tx);
                updateCmd.Parameters.AddWithValue("@BorrowDate", newBorrowDate);
                updateCmd.Parameters.AddWithValue("@ReturnDate", (object?)newReturnDate ?? DBNull.Value);
                updateCmd.Parameters.AddWithValue("@Status", status);
                updateCmd.Parameters.AddWithValue("@BorrowId", borrowId);
                var rows = updateCmd.ExecuteNonQuery();

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
            using var conn = CreateConnection();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                Borrow borrow = null;
                using (var cmd = new SqlCommand(
                    @"SELECT TOP (1) * FROM [Borrow] 
                      WHERE StudentId=@StudentId AND Status=@Status 
                      ORDER BY BorrowDate DESC", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@StudentId", studentId);
                    cmd.Parameters.AddWithValue("@Status", StatusBorrowed);
                    using var reader = cmd.ExecuteReader();
                    if (reader.Read())
                        borrow = MapBorrow(reader);
                }

                if (borrow == null)
                {
                    Console.WriteLine("No active borrow found.");
                    tx.Rollback();
                    return;
                }

                var now = DateTime.Now;
                using (var updBorrow = new SqlCommand(
                    "UPDATE [Borrow] SET ReturnDate=@ReturnDate, Status=@Status WHERE BorrowId=@BorrowId", conn, tx))
                {
                    updBorrow.Parameters.AddWithValue("@ReturnDate", now);
                    updBorrow.Parameters.AddWithValue("@Status", StatusReturned);
                    updBorrow.Parameters.AddWithValue("@BorrowId", borrow.BorrowId);
                    updBorrow.ExecuteNonQuery();
                }

                using (var updStudent = new SqlCommand("UPDATE [Student] SET HasBorrowed=0 WHERE StudentId=@StudentId", conn, tx))
                {
                    updStudent.Parameters.AddWithValue("@StudentId", studentId);
                    updStudent.ExecuteNonQuery();
                }

                using (var updBook = new SqlCommand("UPDATE [Book] SET Quantity = Quantity + 1 WHERE BookId=@BookId", conn, tx))
                {
                    updBook.Parameters.AddWithValue("@BookId", borrow.BookId);
                    updBook.ExecuteNonQuery();
                }

                tx.Commit();
                Console.WriteLine("Book returned successfully.");
            }
            catch (Exception ex)
            {
                tx.Rollback();
                Console.WriteLine($"Error processing return: {ex.Message}");
            }
        }

        // ===== Data Access Helpers =====
        private static Student GetStudentById(int id, SqlConnection conn = null, SqlTransaction tx = null)
        {
            bool external = conn != null;
            if (!external)
            {
                conn = CreateConnection();
                conn.Open();
            }
            using var cmd = new SqlCommand("SELECT * FROM [Student] WHERE StudentId=@Id", conn, tx);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            Student result = null;
            if (reader.Read())
                result = MapStudent(reader);
            if (!external)
                conn.Dispose();
            return result;
        }

        private static Book GetBookById(int id, SqlConnection conn = null, SqlTransaction tx = null)
        {
            bool external = conn != null;
            if (!external)
            {
                conn = CreateConnection();
                conn.Open();
            }
            using var cmd = new SqlCommand("SELECT * FROM [Book] WHERE BookId=@Id", conn, tx);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            Book result = null;
            if (reader.Read())
                result = MapBook(reader);
            if (!external)
                conn.Dispose();
            return result;
        }

        private static Borrow GetBorrowById(int id, SqlConnection conn = null, SqlTransaction tx = null)
        {
            bool external = conn != null;
            if (!external)
            {
                conn = CreateConnection();
                conn.Open();
            }
            using var cmd = new SqlCommand("SELECT * FROM [Borrow] WHERE BorrowId=@Id", conn, tx);
            cmd.Parameters.AddWithValue("@Id", id);
            using var reader = cmd.ExecuteReader();
            Borrow result = null;
            if (reader.Read())
                result = MapBorrow(reader);
            if (!external)
                conn.Dispose();
            return result;
        }

        private static Student MapStudent(SqlDataReader r) => new()
        {
            StudentId = r.GetInt32(r.GetOrdinal("StudentId")),
            Name = r.GetString(r.GetOrdinal("Name")),
            ContactNumber = r.GetString(r.GetOrdinal("ContactNumber")),
            Email = r.GetString(r.GetOrdinal("Email")),
            HasBorrowed = r.GetBoolean(r.GetOrdinal("HasBorrowed"))
        };

        private static Book MapBook(SqlDataReader r) => new()
        {
            BookId = r.GetInt32(r.GetOrdinal("BookId")),
            Title = r.GetString(r.GetOrdinal("Title")),
            Author = r.GetString(r.GetOrdinal("Author")),
            Quantity = r.GetInt32(r.GetOrdinal("Quantity"))
        };

        private static Borrow MapBorrow(SqlDataReader r) => new()
        {
            BorrowId = r.GetInt32(r.GetOrdinal("BorrowId")),
            BookId = r.GetInt32(r.GetOrdinal("BookId")),
            StudentId = r.GetInt32(r.GetOrdinal("StudentId")),
            BorrowDate = r.GetDateTime(r.GetOrdinal("BorrowDate")),
            ReturnDate = r.IsDBNull(r.GetOrdinal("ReturnDate")) ? null : r.GetDateTime(r.GetOrdinal("ReturnDate")),
            Status = r.GetString(r.GetOrdinal("Status"))
        };

        private static void PrintBorrowDetails(SqlConnection conn, Borrow borrow)
        {
            var student = GetStudentById(borrow.StudentId, conn);
            var book = GetBookById(borrow.BookId, conn);
            Console.WriteLine($"BorrowId: {borrow.BorrowId}");
            Console.WriteLine($"StudentId: {borrow.StudentId} {(student != null ? $"({student.Name})" : "")}");
            Console.WriteLine($"BookId: {borrow.BookId} {(book != null ? $"({book.Title})" : "")}");
            Console.WriteLine($"BorrowDate: {borrow.BorrowDate:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"ReturnDate: {(borrow.ReturnDate.HasValue ? borrow.ReturnDate.Value.ToString("yyyy-MM-dd HH:mm") : "null")}");
            Console.WriteLine($"Status: {borrow.Status}");
        }

        // ===== Generic Helpers =====
        private static int ReadInt(string prompt, int? min = null, int? max = null)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (int.TryParse(input, out var val))
                {
                    if (min.HasValue && val < min.Value) { Console.WriteLine($"Must be >= {min.Value}"); continue; }
                    if (max.HasValue && val > max.Value) { Console.WriteLine($"Must be <= {max.Value}"); continue; }
                    return val;
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
    }

    // ===== Models =====
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