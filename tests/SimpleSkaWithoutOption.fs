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
    let ska: Domain.Ska = { Name = "NodeJS Express"
                            Path = Path.Combine("skas","node-backend","ska_Node_Backend.yaml")
                            Scripts = [
                                "npm i"
                            ]
                            Options = []
    }
    let toPath =
        Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString())
    // Act
    SkaEngine.run ska toPath
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
