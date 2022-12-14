Console.WriteLine($"ResultA: {SolveA()}");
Console.WriteLine($"ResultB: {SolveB()}");

int SolveA()
    => InputParser.LoadPairs("input.txt").Where(x => x.Compare() == -1).Select(x => x.Index).Sum(x => x);

int SolveB()
{
    var packets = InputParser.LoadPackets("input.txt");
    packets.Add(new Packet("[[2]]"));
    packets.Add(new Packet("[[6]]"));
    packets.Sort();

    return packets.Select((p, i) => (I: i + 1, P: p.ToString())).Aggregate(1, (acc, b) =>
    {
        if (b.P == "[[2]]" || b.P == "[[6]]")
        {
            acc *= b.I;
        }
        return acc;
    }, acc => acc);
}