module ScriptRunner

open Xunit
open FsUnit.Xunit
open ScriptRunner

[<Fact>]
let ``ls script`` () =
    let result = run "ls"
    result.Length |> should greaterThan 2
    
[<Fact>]
let ``echo script`` () =
    let result = run "echo madafaka"
    result |> should equal ["madafaka"]