module SimpleSkaWithoutOptions

open System
open System.IO
open Xunit
open FsUnit.Xunit
                
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
    let copiedFolders =
        Directory.GetDirectories(toPath, "*.*")
        |> Array.map(fun filePath -> filePath.Replace($"{toPath}{Path.DirectorySeparatorChar}", ""))
    copiedFiles |> should contain "package.json"
    copiedFolders |> should contain "node_modules"
    copiedFolders |> should contain "src"
    copiedFolders |> should contain "tests"

