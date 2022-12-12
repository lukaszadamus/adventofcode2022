var graph = InputParser.Load("input.txt");

Console.WriteLine($"ResultA: {FindA(graph)}");
Console.WriteLine($"ResultB: {FindB(graph)}");

static int FindA(Dictionary<Vertex, List<Vertex>> graph)
{
    var source = graph.Where(x => x.Key.Type == VertexType.Start).First().Key;
    var target = graph.Where(x => x.Key.Type == VertexType.End).Last().Key;

    var result = Dijkstra(graph, source, target);

    return result[target];
}

static int FindB(Dictionary<Vertex, List<Vertex>> graph)
{    
    var target = graph.Where(x => x.Key.Type == VertexType.End).Last().Key;

    var sources = graph.Keys.Where(x => x.Elevation == (int)'a').ToList();

    return sources.Aggregate(new List<int>(), (acc, source) =>
    {
        var result = Dijkstra(graph, source, target);
        if(result[target] > 0)
            acc.Add(result[target]);
        return acc;

    }, acc => acc.Min());
}

static Dictionary<Vertex, int> Dijkstra(Dictionary<Vertex, List<Vertex>> graph, Vertex source, Vertex? target)
{
    var distance = new Dictionary<Vertex, int>();
    distance[source] = 0;

    var pq = new Q();

    foreach (var vertex in graph.Keys)
    {
        if (vertex != source)
            distance[vertex] = int.MaxValue;

        pq.Enqueue(vertex, distance[vertex]);
    }

    while (!pq.Empty())
    {
        var current = pq.Dequeue();

        if (target is not null && current == target)
            return distance;

        foreach (var vertex in graph[current])
        {
            var alt = distance[current] + 1;
            if (alt < distance[vertex])
            {
                distance[vertex] = alt;
                pq.ChangePriority(vertex, alt);
            }
        }
    }

    return distance;
}

record Vertex(int X, int Y, int Elevation, VertexType Type);
enum VertexType { Start, Normal, End }

internal static class InputParser
{
    public static Dictionary<Vertex, List<Vertex>> Load(string path)
    {
        var lines = File.ReadAllLines(path);
        var r = lines.Count();
        var c = lines.First().Length;
        var elevations = new byte[r, c];
        lines.Select((line, y) => (Y: y, Line: line)).Aggregate(elevations, (acc, line) =>
        {
            for (var x = 0; x < line.Line.Length; x++)
            {
                acc[line.Y, x] = line.Line[x] switch
                {
                    'S' => (byte)'a',
                    'E' => (byte)'z',
                    _ => (byte)line.Line[x]

                };
            }
            return acc;
        });

        Vertex startVertex = null;
        Vertex endVertex = null;

        var graph = lines
            .Select((line, y) => (Y: y, Line: line))
            .Aggregate(new Dictionary<Vertex, List<Vertex>>(), (acc, vertex) =>
            {
                for (var x = 0; x < vertex.Line.Length; x++)
                {
                    var v = vertex.Line[x] switch
                    {
                        'S' => new Vertex(x, vertex.Y, (byte)'a', VertexType.Start),
                        'E' => new Vertex(x, vertex.Y, (byte)'z', VertexType.End),
                        _ => new Vertex(x, vertex.Y, (int)vertex.Line[x], VertexType.Normal)
                    };
                    acc.Add(v, new List<Vertex>());
                    if (v.Type == VertexType.Start)
                    {
                        startVertex = v;
                    }
                    else if (v.Type == VertexType.End)
                    {
                        endVertex = v;
                    }

                }

                return acc;
            });

        foreach (var vertex in graph.Keys)
        {
            graph[vertex] = FindNeighbors(vertex, elevations, startVertex, endVertex).OrderByDescending(x => x.Elevation).ToList();
        }

        return graph;
    }

    private static IEnumerable<Vertex> FindNeighbors(Vertex vertex, byte[,] elevations, Vertex start, Vertex end)
    {
        if (vertex.X - 1 >= 0 && elevations[vertex.Y, vertex.X - 1] - vertex.Elevation <= 1)
            yield return new Vertex(vertex.X - 1, vertex.Y, elevations[vertex.Y, vertex.X - 1], FindType(vertex.X - 1, vertex.Y));
        if (vertex.Y - 1 >= 0 && elevations[vertex.Y - 1, vertex.X] - vertex.Elevation <= 1)
            yield return new Vertex(vertex.X, vertex.Y - 1, elevations[vertex.Y - 1, vertex.X], FindType(vertex.X, vertex.Y - 1));
        if (vertex.X + 1 < elevations.GetLength(1) && elevations[vertex.Y, vertex.X + 1] - vertex.Elevation <= 1)
            yield return new Vertex(vertex.X + 1, vertex.Y, elevations[vertex.Y, vertex.X + 1], FindType(vertex.X + 1, vertex.Y));
        if (vertex.Y + 1 < elevations.GetLength(0) && elevations[vertex.Y + 1, vertex.X] - vertex.Elevation <= 1)
            yield return new Vertex(vertex.X, vertex.Y + 1, elevations[vertex.Y + 1, vertex.X], FindType(vertex.X, vertex.Y + 1));

        VertexType FindType(int x, int y)
        {
            if (x == start.X && y == start.Y)
            {
                return VertexType.Start;
            }
            else if (x == end.X && y == end.Y)
            {
                return VertexType.End;
            }
            else
            {
                return VertexType.Normal;
            }
        }
    }
}

internal class Q
{
    private readonly Dictionary<Vertex, int> _elements = new Dictionary<Vertex, int>();

    public void Enqueue(Vertex vertex, int priority)
        => _elements.Add(vertex, priority);


    public Vertex Dequeue()
    {
        var element = _elements.OrderBy(x => x.Value).First().Key;
        _elements.Remove(element);
        return element;
    }

    public void ChangePriority(Vertex vertex, int priority)
        => _elements[vertex] = priority;

    public bool Empty()
        => _elements.Count == 0;
}