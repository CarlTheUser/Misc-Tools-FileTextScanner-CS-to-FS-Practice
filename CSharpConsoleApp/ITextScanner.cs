using System.Collections;

namespace CSharpConsoleApp
{
    internal interface ITextScanner<in TSource>
    {
        IEnumerable<SearchResult> RunScan(IEnumerable<SearchEntry> entries, TSource source);
    }

    internal class DirectoryFirstLevelTextScanner : ITextScanner<DirectoryInfo>
    {
        public IEnumerable<SearchResult> RunScan(IEnumerable<SearchEntry> entries, DirectoryInfo source)
        {
            FileInfo[] files = source.GetFiles(
                searchPattern: "*.*",
                searchOption: SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                var filename = file.Name;

                var lines = File.ReadAllLines(file.FullName);

                var length = lines.Length;

                for (int i = 0; i < length; ++i)
                {
                    var line = lines[i];

                    foreach (var entry in entries)
                    {
                        if (line.Contains(entry.Value))
                        {
                            yield return entry.Resolve(
                                filename: filename,
                                line: i + 1,
                                proximity: new string[]
                                {
                                    lines[i - 1],
                                    lines[i],
                                    i + 1 < length - 1 ? lines[i + 1] : ""
                                });
                        }
                    }
                }
            }
        }
    }
}
