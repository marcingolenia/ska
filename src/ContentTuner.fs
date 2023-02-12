module ContentTuner

open System.IO

let removeAnnotations (path: string) =
    let noAnnotation (line: string) = not (line.IndexOf("//+ska_") > -1 || line.IndexOf("//-ska_") > -1)
    File.ReadAllLines path
        |> Array.filter noAnnotation
        |> (fun lines -> File.WriteAllLines(path, lines))

let tune (path: string) =
        Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
        |> Array.iter removeAnnotations 