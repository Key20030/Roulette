namespace Roulette
{
    internal class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Passwort { get; set; } = null!;
        public double Guthaben { get; set; }
    }
}
