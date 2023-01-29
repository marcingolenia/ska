module SourceFilesCopier

open System
open System.IO
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Ska: All files of the ska directory should be copied except ska definition`` () =
    // Arrange
    let toPath =
        Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString())
    let fromPath = Path.Combine("skas", "go-backend")
    // Act
    FilesCopier.copyFiles fromPath toPath
    // Assert
    Directory.GetFiles toPath
    |> Array.map(fun file -> file |> Path.GetFileName)
    |> should equal [| "papa.json"; "root.go" |]

[<Fact>]
let ``Ska: All subdirectories that don't start with "ska_" including files should be copied`` () =
    // Arrange
    let toPath =
        Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString())
    let fromPath = Path.Combine("skas", "go-backend")
    // Act
    FilesCopier.copyFiles fromPath toPath
    // Assert
    let copiedFiles =
        Directory.GetFiles(toPath, "*.*", SearchOption.AllDirectories)
        |> Array.map(fun filePath -> filePath.Replace($"{toPath}{Path.DirectorySeparatorChar}", ""))
    copiedFiles |> should equal [| "papa.json"; "root.go"; "subdir/whaterver.md" |]
