(int A, int B) score = (0, 0);
var gameScore = File.ReadAllLines("input.txt").Aggregate(score, (a, b) =>
{    
    score.A += Score(PlayA(b)).p2Score;
    score.B += Score(PlayB(b)).p2Score;
    return score;
});

Console.WriteLine($"ResultA: {score.A}");
Console.WriteLine($"ResultB: {score.B}");

(int p1Score, int p2Score) Score(string round) => (round) switch
{
    ("A A") => (4, 4),
    ("A B") => (1, 8),
    ("A C") => (7, 3),
    ("B B") => (5, 5),
    ("B A") => (8, 1),
    ("B C") => (2, 9),
    ("C C") => (6, 6),
    ("C A") => (3, 7),
    ("C B") => (9, 2),
    _ => (0, 0)
};

string PlayA(string round) => round.Last() switch
{
    'X' => round.Replace('X', 'A'),
    'Y' => round.Replace('Y', 'B'),
    'Z' => round.Replace('Z', 'C'),
    _ => round
};

string PlayB(string round) => round.Last() switch
{
    'X' => round.Replace('X', Lose(round.First())),
    'Y' => round.Replace('Y', round.First()),
    'Z' => round.Replace('Z', Win(round.First())),
    _ => round
};

char Lose(char with) => with switch
{
    'A' => 'C',
    'B' => 'A',
    'C' => 'B',
    _ => with
};

char Win(char with) => with switch
{
    'A' => 'B',
    'B' => 'C',
    'C' => 'A',
    _ => with
};