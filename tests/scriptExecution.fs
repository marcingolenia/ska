module LearningTests

open System.Diagnostics
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``echo script`` () =
    use cmd = new Process()
    let cmdInfo = ProcessStartInfo()
    cmdInfo.FileName <- "bash"
    cmdInfo.RedirectStandardInput <- true
    cmdInfo.RedirectStandardOutput <- true
    cmdInfo.RedirectStandardError <- true
    cmdInfo.UseShellExecute <- false
    cmd.StartInfo <- cmdInfo
    cmd.Start() |> ignore
    cmd.StandardInput.WriteLine("echo hello")
    let output = ResizeArray<string> []
    while cmd.StandardOutput.Peek() > -1 do
        output.Add(cmd.StandardOutput.ReadLine())
    output.Count |> should equal 1
    output[0] |> should equal "hello"

[<Fact>]
let ``ls script`` () =
    use cmd = new Process()
    let cmdInfo = ProcessStartInfo()
    cmdInfo.FileName <- "bash"
    cmdInfo.RedirectStandardInput <- true
    cmdInfo.RedirectStandardOutput <- true
    cmdInfo.RedirectStandardError <- true
    cmdInfo.UseShellExecute <- false
    cmd.StartInfo <- cmdInfo
    cmd.Start() |> ignore
    cmd.StandardInput.WriteLine("ls")
    let output = ResizeArray<string> []
    while cmd.StandardOutput.Peek() > -1 do
        output.Add(cmd.StandardOutput.ReadLine())
    output.Count |> should greaterThan 2
