using System;
using System.IO;
using System.Linq;
using Algorithms;

namespace Solver
{
    class Program
    {
        public static Domain LoadDomain()
        {
            var scheduleItems = File.ReadAllLines("../../../Data/Schedule.csv")
                .Skip(1)
                .Select(a => a.Split(','))
                .Select(b => new ScheduleItem
                {
                    GameNumber = int.Parse(b[0]),
                    Opponent = b[1]
                })
                .ToArray();

            var playerItems = File.ReadAllLines("../../../Data/Player.csv")
                .Skip(1)
                .Select(a => a.Split(','))
                .Select(b => new PlayerItem
                {
                   Name = b[0]
                })
                .ToArray();

            var playerGameAvailabilityItems = File.ReadAllLines("../../../Data/PlayerGameAvailability.csv")
                .Skip(1)
                .Select(a => a.Split(','))
                .Select(b => new PlayerGameAvailabilityItem
                {
                    GameNumber = int.Parse(b[0]),
                    Name = b[1]
                })
                .ToArray();

            var playerPositionItems = File.ReadAllLines("../../../Data/PlayerPosition.csv")
                .Skip(1)
                .Select(a => a.Split(','))
                .Select(b => new PlayerPositionItem
                {
                    Name = b[0],
                    Rank = int.Parse(b[1]),
                    Position = (Position)Enum.Parse(typeof(Position), b[2])
                })
                .ToArray();


            return new Domain(scheduleItems, playerItems, playerGameAvailabilityItems, playerPositionItems);
        }

        static void Main(string[] args)
        {
            var domain = LoadDomain();

            // Now, Assemble and write out your Lineups.
            var solver = new LineupSolver();

            solver.SolvingMode = SolvingMode.OptimizeForSkill;

            var solution = solver.Solve(domain);
            if (solution.IsSolvable)
            {
                foreach (var game in solution.Games)
                {
                    Console.WriteLine(game.Name);
                    Console.WriteLine();
                    foreach (var inning in game.Innings)
                    {
                        Console.WriteLine($"Inning #{inning.Number}");
                        foreach (var position in LineupSolver.AvailablePositions())
                        {
                            Console.WriteLine($"\t{position} - {inning[position]}");
                        }
                    }

                    Console.WriteLine();
                }

                Console.WriteLine("Your lineup is printed. Press Enter to Exit.");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Data provided does not allow for a solution here. Press Enter to Exit.");
                Console.ReadLine();
            }


        }
    }
}
