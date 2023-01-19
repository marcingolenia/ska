module SourceFilesCopier

open Xunit
open FsUnit.Xunit


[<Fact>]
let ``All files of the ska directory and subdirectories should be copied`` () =
    // Arrange
    let skas = YamlDao.listBy "skas"
    // Act
    FilesCopier.copy skas[0]
    ()