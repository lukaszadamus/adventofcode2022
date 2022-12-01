var input = File.ReadAllLines("input.txt").Aggregate(new List<List<long>>() { new() }, (a, b) =>
{
    if (b.Trim().Length != 0)
    {
        a.Last().Add(long.Parse(b));
    }
    else
    {
        a.Add(new());
    }
    return a;
}, c => c.Select(x => x.Sum()).OrderByDescending(x => x));

Console.WriteLine($"ResultA: {input.First()}");
Console.WriteLine($"ResultB: {input.Take(3).Sum()}");
