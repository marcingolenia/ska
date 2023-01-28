module FilesCopier

open System.IO

let copyFiles fromPath toPath =
    toPath |> Directory.CreateDirectory
    Directory.GetFiles(fromPath, "*.*", SearchOption.AllDirectories)
    |> Array.filter(fun path -> not <| path.Contains("ska_"))
    |> Array.iter(fun filePath ->
        let pathToBuild = Path.GetDirectoryName(filePath.Replace(fromPath + Path.DirectorySeparatorChar.ToString(), ""))
        Path.Combine(toPath, pathToBuild) |> Directory.CreateDirectory
        let targetPath = Path.Combine(toPath, pathToBuild, Path.GetFileName(filePath))
        File.Copy(filePath, targetPath))
