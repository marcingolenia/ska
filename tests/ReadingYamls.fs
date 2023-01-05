module Tests

open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Should generate titles from all yamls`` () =
    // Arrange + Act
    let skas = FileLookup.findFileNames "skas" "yaml"
    // Assert
    printfn $"files: %A{skas}!"
    skas |> should equal [|"0_Go_Backend.yaml"; "GRPC.yaml"|]
    