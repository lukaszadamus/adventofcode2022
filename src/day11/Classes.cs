public class Game
{
    private readonly bool _reduceWorry;
    public List<Monkey> Monkeys { get; set; }
    public Game(bool reduceWorry = true)
    {
        Monkeys = File.ReadAllLines("input.txt").Chunk(7).Select(x => new Monkey(x)).ToList();
        _reduceWorry = reduceWorry;
    }

    public Game Play(int numberOfRounds)
    {
        for (var i = 0; i < numberOfRounds; i++)
        {
            foreach (var monkey in Monkeys)
            {
                monkey.Play(Monkeys, _reduceWorry);
            }
        }
        return this;
    }
}

public class Monkey
{
    public int Id { get; }
    public List<ulong> Items { get; private set; }
    public Operation Operation { get; private set; }
    public Divisible Test { get; private set; }
    public ulong Inspections { get; private set; } = 0;

    public Monkey(string[] lines)
    {
        Id = int.Parse(lines[0].Split(" ")[1].TrimEnd(':'));
        SetStartingItems(lines[1]);
        SetOperation(lines[2]);
        SetTest(lines.Skip(3).Take(3).ToList());
    }

    public void Play(List<Monkey> monkeys, bool reduceWorry = true)
    {
        while (Items.Count > 0)
        {
            var worryLevel = Items[0];
            Items.RemoveAt(0);

            worryLevel = Operation.Execute(worryLevel);

            if (reduceWorry)
            {
                worryLevel = BeforeTest(worryLevel);
                var throwToId = Test.Execute(worryLevel);

                monkeys.Where(x => x.Id == throwToId).First().Catch(worryLevel);
            }
            else
            {
                var uberDivisor = monkeys.Select(x => x.Test.Divisor).Aggregate(1UL, (acc, b) => acc * b);
                worryLevel = worryLevel % uberDivisor;
                var throwToId = Test.Execute(worryLevel);

                monkeys.Where(x => x.Id == throwToId).First().Catch(worryLevel);
            }

            Inspections++;
        }
        return;
    }

    public void Catch(ulong worryLevel)
    {
        Items.Add(worryLevel);
    }

    private ulong BeforeTest(ulong worryLevel) => (ulong)Math.Floor(worryLevel / 3.0);

    private void SetStartingItems(string line)
    {
        Items = line.Replace("  Starting items: ", "").Split(", ").Select(x => ulong.Parse(x)).ToList();
    }

    private void SetOperation(string line)
    {
        var ele = line.Replace("  Operation: new = ", "").Split(" ");

        if (ele[2] == "old")
        {
            if (ele[1] == "*")
            {
                Operation = new Pow2();
            }
            else
            {
                Operation = new Multiplication(int.Parse(ele[2]));
            }
        }
        else
        {
            if (ele[1] == "*")
            {
                Operation = new Multiplication(int.Parse(ele[2]));
            }
            else
            {
                Operation = new Addition(int.Parse(ele[2]));
            }
        }

    }

    private void SetTest(List<string> lines)
    {
        var ele = lines[0].Replace("  Test: ", "").Split(" ");
        if (ele[0] == "divisible")
        {
            var divisor = ulong.Parse(ele[2]);
            var trueMonkeyId = int.Parse(lines[1].Replace("    If true: ", "").Split(" ").Last());
            var falseMonkeyId = int.Parse(lines[2].Replace("    If false: ", "").Split(" ").Last());
            Test = new Divisible(divisor, trueMonkeyId, falseMonkeyId);
        }
    }
}

public abstract class Operation
{
    public abstract ulong Execute(ulong worryLevel);
}

public class Addition : Operation
{
    private readonly int _term;

    public Addition(int term)
    {
        _term = term;
    }

    public override ulong Execute(ulong worryLevel) => checked(worryLevel + (ulong)_term);
}

public class Multiplication : Operation
{
    private readonly int _factor;

    public Multiplication(int factor)
    {
        _factor = factor;
    }

    public override ulong Execute(ulong worryLevel) => checked(worryLevel * (ulong)_factor);
}

public class Pow2 : Operation
{
    public override ulong Execute(ulong worryLevel) => checked(worryLevel * worryLevel);
}

public class Divisible
{
    public ulong Divisor { get; set; }
    public int TrueMonkeyId { get; set; }
    public int FalseMonkeyId { get; set; }

    public Divisible(ulong divisor, int trueMonkeyId, int falseMonkeyId)
    {
        Divisor = divisor;
        TrueMonkeyId = trueMonkeyId;
        FalseMonkeyId = falseMonkeyId;
    }

    public int Execute(ulong worryLevel)
        => worryLevel % (ulong)Divisor == 0 ? TrueMonkeyId : FalseMonkeyId;
}