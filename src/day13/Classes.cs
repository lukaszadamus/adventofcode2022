static class InputParser
{
    public static List<Pair> LoadPairs(string path)
        => File.ReadAllLines(path).Chunk(3).Select((pair, i) => new Pair
        {
            Index = i + 1,
            Left = new Packet(pair[0]),
            Right = new Packet(pair[1]),
        }).ToList();

    public static List<Packet> LoadPackets(string path)
        => File.ReadAllLines(path).Where(x => !string.IsNullOrWhiteSpace(x)).Select((packet, i) => new Packet(packet)).ToList();
}

class Pair
{
    public int Index { get; set; }
    public Packet Left { get; set; }
    public Packet Right { get; set; }

    public int Compare()
    {
        return Left.Data.Compare(Right.Data);
    }
}

class Packet : IComparable<Packet>
{
    public DataList Data { get; set; } = new DataList();
    public Packet(string line)
    {
        var stack = new Stack<DataList>();
        var integerBuffer = string.Empty;
        foreach (var c in line)
        {
            switch (c)
            {
                case '[':
                    stack.Push(new DataList());
                    break;
                case ']':
                    {
                        if (!string.IsNullOrEmpty(integerBuffer))
                        {
                            stack.Peek().Values.Add(new DataInteger() { Value = int.Parse(integerBuffer) });
                            integerBuffer = string.Empty;
                        }

                        var list = stack.Pop();
                        if (stack.Count == 0)
                        {
                            Data = list;
                        }
                        else
                        {
                            stack.Peek().Values.Add(list);
                        }

                        break;
                    }

                default:
                    if (char.IsNumber(c))
                    {
                        integerBuffer += c;
                    }
                    else if (c == ',')
                    {
                        if (!string.IsNullOrEmpty(integerBuffer))
                        {
                            stack.Peek().Values.Add(new DataInteger() { Value = int.Parse(integerBuffer) });
                            integerBuffer = string.Empty;
                        }
                    }

                    break;
            }
        }
    }

    public int CompareTo(Packet? other) => other switch
    {
        null => 1,
        _ => Data.Compare(other.Data)
    };

    public override string ToString() => Data.ToString();
}

abstract class Data
{

}

class DataInteger : Data
{
    public int Value { get; set; }

    public int Compare(DataInteger b)
    {
        if (this.Value < b.Value)
        {
            return -1;
        }
        else if (this.Value > b.Value)
        {
            return 1;
        }
        return 0;
    }

    public DataList AsList()
        => new DataList() { Values = new List<Data>() { new DataInteger { Value = Value } } };

    public override string ToString()
    {
        return Value.ToString();
    }
}

class DataList : Data
{
    public List<Data> Values { get; set; } = new List<Data>();

    public int Compare(DataList b)
    {
        var count = Math.Max(this.Values.Count, b.Values.Count);
        for (var i = 0; i < count; i++)
        {
            var l = this.Values.Skip(i).Take(1).FirstOrDefault();
            var r = b.Values.Skip(i).Take(1).FirstOrDefault();

            var compareResult = 0;

            switch (l)
            {
                case not null when r is null:
                    compareResult = 1;
                    break;
                case null when r is not null:
                    compareResult = -1;
                    break;
            }

            switch (l)
            {
                case DataInteger when r is DataInteger:
                    {
                        compareResult = ((DataInteger)l).Compare((DataInteger)r);
                        break;
                    }

                case DataList when r is DataList:
                    {
                        compareResult = ((DataList)l).Compare((DataList)r);
                        break;
                    }

                case DataInteger when r is DataList:
                    {
                        compareResult = ((DataInteger)l).AsList().Compare((DataList)r);
                        break;
                    }

                case DataList when r is DataInteger:
                    {
                        compareResult = ((DataList)l).Compare(((DataInteger)r).AsList());
                        break;
                    }
            }

            if (compareResult != 0)
            {
                return compareResult;
            }
        }
        return 0;
    }

    public override string ToString()
    {
        return $"[{string.Join(",", Values)}]";
    }
}