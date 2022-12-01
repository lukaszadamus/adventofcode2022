var input = File.ReadAllLines("input.txt").Aggregate(new List<List<long>>() { new List<long>() }, (a, b) =>
{
    if (b.Trim().Length == 0)
    {
        a.Add(new());
    }
    else
    {
        a.Last().Add(long.Parse(b));
    }
    return a;
}, c => c);

var resultA = input.Select(x => x.Sum()).OrderByDescending(x => x).First();
Console.WriteLine($"ResultA: {resultA}");

var resultB = input.Select(x => x.Sum()).OrderByDescending(x => x).Take(3).Sum();
Console.WriteLine($"ResultA: {resultB}");
