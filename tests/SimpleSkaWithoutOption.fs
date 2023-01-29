module SimpleSkaWithoutOptions

open System
open System.IO
open Xunit
open FsUnit.Xunit
    module SkaEngine =
        let run (ska: Domain.Ska) (targetPath: string) =
               ()

[<Fact(Skip="Failing test: next todo")>]
let ``Simple nodejs project with express can be scaffolded``() = 
    // Arrange 
    let ska: Domain.Ska = { Name = "NodeJS Express"
                            Path = failwith "skas/node-backend/ska_Node_Backend.yaml"
                            Scripts = [
                                "npm install"
                            ]
                            Options = []
    }
    let toPath =
        Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString())
    // Act
    SkaEngine.run ska toPath
    // Assert
    let copiedFiles =
        Directory.GetFiles(toPath, "*.*", SearchOption.AllDirectories)
        |> Array.map(fun filePath -> filePath.Replace($"{toPath}{Path.DirectorySeparatorChar}", ""))
    copiedFiles |> should contain [| "package.json" |]