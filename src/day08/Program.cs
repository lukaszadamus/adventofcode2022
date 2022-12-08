var input = ParseInput();

Console.WriteLine($"ResultA: {SolveA(input.trees, input.maxX, input.maxY)}");
Console.WriteLine($"ResultB: {SolveB(input.trees)}");

int SolveA(List<Tree> trees, int maxX, int maxY)
     => trees.Aggregate(new HashSet<Tree>(), (acc, tree) =>
    {
        if (!acc.Contains(tree))
        {
            if (tree.X == 0 || tree.X == maxX || tree.Y == 0 || tree.Y == maxY)
            {
                acc.Add(tree);
            }
            else
            {
                if (!trees.Any(x => x.X == tree.X && x.Y < tree.Y && x.Height >= tree.Height))
                {
                    acc.Add(tree);
                }
                else if (!trees.Any(x => x.X == tree.X && x.Y > tree.Y && x.Height >= tree.Height))
                {
                    acc.Add(tree);
                }
                else if (!trees.Any(x => x.Y == tree.Y && x.X > tree.X && x.Height >= tree.Height))
                {
                    acc.Add(tree);
                }
                else if (!trees.Any(x => x.Y == tree.Y && x.X < tree.X && x.Height >= tree.Height))
                {
                    acc.Add(tree);
                }
            }
        }
        return acc;
    }, acc => acc.Count);

int SolveB(List<Tree> trees)
     => trees.Aggregate(new Dictionary<Tree, int>(), (acc, tree) =>
    {
        var up = trees.Where(x => x.X == tree.X && x.Y < tree.Y).OrderByDescending(x => x.Y);
        var down = trees.Where(x => x.X == tree.X && x.Y > tree.Y).OrderBy(x => x.Y);
        var left = trees.Where(x => x.Y == tree.Y && x.X < tree.X).OrderByDescending(x => x.X);
        var right = trees.Where(x => x.Y == tree.Y && x.X > tree.X).OrderBy(x => x.X);

        acc.Add(tree, Walk(up) * Walk(down) * Walk(left) * Walk(right));

        int Walk(IEnumerable<Tree> direction)
        {
            var visible = 0;
            foreach (var vTree in direction)
            {
                if (vTree.Height <= tree.Height)
                {
                    visible++;
                }
                if (vTree.Height == tree.Height)
                {
                    break;
                }
            }
            return visible;
        }

        return acc;
    }, acc => acc.Select(x => x.Value).OrderByDescending(x => x).First());

(List<Tree> trees, int maxX, int maxY) ParseInput()
{
    var lines = System.IO.File.ReadAllLines("input.txt");
    var trees = new List<Tree>();
    for (int y = 0; y < lines.Length; y++)
    {
        for (int x = 0; x < lines[y].Length; x++)
        {
            trees.Add(new Tree(x, y, (int)char.GetNumericValue(lines[y][x])));
        }
    }
    return (trees, lines[0].Length - 1, lines.Length - 1);
}


record Tree(int X, int Y, int Height);