var moves = ReadMoves();
Console.WriteLine($"ResultA: {Solve(moves, 2)}");
Console.WriteLine($"ResultB: {Solve(moves, 10)}");

int Solve(List<Move> moves, int numberOfKnots)
{
    var knots = new List<Knot>();
    for (int i = 0; i < numberOfKnots; i++)
    {
        knots.Add(new Knot(i == 0 ? "H" : i.ToString(), new List<Position>() { new Position(0, 0) }));
    }
    var result = moves.Aggregate(knots, (acc, move) =>
    {
        Move(acc, move);
        return acc;
    }, r => r.Last().Positions.Distinct().Count());
    return result;
}

void Move(IEnumerable<Knot> knots, Move move)
{
    for (int i = 0; i < move.Steps; i++)
    {
        var currentPosition = knots.First().Positions.Last();
        var newPosition = move.Direction switch
        {
            'R' => new Position(currentPosition.X + 1, currentPosition.Y),
            'L' => new Position(currentPosition.X - 1, currentPosition.Y),
            'U' => new Position(currentPosition.X, currentPosition.Y + 1),
            'D' => new Position(currentPosition.X, currentPosition.Y - 1),
            _ => currentPosition,
        };
        knots.First().Positions.Add(newPosition);
        KeepUp(knots, move.Direction);
    }

    void KeepUp(IEnumerable<Knot> knots, char direction)
    {
        for (int i = 0; i < knots.Count(); i++)
        {
            var pair = knots.Skip(i).Take(2);
            if (pair.Count() == 2)
            {
                Position newPosition = null;

                var head = pair.First().Positions.Last();
                var tail = pair.Last().Positions.Last();

                if (Math.Abs(head.X - tail.X) > 1
                    || Math.Abs(head.Y - tail.Y) > 1)
                {
                    newPosition = new Position(Find(head.X, tail.X), Find(head.Y, tail.Y));
                }
                if (newPosition is not null)
                {
                    pair.Last().Positions.Add(newPosition);
                }
                else
                {
                    break;
                }
            }
        }
    }

    int Find(int head, int tail) => (head - tail) switch
    {
        > 0 => tail + 1,
        < 0 => tail - 1,
        _ => tail,
    };
}

List<Move> ReadMoves()
    => File.ReadAllLines("input.txt")
        .Select(x => x.Split(" "))
        .Select(x => new Move(x[0].First(), int.Parse(x[1])))
        .ToList();

record Position(int X, int Y);
record Knot(string id, List<Position> Positions);
record Move(char Direction, int Steps);
