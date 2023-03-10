module YamlDao

open System.IO
open Domain
open FSharp.Configuration
open OsDetect

type Config = YamlConfig<"ref_config.yaml">

let private listScripts (scripts: Config.scripts_Type) =
    (match os with
     | Linux -> scripts.Linux
     | Windows -> scripts.Windows
     | MacOs -> scripts.MacOS
     |> Seq.toList)
    @ (scripts.All |> Seq.toList)
    |> List.filter(fun script -> script <> "")

let private (|ExtendsPath|_|) (potentialParentPath: string option) (path: string) =
    let extendsPath =
        potentialParentPath.IsSome
        && path.Contains($"{Directory.GetParent(potentialParentPath.Value).Name}{Path.DirectorySeparatorChar}")
    if extendsPath then
        Some ExtendsPath
    else
        None

let toNaturalName (path: string) = Path.GetFileNameWithoutExtension(path).Replace("ska_", "").Replace("_", " ")
let toFileName name = $"ska_{name}.yaml".Replace(" ", "_")

let listBy path =
    let sortedSkasPaths =
        Directory.GetFiles(path, "*.yaml", SearchOption.AllDirectories)
        |> Array.toList
        |> List.sortBy(fun path -> Directory.GetParent(path).FullName)
    ([], sortedSkasPaths)
    ||> List.fold(fun acc skaPath ->
        let lastSka = acc |> List.tryLast
        let lastPath = lastSka |> Option.map(fun ska -> ska.Path)
        let skaYaml = Config()
        skaYaml.Load(skaPath)
        let scripts = skaYaml.scripts |> listScripts
        let skaToAppend =
            { Name = skaPath |> toNaturalName
              Variables =
                skaYaml.variables
                |> Seq.map(fun variable -> variable.name, variable.value)
                |> Map.ofSeq
              Path = skaPath
              Scripts = scripts
              Options = [] }
        match skaPath with
        | ExtendsPath lastPath ->
            let completeSkas = acc[0 .. (acc.Length - 2)]
            completeSkas
            @ [ { lastSka.Value with Options = lastSka.Value.Options @ [ skaToAppend ] } ]
        | _ -> acc @ [ skaToAppend ])
