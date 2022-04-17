// See https://aka.ms/new-console-template for more information
using CSharpConsoleApp;

static string? GetPromptValue(string prompt)
{
    Console.Write(prompt);

    return Console.ReadLine();
}

/// <summary>
/// If called from cmd or by double clicking .exe file without arguments, program will use current cmd location for searching.
/// If called with "directory-source:args" argument, program will use value from "use-directory:{folder path}" argument for searching.
/// If called with "directory-source:prompt" argument, program will prompt user to provide folder path for searching.
/// </summary>

string? directorySourceParam = args.FirstOrDefault(a => a.StartsWith("directory-source:"));

string directory = directorySourceParam switch
{
    "directory-source:args" => args.First(a => a.StartsWith("use-directory:"))["use-directory:".Length..],
    "directory-source:prompt" => GetPromptValue("Enter source directory: ") ?? throw new Exception("Invalid source directory."),
    _ => Environment.CurrentDirectory
};

Console.WriteLine($"Search Directory: {directory}");

string? entriesParam = args.FirstOrDefault(a => a.StartsWith("entries:"));

string[] searchTerms = string.IsNullOrWhiteSpace(entriesParam) ?
    (GetPromptValue("Type entries (;) to search: ") ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries) :
    entriesParam["entries:".Length..].Split(';', StringSplitOptions.RemoveEmptyEntries);

if (!searchTerms.Any()) return;

var entries = from item in searchTerms
              select new SearchEntry(item.Trim());

DirectoryFirstLevelTextScanner scanner = new();

IEnumerable<SearchResult> results = scanner.RunScan(
    entries: entries,
    source: new DirectoryInfo(directory));

if (results.Any())
{
    Console.WriteLine();
    foreach (var result in results)
    {
        Console.WriteLine($"Found \"{result.Entry.Value}\" @{result.Filename} Line: {result.Line}. ");
        Console.WriteLine("...");
        foreach (var line in result.Proximity)
        {
            Console.WriteLine(line);
        }
        Console.WriteLine("...");
        Console.WriteLine();
    }
}
else
{
    Console.WriteLine("No results found.");
}

Console.WriteLine("Press F to Pay Respects");

if (char.ToUpper(Console.ReadKey().KeyChar) != 'F')
{
    for (int i = 0; i < 100000; ++i)
    {
        Console.WriteLine("Press F to Pay Respects");
    }
}
else Console.WriteLine("Thank You.");