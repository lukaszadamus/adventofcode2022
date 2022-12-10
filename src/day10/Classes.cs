public class CPU
{
    private Dictionary<char, int> Registries = new Dictionary<char, int>() { { 'x', 1 } };
    
    private int CyclesCount = 1;

    private IEnumerable<Instruction> Instructions;

    void Reset()
    {
        Registries['x'] = 1;
        CyclesCount = 1;
    }

    public void DisplayOnLcd(LCD lcd)
    {
        Reset();
        foreach(var instruction in Instructions)
        {
            for(var i = 0; i< instruction.NumberOfCycles; i++)
            {
                lcd.DrawPixel(CyclesCount + i, Registries['x']);                
            }
            instruction.Execute();
        }

        lcd.Display();
    }    

    public int GetSignalStrengthAt(int cycle)
    {
        Reset();
        foreach(var instruction in Instructions)
        {
            for(var i = 0; i< instruction.NumberOfCycles; i++)
            {
                if(CyclesCount + i == cycle)
                {
                    return (CyclesCount + i) * Registries['x'];
                }
            }
            instruction.Execute();
        }
        throw new Exception();
    }

    public void LoadInstructions(string program)
    {
        Instructions = File.ReadAllLines(program).Select(x => Instruction.Create(x, this));
    }

    public void AddCycles(int numberOfCycles)
    {
        CyclesCount += numberOfCycles;
    }

    public void AddRegistry(char register, int v)
    {
        Registries[register] += v;
    }

    public int GetRegistry(char register)
    {
        return Registries[register];
    }
}

public class LCD 
{
    public List<char> Pixels { get; set; } = new List<char>();    

    public LCD()
    {
        Pixels = Enumerable.Repeat(' ', 6 * 40).ToList();
    }

    public void DrawPixel(int cycle, int spritePosition) 
        => Pixels[cycle - 1] = ((cycle - 1) % 40 < spritePosition - 1 || (cycle - 1) % 40 > spritePosition + 1) ? '.' : '#';

    public void Display()
    {
        foreach(var row in Pixels.Chunk(40))
        {
            foreach(var pixel in row)
            {
                Console.Write(pixel);
            }
            Console.WriteLine("");
        }
    }
}

public abstract class Instruction
{
    public abstract int NumberOfCycles { get; }
    public abstract void Execute();   

    public static Instruction Create(string line, CPU cpu) 
    {
        if(line.StartsWith("noop"))
        {
            return new Noop(line, cpu);
        }
        else if(line.StartsWith("add"))
        {
            return new Add(line, cpu);
        }
        throw new ArgumentException();
    }
}

public class Noop : Instruction
{
    private readonly string _line;
    private readonly CPU _cpu;

    public override int NumberOfCycles { get; } = 1;

    public Noop(string line, CPU cpu)
    {        
        _line = line ?? throw new ArgumentNullException(nameof(line));
        _cpu = cpu ?? throw new ArgumentNullException(nameof(cpu));
        
        if (!line.StartsWith("noop"))
        {
            throw new ArgumentException();
        }
    }

    public override void Execute()
    {
        _cpu.AddCycles(NumberOfCycles);
    }
}

public class Add : Instruction
{
    private readonly string _line;
    private readonly CPU _cpu;

    public override int NumberOfCycles { get; } = 2;
    public char Register { get; }
    public int V { get; }

    public Add(string line, CPU cpu)
    {
        _line = line ?? throw new ArgumentNullException(nameof(line));
        _cpu = cpu ?? throw new ArgumentNullException(nameof(cpu));

        if (!line.StartsWith("add"))
        {
            throw new ArgumentException();
        }

        var ele = line.Replace("add", "").Split(" ");
        Register = ele[0].First();
        V = int.Parse(ele[1]);        
    }

    public override void Execute()
    {
        _cpu.AddCycles(NumberOfCycles);
        _cpu.AddRegistry(Register, V);
    }
}