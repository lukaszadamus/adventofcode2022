var cpu = new CPU();
cpu.LoadInstructions("input.txt");
var lcd = new LCD();

Console.WriteLine($"ResultA: {SolveA(cpu)}");
Console.WriteLine($"ResultB:");
SolveB(cpu, lcd);

int SolveA(CPU cpu) 
    => cpu.GetSignalStrengthAt(20) 
    + cpu.GetSignalStrengthAt(60) 
    + cpu.GetSignalStrengthAt(100) 
    + cpu.GetSignalStrengthAt(140) 
    + cpu.GetSignalStrengthAt(180) 
    + cpu.GetSignalStrengthAt(220);

void SolveB(CPU cpu, LCD lcd)
    => cpu.DisplayOnLcd(lcd);
