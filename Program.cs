using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace Labb_1___SQL_ADO
{
	internal class Program
	{
		static void Main(string[] args)
		{
            bool showmeny = true;
			while (showmeny)
			{
				Console.WriteLine("Välkommen till skolans databas\n");
				Console.WriteLine("1) Lägg till ny elev eller personal");
				Console.WriteLine("2) Skriv ut alla elever");
				Console.WriteLine("3) Hämta en klass");
				Console.WriteLine("4) Hämta personal");
				Console.WriteLine("5) Skriv ut betygen senaste månaden för en kurs");
				Console.WriteLine("6) Snittbetyg per kurs");
                Console.WriteLine("7) Avsluta");

                Console.Write("\nMenyval: ");
				int input= int.Parse(Console.ReadLine());

				switch (input)
				{
					case 1:
						School.AddPerson();
						break;

					case 2:
						School.PrintAllStudents();
						break;

					case 3:
						School.PrintClass();
						break;

					case 4:
						School.PrintStaff();
						break;

					case 5:
						School.PrintGrades();
						break;

					case 6:
						School.PrintAverageGrade();
						break;

					case 7:
						Environment.Exit(0);
						break;
					default:
						Console.Clear();
						Console.WriteLine("Inget giltigt val!");
						break;
                }

				Console.WriteLine("\nTryck enter för att komma tillbaka till menyn..");
				Console.ReadKey();
				Console.Clear();
            }
		}
	}
}