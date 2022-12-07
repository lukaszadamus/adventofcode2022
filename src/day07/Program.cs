var commands = System.IO.File.ReadAllLines("input.txt");

var device = new Device(70000000);
device.Boot(commands);

var resultA = device.FlatFileSystem.Where(x => x.Value is Directory && x.Value.Size() < 100000).Select(x => x.Value.Size()).Sum();

var freeUpSpace = 30000000 - device.UnusedSpace();
var resultB = device.FlatFileSystem.Where(x => x.Value is Directory && x.Value.Size() >= freeUpSpace ).Select(x => x.Value.Size()).OrderBy(x => x).First();

Console.WriteLine($"ResultA: {resultA}");
Console.WriteLine($"ResultB: {resultB}");