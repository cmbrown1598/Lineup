using System.Linq;

namespace Algorithms
{
    public class PlayerItem
    {
        public string Name { get; set; }
        public string Gender { get; set; }

        public bool CanPlayInGame(int gameNumber, PlayerGameAvailabilityItem[] availability)
        {
            return availability.Where(row => row.Name == Name).All(a => a.GameNumber != gameNumber);
        }
    }
}