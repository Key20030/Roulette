namespace Roulette
{
    internal class Verlauf
    {
        public int Id { get; set; }
        public User User { get; set; } = null!;
        public double Gewinn { get; set; }
        public string Ergebnis { get; set; } = null!;
        public string GesetzteFarbe { get; set; } = null!;
    }
}
