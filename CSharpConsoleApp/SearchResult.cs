namespace CSharpConsoleApp
{
    record SearchResult(
        SearchEntry Entry,
        string Filename,
        int Line,
        string[] Proximity);
}
