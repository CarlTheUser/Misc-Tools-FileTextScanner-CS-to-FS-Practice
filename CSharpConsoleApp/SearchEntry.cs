namespace CSharpConsoleApp
{
    record SearchEntry(string Value)
    {
        public SearchResult Resolve(string filename, int line, string[] proximity)
        {
            return new SearchResult(this, filename, line, proximity);
        }
    }
}
