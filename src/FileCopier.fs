module FilesCopier

open System.IO

let createPathAndCopyFile (skaDirectory: string) (toPath: string) (fromPath: string) =
    let pathToBuild =
        Path.GetDirectoryName(fromPath.Replace(skaDirectory + Path.DirectorySeparatorChar.ToString(), ""))
    Path.Combine(toPath, pathToBuild) |> Directory.CreateDirectory |> ignore
    let targetPath = Path.Combine(toPath, pathToBuild, Path.GetFileName(fromPath))
    File.Copy(fromPath, targetPath)

let copyFiles (ska: Domain.Ska) toPath =
    toPath |> Directory.CreateDirectory |> ignore
    let skaDirectory = Path.GetDirectoryName ska.Path
    let skaOptionDirPaths =
        ska.Options |> List.map(fun opt -> Path.GetDirectoryName opt.Path)
    let allOptionsDirPaths =
        Directory.GetFiles(skaDirectory, "ska_*.yaml", SearchOption.AllDirectories)
        |> Array.filter(fun path -> path <> ska.Path)
        |> Array.map(Path.GetDirectoryName)
        |> Array.toList
    let notSelectedOptionsDirPaths = allOptionsDirPaths |> List.except skaOptionDirPaths
    Directory.GetFiles(skaDirectory, "*.*", SearchOption.AllDirectories)
    |> Array.filter(fun path ->
        (path.Contains "ska_" || notSelectedOptionsDirPaths |> List.exists path.Contains)
        |> not)
    |> Array.iter(createPathAndCopyFile skaDirectory toPath)
