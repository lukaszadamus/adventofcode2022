using System.Text;

Console.WriteLine($"ResultA: {Solve(Parse(), true)}");
Console.WriteLine($"ResultB: {Solve(Parse(), false)}");

string Solve(Input input, bool moveOneByOne)
{
    foreach (var move in input.Moves)
    {
        for (var i = 0; i < move.Quantity; i += moveOneByOne ? 1 : move.Quantity)
        {
            var take = moveOneByOne ? 1 : move.Quantity;
            input.Stacks[move.To].AddRange(input.Stacks[move.From].TakeLast(take));
            input.Stacks[move.From].RemoveRange(input.Stacks[move.From].Count - take, take);
        }
    }
    return string.Join("", input.Stacks.Select(x => x.Last()));    
}

Input Parse()
{
    var input = new Input(new List<List<char>>(), new List<Move>());
    var lines = File.ReadAllLines("input.txt");

    foreach (var line in lines)
    {
        if (line.StartsWith("move"))
        {
            var moveData = line.Split(" ");
            input.Moves.Add(new Move(int.Parse(moveData[1]), int.Parse(moveData[3]) - 1, int.Parse(moveData[5]) - 1));
        }
        else if (line.StartsWith(" 1") || line.Length == 0)
        {
            continue;
        }
        else
        {
            var skip = 0;
            var take = 4;
            var stackId = 0;
            while (true)
            {
                var subLine = line.Skip(skip).Take(take).ToArray();

                if (subLine.Length == 0)
                {
                    break;
                }

                if (input.Stacks.Count < stackId + 1)
                {
                    input.Stacks.Add(new List<char>());
                }

                if (char.IsLetter(subLine[1]))
                {
                    input.Stacks[stackId].Insert(0, subLine[1]);
                }

                skip += take;
                stackId++;
            }
        }
    }
    return input;
}

record Input(List<List<char>> Stacks, List<Move> Moves);
record Move(int Quantity, int From, int To);