var input = File.ReadAllText("input.txt");
Console.WriteLine($"ResultA: {Solve(input, 4)}");
Console.WriteLine($"ResultB: {Solve(input, 14)}");

int Solve(string input, int bufferLength)
{
    var processed = 0;
    List<char> buffer = new();
    foreach (var c in File.ReadAllText("input.txt"))
    {
        processed++;
        buffer.Add(c);
        if (buffer.Count == bufferLength)
        {
            if (!buffer.GroupBy(x => x).Select(x => x.Count()).Any(x => x > 1))
            {
                return processed;

            }
            buffer.RemoveAt(0);
        }
    }
    return processed;
}