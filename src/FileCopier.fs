module FilesCopier

open System.IO

let copyFiles (ska: Domain.Ska) toPath =
    toPath |> Directory.CreateDirectory |> ignore
    let dirPath =
        if File.GetAttributes ska.Path = FileAttributes.Directory then
             ska.Path
        else
            Path.GetDirectoryName  ska.Path
    let skaOptionDirPaths = ska.Options |> List.map(fun opt -> Path.GetDirectoryName opt.Path)
    let allOptionsDirPaths =
        Directory.GetFiles(dirPath, "ska_*.yaml", SearchOption.AllDirectories)
        |> Array.filter(fun path -> path <> ska.Path)
        |> Array.map(Path.GetDirectoryName)
        |> Array.toList
    let notSelectedOptionsDirPaths = allOptionsDirPaths |> List.except skaOptionDirPaths
    Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories)
        |> Array.filter(fun path -> (path.Contains "ska_" || notSelectedOptionsDirPaths |> List.exists path.Contains) |> not)
        |> Array.iter(fun filePath ->
            let pathToBuild =
                Path.GetDirectoryName(filePath.Replace(dirPath + Path.DirectorySeparatorChar.ToString(), ""))
            Path.Combine(toPath, pathToBuild) |> Directory.CreateDirectory |> ignore
            let targetPath = Path.Combine(toPath, pathToBuild, Path.GetFileName(filePath))
            File.Copy(filePath, targetPath))
