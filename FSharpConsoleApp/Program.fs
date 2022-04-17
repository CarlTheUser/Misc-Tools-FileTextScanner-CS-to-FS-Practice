open System
open System.Linq 
open System.IO

type SearchEntry = { Value: string }

type SearchResult = {
        Entry: SearchEntry;
        Filename: string;
        Line: int;
        Proximity: string[]
    }

let GetPromptValue prompt : string = 
    printf "%s" prompt

    Console.ReadLine()

let scan (entries:SearchEntry[], directory:DirectoryInfo) : seq<SearchResult> =
    
    let files = directory.GetFiles(
        "*.*", 
        SearchOption.TopDirectoryOnly)

    let filesCount = files.Length;

    let getFileMatches (lines : string[], filename : string) : seq<SearchResult> =
        seq { 
                for lineIndex in 0 .. lines.Length - 1 do 
                    for searchEntryIndex in 0 .. entries.Length - 1 do 
                        if lines.[lineIndex].Contains(entries.[searchEntryIndex].Value)
                        then yield ({ 
                                        Entry = entries.[searchEntryIndex]; 
                                        Filename = filename; 
                                        Line = lineIndex + 1; 
                                        Proximity = [| 
                                            lines.[lineIndex-1]; 
                                            lines.[lineIndex]; 
                                            if lineIndex + 1 < lines.Length then lines.[lineIndex + 1]
                                        |] }:SearchResult)
            }

    seq { 
            for index in 0 .. filesCount - 1 do yield! getFileMatches(File.ReadAllLines(files.[index].FullName), files.[index].Name)
        }

/// <summary>
/// If called from cmd or by double clicking .exe file without arguments, program will use current cmd location for searching.
/// If called with "directory-source:args" argument, program will use value from "use-directory:{folder path}" argument for searching.
/// If called with "directory-source:prompt" argument, program will prompt user to provide folder path for searching.
/// </summary>

[<EntryPoint>]
let main args =

    let directorySourceParam = args.FirstOrDefault(fun a -> a.StartsWith("directory-source:"))

    let directory :string =
        match directorySourceParam with
        | "directory-source:args" -> args.First(fun a -> a.StartsWith("use-directory:")).["use-directory:".Length..]
        | "directory-source:prompt" -> GetPromptValue("Enter source directory: ")
        | _ -> Environment.CurrentDirectory

    printfn "Search Directory: %s" directory

    let entriesParam = args.FirstOrDefault(fun a -> a.StartsWith("entries:"))

    let directoryInfo = new DirectoryInfo(directory)

    //(if String.IsNullOrWhiteSpace(entriesParam) 
    //then GetPromptValue("Type entries (;) to search: ").Split(';', StringSplitOptions.RemoveEmptyEntries)
    //else entriesParam.["entries:".Length..].Split(';', StringSplitOptions.RemoveEmptyEntries))
    //|> Array.map(fun (item:string) -> ({ Value = item.Trim() }):SearchEntry)
    //|> Scan directoryInfo 

    let entries = if String.IsNullOrWhiteSpace(entriesParam) 
                    then GetPromptValue("Type entries (;) to search: ").Split(';', StringSplitOptions.RemoveEmptyEntries)
                    else entriesParam.["entries:".Length..].Split(';', StringSplitOptions.RemoveEmptyEntries)

    scan(Array.map(fun (item:string) -> { Value = item.Trim() } : SearchEntry) entries, directoryInfo)
    |> Seq.iter(fun result -> 
         printfn ""

         printfn "Found %s @%s Line: %i." result.Entry.Value result.Filename result.Line
         printfn "..."
         Array.iter(fun i -> printfn "%s" i) result.Proximity
         printfn "..."
         printfn "")

    printfn "Press F to Pay Respects"

    if Char.ToUpper(Console.ReadKey().KeyChar) <> 'F'
    then for i = 1 to 100000 do
            printfn "Press F to Pay Respects" 
    else printfn "Thank you"

    0