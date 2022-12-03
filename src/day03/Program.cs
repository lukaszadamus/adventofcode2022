var input = File.ReadAllLines("input.txt");

var resultA = input.Select(x => P(x.Take(x.Length / 2).Intersect(x.Skip(x.Length / 2)).First())).Sum();

(int sum, List<string> group) acc = (0, new List<string>());

var resultB = input.Aggregate(acc, (a, b) =>
{
    a.group.Add(b);
    if (a.group.Count == 3)
    {
        a.sum += P(a.group[0].Intersect(a.group[1]).Intersect(a.group[2]).First());
        a.group = new List<string>();
    }
    return a;
}, a => a.sum);


Console.WriteLine($"ResultA: {resultA}");
Console.WriteLine($"ResultB: {resultB}");

int P(char acc) => char.IsLower(acc) ? (int)acc - 96 : (int)acc - 38;