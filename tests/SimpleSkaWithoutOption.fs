module SimpleSkaWithoutOptions

open System
open System.IO
open Xunit
open FsUnit.Xunit
    module SkaEngine =
        let run (ska: Domain.Ska) (targetPath: string) =
               FilesCopier.copyFiles ska.Path targetPath
               ska.Scripts |> List.iter(fun script -> ScriptRunner.run targetPath script |> ignore)
                
[<Fact>]
let ``Simple nodejs project with express can be scaffolded``() = 
    // Arrange 
    let skas = YamlDao.listBy "skas"
    let nodeSka = skas |> List.find(fun ska -> ska.Name = "Node Backend")
    let toPath =
        Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString())
    // Act
    SkaEngine.run nodeSka toPath
    // Assert
    let copiedFiles =
        Directory.GetFiles(toPath, "*.*")
        |> Array.map(fun filePath -> filePath.Replace($"{toPath}{Path.DirectorySeparatorChar}", ""))
    let node_modules =
        Directory.GetDirectories(toPath, "node_modules")
        |> Array.map(fun filePath -> filePath.Replace($"{toPath}{Path.DirectorySeparatorChar}", ""))
    copiedFiles |> should contain "package.json"
    copiedFiles |> should contain "index.ts"
    node_modules |> should contain "node_modules"

[<Fact>]
let ``Working nodejs with tests and more deps project with express can be scaffolded``() = 
    // Arrange 
    let skas = YamlDao.listBy "skas_apps"
    let nodeSka = skas |> List.find(fun ska -> ska.Name = "Node Backend")
    let toPath =
        Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString())
    // Act
    SkaEngine.run nodeSka toPath
    // Assert
    let copiedFiles =
        Directory.GetFiles(toPath, "*.*")
        |> Array.map(fun filePath -> filePath.Replace($"{toPath}{Path.DirectorySeparatorChar}", ""))
    let node_modules =
        Directory.GetDirectories(toPath, "node_modules")
        |> Array.map(fun filePath -> filePath.Replace($"{toPath}{Path.DirectorySeparatorChar}", ""))
    copiedFiles |> should contain "package.json"
    copiedFiles |> should contain "index.ts" // not working its in the src
    node_modules |> should contain "node_modules"