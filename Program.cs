using System.ComponentModel;
using System.Net.Http.Headers;

namespace Labb_1___SQL_ADO
{
	internal class Program
	{
		static void Main(string[] args)
		{
			
			while (true)
			{
				Console.WriteLine("Välkommen till skolan\n");
				Console.WriteLine("1) Lägg till ny elev eller personal");
				Console.WriteLine("2) Skriv ut alla elever");
				Console.WriteLine("3) Hämta en klass");
				Console.WriteLine("4) Hämta personal");
				Console.WriteLine("5) Hämta alla betyg");
				Console.WriteLine("6) Snittbetyg per kurs");

                Console.Write("\nMenyval: ");
				int input= int.Parse(Console.ReadLine());

				switch (input)
				{
					case 1:
						//Lägg till ny elev
						while (true)
						{
							input = int.Parse(Console.ReadLine());
							switch (input)
							{
								case 1:
									break;

								case 2:
									break;

								default;
							}
						}
						AddStudent();
						break;

					case 2:
						PrintStudents();
						break;

					case 3:

						break;
						case 4:
						break;
						case 5:
						break;
						case 6:
						break;
				}
            }
		}
	}
}