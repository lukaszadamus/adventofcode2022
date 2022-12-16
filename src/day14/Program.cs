var entry = new Coords(500, 0);

var cave = InputParser.Load("input.txt");
Console.WriteLine($"ResultA: {Simulate(entry, cave, (s) => s.Y > cave.Abyss)}");

cave = InputParser.Load("input.txt");
Console.WriteLine($"ResultB: {Simulate(entry, cave, (s) => false)}");

long Simulate(Coords entry, Cave cave, Func<Coords, bool> end)
{
    var simulate = true;
    while (simulate)
    {
        var sand = new Coords(entry.X, entry.Y);

        if (cave.Tiles.ContainsKey(entry))
            break;

        while (true)
        {
            if (end(sand))
            {
                simulate = false;
                break;
            }
            
            var down = new Coords(sand.X, sand.Y + 1);
            var left = new Coords(sand.X - 1, sand.Y + 1);
            var right = new Coords(sand.X + 1, sand.Y + 1);


            if (!cave.Tiles.ContainsKey(down) && !IsFloor(down))
            {
                sand = down;
            }
            else if(!cave.Tiles.ContainsKey(left) && !IsFloor(left))
            {
                sand = left;
            }
            else if (!cave.Tiles.ContainsKey(right) && !IsFloor(right))
            {
                sand = right;
            }
            else
            {
                cave.Tiles.Add(sand, 'o');
                break;
            }
        }
    }

    bool IsFloor(Coords sand) => sand.Y == cave.Abyss + 2;

    return cave.Tiles.Where(x => x.Value == 'o').LongCount();
}

internal static class InputParser
{
    public static Cave Load(string path)
    {
        var tiles = LoadTiles(path);
        var maxY = tiles.Keys.Max(x => x.Y);

        return new Cave(tiles, maxY);
    }

    public static Dictionary<Coords, char> LoadTiles(string path)
        => File.ReadAllLines(path).Select(x => x.Split(" -> ")).Aggregate(new Dictionary<Coords, char>(), (acc, path) =>
        {
            Coords start = null;
            Coords stop = null;

            for (var i = 0; i < path.Length; i++)
            {
                var xy = path[i].Split(",").Select(x => int.Parse(x));

                if (i == 0)
                {
                    start = new Coords(xy.First(), xy.Last());
                    continue;
                }
                else
                {
                    stop = new Coords(xy.First(), xy.Last());
                }

                acc.TryAdd(start, '#');
                var delta = GetDelta(start, stop);
                for (var j = 0; j < delta.C; j++)
                {
                    var next = new Coords(start.X + delta.X, start.Y + delta.Y);
                    var added = acc.TryAdd(next, '#');
                    start = next;
                }

                start = stop;
            }
            return acc;

            (int X, int Y, int C) GetDelta(Coords start, Coords stop)
            {
                if (start.X == stop.X)
                {
                    return (0, start.Y < stop.Y ? 1 : -1, Math.Abs(start.Y - stop.Y));
                }
                else if (start.Y == stop.Y)
                {
                    return (start.X < stop.X ? 1 : -1, 0, Math.Abs(start.X - stop.X));
                }
                return (0, 0, 0);
            }
        }, acc => acc);
}

record Cave(Dictionary<Coords, char> Tiles, int Abyss);
record Coords(int X, int Y);