module ContentTuner

open System.IO
open System.Text

let removeSkas (ska: Domain.Ska) (path: string) =
    let selectedSkaOptionIds =
        ska.Options |> List.map(fun opt -> Path.GetFileNameWithoutExtension opt.Path)
    let indexedLines = File.ReadAllLines path |> Array.indexed
    let allSkasInFile =
        indexedLines
        |> Array.filter(fun (_, line) -> line.Contains "//+ska_" or line.Contains("//-ska_"))
        |> Array.map(fun (lineNo, line) -> (lineNo, line[3..]))
    let linesNumbersToRemove =
        allSkasInFile
        |> Array.filter(fun (_, ska) -> selectedSkaOptionIds |> List.contains ska |> not)
        |> Array.pairwise
        |> Array.mapi(fun i pair -> i % 2 = 0, pair)
        |> Array.filter fst
        |> Array.map snd
        |> Array.map(fun (skaStart, skaEnd) -> [| skaStart |> fst .. skaEnd |> fst |])
        |> Array.collect id
    let newContent = StringBuilder()
    indexedLines
        |> Array.iter(fun (lineNo, line) ->
            if linesNumbersToRemove |> Array.contains lineNo || (line.IndexOf("//+ska_") > -1 || line.IndexOf("//-ska_") > -1)
                then ()
                else newContent.AppendLine line |> ignore)
    File.WriteAllText(path, newContent.ToString())

let tune (ska: Domain.Ska) (path: string) =
    Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
    |> Array.iter(removeSkas ska)
