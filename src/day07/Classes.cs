public class Device
{
    private readonly long _totalDiskSPace;
    public Directory Root { get; set; }
    private Directory CurrDir = null;
    public Dictionary<string, Entry> FlatFileSystem { get; }
    public long UnusedSpace() => _totalDiskSPace - Root.Size();

    public Device(long totalDiskSpace)
    {
        Root = new Directory();
        FlatFileSystem = new Dictionary<string, Entry>();
        FlatFileSystem.Add("/", Root);
        _totalDiskSPace = totalDiskSpace;
    }

    public void Boot(IEnumerable<string> commands)
    {
        foreach (var command in commands)
        {
            RunCmd(command);
        }

        Root.Size();
    }

    public void RunCmd(string command)
    {
        if (command == "$ cd /")
        {
            CurrDir = Root;
        }
        else if (command == "$ cd ..")
        {
            CurrDir = CurrDir.Parent;
        }
        else if (command.StartsWith("$ cd "))
        {
            var ele = command.Split(" ");
            CurrDir = (Directory)CurrDir.Entries.Where(x => x is Directory && x.Name == ele[2]).FirstOrDefault();
        }
        else if (command == "$ ls")
        {
            // ignore for now
        }
        else if (command.StartsWith("dir"))
        {
            var entry = new Directory(CurrDir, command);
            FlatFileSystem.Add(entry.Path, entry);
            CurrDir.Entries.Add(entry);
        }
        else
        {
            var entry = new File(CurrDir, command);
            FlatFileSystem.Add(entry.Path, entry);
            CurrDir.Entries.Add(entry);
        }
    }    
}

public abstract class Entry
{
    public string Path { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Directory Parent { get; set; }
    public abstract long Size();

}

public class Directory : Entry
{
    public Directory()
    {
        Path = "/";
        Name = "/";
        Parent = null;
        Entries = new List<Entry>();
    }

    public Directory(Directory parent, string line)
    {
        Parent = parent;
        Entries = new List<Entry>();
        var ele = line.Split(" ");
        Name = ele[1];
        Path = parent.Path.EndsWith("/") ? $"{parent.Path}{Name}" : $"{parent.Path}/{Name}";
    }

    public List<Entry> Entries { get; set; } = new List<Entry>();

    private long? CachedSize = null;
    public override long Size()
    {

        if (!CachedSize.HasValue)
        {
            CachedSize = 0;

            if (Entries is null)
            {
                CachedSize = 0;
            }
            else
            {
                foreach (var e in Entries)
                {
                    CachedSize += e.Size();
                }
            }
        }
        return CachedSize.Value;
    }
}

public class File : Entry
{
    private readonly long _size = 0;
    public File(Directory parent, string line)
    {
        var ele = line.Split(" ");
        _size = long.Parse(ele[0]);
        Name = ele[1];
        Parent = parent;
        Path = parent.Path.EndsWith("/") ? $"{parent.Path}{Name}" : $"{parent.Path}/{Name}";
    }

    public override long Size() => _size;
}