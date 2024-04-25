using Microsoft.EntityFrameworkCore;
using Roulette.Daten;
using System.Text;


namespace Roulette
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            RouletteContext dbContext = new RouletteContext();
            User UserEinloggen = null;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Roulette von eric");
                UserEinloggen = await UserEinloggenMethode(dbContext);
                if (UserEinloggen != null)
                    break;
            }
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"----------------------------------------------\nRoulette spiel von eric User; {UserEinloggen.Name}\n----------------------------------------------");
                Console.WriteLine("Menu");
                Console.WriteLine("----------------------------------------------");
                Console.WriteLine("1. Roulette Spielen");
                Console.WriteLine("2. Verlauf");
                Console.WriteLine("3. ende");
                string MenuAusWahl = Console.ReadLine();
                switch (MenuAusWahl)
                {
                    case "1":
                        Console.Clear();
                        Random RandomNumberGenerator = new Random();
                        Console.WriteLine($"Aktuelles Guthaben {UserEinloggen.Guthaben}");
                        Console.WriteLine("Wieviel wollen wie wetten?");
                        double Einsatz = Convert.ToDouble(Console.ReadLine());
                        Console.WriteLine("Auf welche farbe setzten sie? (Grün, Rot, Schwarz)");
                        string EinsatzFarbe = Console.ReadLine();
                        int RandomNumber = RandomNumberGenerator.Next(36);
                        string Farbe = null;
                        if(RandomNumber % 2 == 0)
                        {
                            Farbe = "Schwarz";
                            if (RandomNumber == 0)
                            {
                                Farbe = "Grün";
                            }
                            Console.WriteLine($"Das ergebis ist {RandomNumber} {Farbe}");
                        }
                        else
                        {
                            Farbe = "Rot";
                            Console.WriteLine($"Das ergebis ist {RandomNumber} {Farbe}");
                        }
                        double gewinn = 0;
                        if(EinsatzFarbe == Farbe)
                        {
                            if(EinsatzFarbe == "Grün")
                            {
                                gewinn = Einsatz*14;
                                User UserToUpdate = await dbContext.User.FirstOrDefaultAsync(u => u == UserEinloggen); // Angenommen, die ID ist bekannt
                                if (UserToUpdate != null)
                                {
                                    UserToUpdate.Guthaben += Einsatz*14;
                                    await dbContext.SaveChangesAsync();
                                }
                                Console.WriteLine($"Sie haben gewonnen das neue guthaben beträgt {UserEinloggen.Guthaben}");
                            }
                            else
                            {
                                gewinn = Einsatz;
                                User UserToUpdate = await dbContext.User.FirstOrDefaultAsync(u => u == UserEinloggen); // Angenommen, die ID ist bekannt
                                if (UserToUpdate != null)
                                {
                                    UserToUpdate.Guthaben += Einsatz;
                                    await dbContext.SaveChangesAsync();
                                }
                                Console.WriteLine($"Sie haben gewonnen das neue guthaben beträgt {UserEinloggen.Guthaben}");
                            }
                        }
                        else
                        {
                            gewinn -= Einsatz;
                            User UserToUpdate = await dbContext.User.FirstOrDefaultAsync(u => u == UserEinloggen); // Angenommen, die ID ist bekannt
                            if (UserToUpdate != null)
                            {
                                UserToUpdate.Guthaben -= Einsatz;
                                await dbContext.SaveChangesAsync();
                            }
                            Console.WriteLine($"Sie haben verloren das neue guthaben beträgt {UserEinloggen.Guthaben}");
                        }
                        dbContext.Verlauf.Add(new Verlauf { Ergebnis = Farbe, GesetzteFarbe = EinsatzFarbe, Gewinn = gewinn, User = UserEinloggen });
                        await dbContext.SaveChangesAsync();
                        Console.ReadKey();
                        break;
                    case "2":
                        VerlaufAnzeigen(dbContext, UserEinloggen);
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Falsche eingabe!");
                        Console.ReadKey();
                        break;
                }
            }

            static async Task VerlaufAnzeigen(RouletteContext dbContext, User UserEinloggen)
            {
                Console.Clear();
                var AlleVerlaufe = await dbContext.Verlauf
                                                      .Include(v => v.User) // Includiert die Kundendaten
                                                      .Where(v => v.User == UserEinloggen)
                                                      .ToListAsync();
                Console.WriteLine("-----------------------------------------");
                Console.WriteLine("Verlauf:\n");
                foreach (var VerlaufEinzeln in AlleVerlaufe)
                {
                    Console.WriteLine($"ID: {VerlaufEinzeln.Id}, Preis: {VerlaufEinzeln.Gewinn}, GesetzteFarbe: {VerlaufEinzeln.GesetzteFarbe}, Ergebnis: {VerlaufEinzeln.Ergebnis}");
                }
                Console.ReadKey();
            }

            static async Task<User> UserEinloggenMethode(RouletteContext dbContext)
            {
                Console.WriteLine("1) anmeldung");
                Console.WriteLine("2) Neuen account erstellen");
                Console.WriteLine("3) ende");
                string Anmeldewauswahl = Console.ReadLine();
                switch (Anmeldewauswahl)
                {
                    case "1":
                        Console.Write("Geben Sie Ihren Namen ein:");
                        string Name = Console.ReadLine();
                        Console.Write("Geben Sie Ihr Passwort ein:");
                        string Passwort = Console.ReadLine();
                        User benutzerLogin = await dbContext.User.FirstOrDefaultAsync(b => b.Name == Name && b.Passwort == Passwort);
                        if (benutzerLogin == null)
                        {
                            Console.WriteLine("Benutzername oder Passwort ist falsch.");
                            Console.ReadKey();
                            return null;
                        }
                        Console.Clear();
                        return benutzerLogin;
                    case "2":
                        Console.WriteLine("Geben Sie Ihren Namen an:");
                        string NameNeuAccount = Console.ReadLine();
                        Console.WriteLine("Geben Sie Íhr Passwort an:");
                        string passwort = Console.ReadLine();
                        User benutzerNeu = new User { Name = NameNeuAccount, Passwort = passwort, Guthaben = 1500 };
                        dbContext.User.Add(benutzerNeu);
                        dbContext.SaveChanges();
                        Console.Clear();
                        return benutzerNeu;
                    case "3":
                        Environment.Exit(0);
                        return null;
                    default:
                        Console.WriteLine("Falsche Eingabe!");
                        Console.ReadKey();
                        Console.Clear();
                        return null;
                }
            }
        }
    }
}