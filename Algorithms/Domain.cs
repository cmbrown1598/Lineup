using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms
{
    public class Domain
    {
        public Domain(ScheduleItem[] scheduleItems, PlayerItem[] playerItems,
            PlayerGameAvailabilityItem[] playerGameAvailabilityItems, PlayerPositionItem[] playerPositionItems)
        {
            ScheduleItems = scheduleItems;
            PlayerItems = playerItems;
            PlayerGameAvailabilityItems = playerGameAvailabilityItems;
            PlayerPositionItems = playerPositionItems;
        }

        public ScheduleItem[] ScheduleItems { get; set; }

        public PlayerItem[] PlayerItems { get; set; }

        public PlayerGameAvailabilityItem[] PlayerGameAvailabilityItems { get; set; }
        public PlayerPositionItem[] PlayerPositionItems { get; set; }
    }
}
