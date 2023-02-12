module FilesCopier

open System.IO

let copyFiles (fromPath: string) toPath =
    toPath |> Directory.CreateDirectory |> ignore
    let dirPath = if File.GetAttributes fromPath = FileAttributes.Directory then fromPath else Path.GetDirectoryName fromPath
    Directory.GetFiles(
        dirPath
        , "*.*"
        , SearchOption.AllDirectories)
    |> Array.filter(fun path -> not <| path.Contains "ska_")
    |> Array.iter(fun filePath ->
        let pathToBuild = Path.GetDirectoryName(filePath.Replace(dirPath + Path.DirectorySeparatorChar.ToString(), ""))
        Path.Combine(toPath, pathToBuild) |> Directory.CreateDirectory |> ignore
        let targetPath = Path.Combine(toPath, pathToBuild, Path.GetFileName(filePath))
        File.Copy(filePath, targetPath))
