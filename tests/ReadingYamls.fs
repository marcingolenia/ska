module Tests

open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Should generate titles from all yamls`` () =
    // Arrange + Act
    let skas = FileLookup.find "skas"
    // Assert
    skas.Length |> should equal 2
    