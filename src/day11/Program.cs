var resultA = new Game().Play(20).Monkeys.Select(x => x.Inspections).OrderByDescending(x => x).Take(2).Aggregate(1UL, (acc, b) => acc * b);
Console.WriteLine($"ResultA: {resultA}");

var resultB = new Game(false).Play(10000).Monkeys.Select(x => x.Inspections).OrderByDescending(x => x).Take(2).Aggregate(1UL, (acc, b) => acc * b);
Console.WriteLine($"ResultB: {resultB}");