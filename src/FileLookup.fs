module FileLookup

open System.IO

let find path ext =
    Directory.GetFiles(path, $"*.%s{ext}", SearchOption.AllDirectories)
    
let getFileNames (paths: string array) = 
    paths |> Array.map(fun path -> path |> Path.GetFileName)

let findFileNames path ext = 
    find path ext |> getFileNames