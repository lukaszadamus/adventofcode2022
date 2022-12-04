var input = File.ReadAllLines("input.txt").Select(x => Parse(x));

Console.WriteLine($"ResultA: {input.Where(x => ConditionA(x)).Count()}");
Console.WriteLine($"ResultB: {input.Where(x => ConditionB(x)).Count()}");

(int start1, int end1, int start2, int end2) Parse(string line)
    => (int.Parse(line.Split(",")[0].Split("-")[0]), int.Parse(line.Split(",")[0].Split("-")[1]),
       int.Parse(line.Split(",")[1].Split("-")[0]), int.Parse(line.Split(",")[1].Split("-")[1]));

bool ConditionA((int start1, int end1, int start2, int end2) ranges)
{
    if (ranges.start1 <= ranges.start2 && ranges.end1 >= ranges.end2) return true;
    if (ranges.start2 <= ranges.start1 && ranges.end2 >= ranges.end1) return true;
    return false;
};

bool ConditionB((int start1, int end1, int start2, int end2) ranges)
{
    if (ranges.end1 < ranges.start2) return false;
    if (ranges.end2 < ranges.start1) return false;
    return true;
};
