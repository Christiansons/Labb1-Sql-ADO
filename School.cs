using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Labb_1___SQL_ADO
{
	internal class School
	{
		//Connection-string som kopplar upp till databasen
		public static string connectionString = @"Data Source=(localdb)\mssqllocaldb;Initial Catalog=Skolan;Integrated Security=True;Pooling=False;Encrypt=False;";
		public static void AddPerson()
		{
			Console.Clear();

			//Meny som väljer att lägga till en ny student eller personal
			bool showmeny = true;
			while (showmeny)
			{
				Console.WriteLine("1) Lägg till student");
				Console.WriteLine("2) Lägg till Personal");
				Console.Write("\nMenyval: ");
				int input = int.Parse(Console.ReadLine());
				switch (input)
				{
					case 1:
						AddStudent();
						showmeny = false;
						break;

					case 2:
						AddStaff();
						showmeny = false;
						break;

					default:
						Console.Clear();
						Console.WriteLine("Invalid Input\n");
						break;
				} 
			}
		}
		public static void AddStudent()
		{
			Console.Clear ();
			//Variabel för att ta emot StudentID som autoskapas när man skapar en ny student
			int StudentID;

			//Tar emot information om eleven
			Console.Write("Skriv in studentens förnamn: ");
			string firstName = Console.ReadLine();

			Console.Write("Skriv in studentens efternamn: ");
			string lastName = Console.ReadLine();

			//Tar emot ClassID som motsvarar Klassen i min databas
			Console.WriteLine(
				"[1] NET23\n" +
				"[2] UXF22\n");
			Console.Write("Vilken klass går studenten i: ");
			int ClassID = Convert.ToInt32(Console.ReadLine());

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				//Code for inserting a new student, using Scope_Identity and executescalar to retrieve the Newly added students studentID
				string sqlQuery = "" +
					"INSERT INTO Students (FirstName, LastName, ClassID) " +
					"VALUES (@FirstName, @LastName, @ClassID);" +
					"Select Scope_Identity();";

				using (SqlCommand command = new SqlCommand(sqlQuery, connection))
				{
					command.Parameters.AddWithValue("@FirstName", firstName);
					command.Parameters.AddWithValue("@LastName", lastName);
					command.Parameters.AddWithValue("@ClassID", ClassID);
					
					StudentID = Convert.ToInt32(command.ExecuteScalar());
				}

				//While-loop to be able to enroll into multiple courses
				while (true)
				{
					Console.Clear() ;
					Console.WriteLine("Vilka kurser går studenten i? (Du kommer kunna välja flera svar)");
					Console.WriteLine("1) It Tech and Ops");
					Console.WriteLine("2) C# and .NET");
					Console.WriteLine("3) Databases");
					Console.WriteLine("4) Backend Dev");
					Console.WriteLine("5) INGA FLER KURSER");
					Console.Write("\nMenyval: ");
					int.TryParse(Console.ReadLine(), out int CourseID);

					//If-sats som bestämmer vilken kurs eleven ska bli enrollad i, eller avsluta valen
                    if (CourseID == 5)
					{
						break;
					}
					else if (CourseID < 5)
					{
						//Tar emot ett betyg om användaren väljer en kurs
						Console.WriteLine("\nVilket betyg fick eleven på kursen?\n " +
						"Svara i 1-5, där 1 är E och 5 är A (Tryck enter om kursen inte är klar)");
						Console.Write("\nBetyg: ");
						int.TryParse(Console.ReadLine(), out int grade);

						//Tar emot datum studenten började kursen
                        Console.WriteLine("När började eleven kursen? Svara i YYYY-MM-DD");
                        Console.Write("\nSvar:");
						string enrollDate = Console.ReadLine();

						//Metod för att lägga till en student i Enrollments
                        EnrollStudent(CourseID, grade, StudentID, enrollDate, connection);

                        Console.WriteLine("\nKursen tillagd! Tryck enter för att göra ett till val.");
						Console.ReadLine();
                    }
					else
					{
						Console.WriteLine("Invalid input");
					}
				}
			}
		}

		//Code for enrolling new student into classes
		public static void EnrollStudent(int CourseID, int Grade, int StudentID, string EnrollDate, SqlConnection connection)
		{
			string sqlQuery = "" +
				"INSERT INTO Enrollments (StudentID, CourseID, Grade, EnrollDate)" +
				"VALUES (@StudentID, @CourseID, @Grade, @EnrollDate)";
			using (SqlCommand command = new SqlCommand(sqlQuery, connection))
			{
				
				command.Parameters.AddWithValue("@StudentID", StudentID);
				command.Parameters.AddWithValue("@CourseID", CourseID);
				command.Parameters.AddWithValue("@EnrollDate", EnrollDate);
				if (Grade == 0)
				{
					command.Parameters.AddWithValue("@Grade", DBNull.Value);
				}
				else
				{
					command.Parameters.AddWithValue("@Grade", Grade);
				}
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
				
			}
		}
		
		public static void AddStaff()
		{
			Console.Clear();
			//Tar emot information om personalen
			Console.Write("Skriv in personalens förnamn: ");
			string firstName = Console.ReadLine();

			Console.Write("Skriv in personalens efternamn: ");
			string lastName = Console.ReadLine();

			Console.WriteLine(
				"\nVad är personalens roll?\n" +
				"1)Lärare\n" +
				"2)Administration\n" +
				"3)HR");
			Console.Write("\nRole: ");
			int role = Convert.ToInt32(Console.ReadLine());

			//Kod för att lägga till alla klasser läraren har. 
			List<int> classIDs = new List<int>();
			if (role == 1)
			{
				while (true)
				{
					Console.Clear() ;
					Console.WriteLine("Vilken klass är hen lärare för (du kan göra fler val)\n" +
					"1) NET23\n" +
					"2) UXF22\n" +
					"3) INGA FLER VAL");
					Console.Write("\nMenyval: ");
					int choice = Convert.ToInt32(Console.ReadLine());

					if (choice == 1 || choice == 2)
					{
						classIDs.Add(choice);
						
						Console.WriteLine("Klassen tillagd på läraren!\n");
						Console.WriteLine("\nTryck enter för att göra ett till val..");
						Console.ReadKey();
					}
					else if (choice == 3)
					{
						break;
					}

				}
			}

			//Variable to hold staff-id to collect from query using ExecuteScalar and Scope Identity
			int StaffID = 0;

			//Code to add a new staff member
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string sqlQuery = "" +
					"INSERT INTO Staff (FirstName, LastName) " +
					"VALUES (@FirstName, @LastName) " +
					"Select Scope_Identity();";

				using (SqlCommand command = new SqlCommand(sqlQuery, connection))
				{
					command.Parameters.AddWithValue("FirstName", firstName);
					command.Parameters.AddWithValue("LastName", lastName);
					try
					{
						StaffID = Convert.ToInt32(command.ExecuteScalar());
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}


				//Code to assign a staff-member to its role by using the StaffID collected in previous query
				sqlQuery = "" +
					"INSERT INTO AssignedRole (StaffID, RoleID) " +
					"VALUES (@StaffID, @RoleID)";

				using(SqlCommand command = new SqlCommand(sqlQuery, connection))
				{
					command.Parameters.AddWithValue("StaffID", StaffID);
					command.Parameters.AddWithValue("RoleID", role);
					try
					{
						command.ExecuteNonQuery();
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}

				//Code to assign a teacher to his class/classes
				foreach(int ClassID in classIDs)
				{
					AssignTeacher(ClassID, StaffID, connection);
				}
			}
		}

		// Method to assign a teacher for a class
		public static void AssignTeacher(int classID, int StaffID,SqlConnection connection)
		{
			string sqlQuery = "" +
				"INSERT INTO AssignedTeacher (StaffID, ClassID) " +
				"VALUES (@StaffID, @ClassID)";
			using (SqlCommand command = new SqlCommand(sqlQuery, connection))
			{
				command.Parameters.AddWithValue("StaffID", StaffID);
				command.Parameters.AddWithValue("ClassID", classID);
				try
				{
					command.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
		}

		public static void PrintAllStudents()
		{
			Console.Clear();
			string orderBy; //string för att hålla SQL-kod på hur eleverna ska sorteras
			while (true)
			{
				Console.WriteLine("Hur vill du sortera?\n" +
					"\n1) Förnamn fallande" +
					"\n2) Efternamn fallande" +
					"\n3) Förnamn stigande" +
					"\n4) Efternamn stigande");
				Console.Write("\nSvar:");

				Console.Clear();
				int choice = Convert.ToInt32(Console.ReadLine());

				if (choice == 1)
				{
					orderBy = "FirstName DESC";
					break;
				}
				else if (choice == 2)
				{
					orderBy = "LastName DESC";
					break;
				}
				else if (choice == 3)
				{
					orderBy = "FirstName ASC";
					break;
				}
				else if (choice == 4)
				{
					orderBy = "LastName ASC";
					break;
				}
				else
				{
					Console.Clear();
                    Console.WriteLine("Ogiltigt svar!\n");
                }
			}

			//Kod för att skriva ut alla elever sorterade enligt användarens val
            using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string sqlQuery = "" +
					"SELECT FirstName, LastName, ClassName FROM Students " +
					"JOIN Classes ON Students.ClassID = Classes.ClassID " +
					$"ORDER BY {orderBy}"; //Lägger in sql-koden för hur det ska sorteras
				using (SqlCommand command = new SqlCommand(sqlQuery, connection))
				{
					using(SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							string firstName = reader.GetString(reader.GetOrdinal("FirstName")).Trim();
							string lastName = reader.GetString(reader.GetOrdinal("LastName")).Trim();
							string className = reader.GetString(reader.GetOrdinal("ClassName")).Trim();

                            Console.WriteLine($"{firstName} {lastName} Klass: {className}");
                        }
					}
				}
			}
		}

		public static void PrintClass()
		{
			Console.Clear();

			//Kod för att skriva ut en klass
            using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string sqlQuery = "" +
					"SELECT FirstName, LastName, ClassName FROM Students " +
					"RIGHT JOIN Classes ON Students.ClassID = Classes.ClassID " +
					"WHERE Students.ClassID = @ClassID";
				
				using (SqlCommand command = new SqlCommand(sqlQuery, connection))
				{
					//Tar emot ClassID för vilken klass som ska skrivas ut
                    Console.WriteLine(
						"Vilken klass vill du visa:\n" +
						"1)NET23\n" +
						"2)UXF22");
                    Console.Write("Val: ");

                    int.TryParse(Console.ReadLine(), out int input);

					command.Parameters.AddWithValue("ClassID", input);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							string firstName = reader.GetString(reader.GetOrdinal("FirstName")).Trim();
							string lastName = reader.GetString(reader.GetOrdinal("LastName")).Trim();
							string className = reader.GetString(reader.GetOrdinal("ClassName")).Trim();

							Console.WriteLine($"Klass: {className}. Namn:{firstName} {lastName} ");
						}
					}
				}
			}
		}

		public static void PrintStaff()
		{
			//Frågar vilken personal som ska skrivas ut, och tar emot Roll-Id för att lägga in i query
			Console.Clear();
			Console.WriteLine(
				"Vilka av personalen vill du se?\n" +
				"1)Lärare\n" +
				"2)Administration\n" +
				"3)HR");
			Console.Write("\nRole: ");
			int role = Convert.ToInt32(Console.ReadLine());

			
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				//Joinar ihop Staff för att få information om personen
				//Joinar ihop AssignedRole för att få fram vilken roll personalen är länkad till
				//Joinar ihop Roles som beskriver vad rollerna är
				string sqlQuery = "" +
					"SELECT FirstName, LastName, RoleDescription FROM Staff " +
					"JOIN AssignedRole ON Staff.StaffID = AssignedRole.StaffID " + 
					"JOIN Roles ON AssignedRole.RoleID = Roles.RoleID " +
					"WHERE Roles.RoleID = @RoleID";

				using (SqlCommand command = new SqlCommand(sqlQuery, connection))
				{
					command.Parameters.AddWithValue("RoleID", role);

					//Läser av informationen för att skriva ut
					using(SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							string firstName = reader.GetString(reader.GetOrdinal("FirstName")).Trim();
							string lastName = reader.GetString(reader.GetOrdinal("LastName")).Trim();
							string roleDescription = reader.GetString(reader.GetOrdinal("RoleDescription")).Trim();
							Console.WriteLine($"Personalens roll: {roleDescription}. Namn: {firstName} {lastName} ");
						}
					}
				}
			}
		}

		public static void PrintGrades()
		{
			Console.Clear();

			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				//Query som väljer alla kursnamn för att skriva ut dem
				string sqlQuery = "SELECT CourseName FROM Courses";
				connection.Open();

				//Command to get and print all courses for a menu
				using (SqlCommand command = new SqlCommand(sqlQuery, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						int counter = 0;
						//Reader som skriver ut alla kurser
						while (reader.Read())
						{
							counter++; //Lägger till en counter för att få alla kurser i menyform
							string courseName = reader.GetString(reader.GetOrdinal("CourseName")).Trim();
							Console.WriteLine($"{counter}) {courseName}");
						}
					}
				}

				//Tar emot vilken kurs som användaren vill skriva ut
				Console.WriteLine("För vilken kurs vill du se senaste månadens betyg?");
				Console.Write("\nSvar: ");
				int.TryParse(Console.ReadLine(), out int CourseID);

				sqlQuery = "" +
					"SELECT FirstName, LastName, CourseName, Grade, EnrollDate FROM Enrollments " +
					"JOIN Courses ON Enrollments.CourseID = Courses.CourseID " +
					"JOIN Students ON Enrollments.StudentID = Students.StudentID " +
					$"WHERE Enrollments.CourseID = {CourseID} " + //Lägger in vilken kurs som ska skrivas ut
					$"AND EnrollDate > DATEADD(MONTH, -1, CURRENT_TIMESTAMP)"; //Dateadd som tar en bort en månad från dagens datum

				//Command som skriver ut betygen för kursen
				using (SqlCommand command = new SqlCommand(sqlQuery, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							string firstName = reader.GetString(reader.GetOrdinal("FirstName")).Trim();
							string lastName = reader.GetString(reader.GetOrdinal("LastName")).Trim();
							DateTime enrollDate = reader.GetDateTime(reader.GetOrdinal("EnrollDate")); //Hämtar tiden som eleverna blev registrerad i kursen
							int grade;
							//Kollar om grade är null annars sätter den till 10.
							//Valde nummer 10 av slump, då grade endast kan vara 1-5. Är det nummer så finns inga betyg i kursen ännu
							if(!reader.IsDBNull("Grade"))
							{
								grade = reader.GetInt32(reader.GetOrdinal("Grade"));
							}
							else
							{
								grade = 10;
							}
							
							//Skriver ut olika saker beroende på om det fanns några betyg i kursen eller inte
							if (grade == 10)
							{
								Console.WriteLine($"Namn: {firstName} {lastName} Betyg: EJ AVSLUTAD Startade kursen: {enrollDate}");
							}
							else
							{
								Console.WriteLine($"Namn: {firstName} {lastName} Betyg: {grade} Startade kursen: {enrollDate}");
							}
						}
					}
				}
			}
		}

		public static void PrintAverageGrade()
		{
			Console.Clear();

			//Lägger till alla kurser i en lista för att kunna ha en foreach-loop som går igenom betygen för varje kurs
			List<string> courseList = new List<string>()
			{
				"It Tech and Ops",
				"C# and .NET",
				"Databases",
				"Backend Dev"
			};


			//Kod för att skriva ut varje kurs medelvärde, maxvärde, minvärde
			using(SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();

				//Använder en foreach-loop för att göra samma query på varje kurs
				foreach (string course in courseList)
				{
					//Skriver ut kursen och dess betyg
                    Console.WriteLine($"Genomsnitt,högst och lägst betyg i kursen: {course}");
					string sqlQuery = "" +
						"SELECT CourseName, AVG(Grade) as Avg, MAX(Grade) as Max, MIN(Grade) as Min FROM ENROLLMENTS " +
						"RIGHT OUTER JOIN Courses ON Enrollments.CourseID = Courses.CourseID " +
						$"WHERE CourseName = '{course}' " + 
						"GROUP BY CourseName" 
						;
					using(SqlCommand command = new SqlCommand(sqlQuery, connection))
					{
						using(SqlDataReader reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								//Använder en trycatch för att se om det finns tillagda betyg, annars skulle det returnera null
								//Det skulle räcka med ett betyg för att få ut alla värden
								try
								{
									int avgGrade = reader.GetInt32(reader.GetOrdinal("Avg"));
									int maxGrade = reader.GetInt32(reader.GetOrdinal("Max"));
									int minGrade = reader.GetInt32(reader.GetOrdinal("Min"));

									Console.WriteLine($"\nGenomsnitt: {avgGrade}, Högsta betyg: {maxGrade}, Lägsta betyg: {minGrade}");
								}
								catch (Exception ex)
								{
									Console.WriteLine();
									Console.WriteLine("Inga betyg tilldelade kursen ännu!");
								}
								
                            }
						}
					}
                    Console.WriteLine("--------------------------------------\n");
                }
			}
		}
	}
}
