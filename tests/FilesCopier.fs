module SourceFilesCopier

open Xunit

[<Fact>]
let ``All files of the ska directory and subdirectories should be copied`` () =
    // Arrange
    let path = "skas"
    // Act
    FilesCopier.copy ""
    ()