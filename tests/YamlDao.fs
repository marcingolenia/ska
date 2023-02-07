module Tests

open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Should assemble ska root paths`` () =
    // Arrange + Act
    let skas = YamlDao.listBy "skas"
    // Assert
    let rootPaths = skas |> List.map(fun ska -> ska.Path)
    rootPaths
    |> should
        equal
        [ "skas/go-backend/ska_Go_Backend.yaml"
          "skas/node-backend/ska_Node_Backend.yaml" ]

[<Fact>]
let ``Should parse filenames into natural name`` () =
    // Arrange + Act
    let skas = YamlDao.listBy "skas"
    // Assert
    skas |> Views.mainOptionsNames |> should equal [ "Go Backend"; "Node Backend" ]
    skas[0] |> Views.optionsNames |> should equal [ "GRPC" ]
    skas[1] |> Views.optionsNames |> should be Empty

[<Fact>]
let ``should read scripts`` () =
    // Arrange + Act
    let skas = YamlDao.listBy "skas"
    // Assert
    skas[0].Scripts
    |> should equal [ "echo I will create go backend for you"; "echo echo" ]
    skas[0].Options[0].Scripts |> should equal [ "echo linuxik"; "echo all" ]
