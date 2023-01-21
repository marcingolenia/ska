module FilesCopier

open System.IO

let copyFiles fromPath toPath =
    let files = Directory.GetFiles(fromPath)
    toPath |> Directory.CreateDirectory
    files
    |> Array.iter(fun file ->
        Directory.CreateDirectory(Path.GetDirectoryName(file))
        let fileName = Path.GetFileName(file)
        if fileName.StartsWith("ska_") then
            ()
        else
            File.Copy(file, Path.Combine(toPath, Path.GetFileName(file))))
