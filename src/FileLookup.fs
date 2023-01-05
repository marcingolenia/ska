module FileLookup

open System.IO

let find (path: string): string array =
    let skas = Directory.GetFiles(path, "*.yaml", SearchOption.AllDirectories)
    skas 
    