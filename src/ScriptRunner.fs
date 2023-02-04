module ScriptRunner

open System.Collections.Concurrent
open System.Diagnostics
open System.Threading.Tasks

let run path script  =
    let cts = TaskCompletionSource()
    use cmd = new Process()
    let cmdInfo = ProcessStartInfo()
    cmdInfo.FileName <- "bash"
    cmdInfo.RedirectStandardInput <- true
    cmdInfo.RedirectStandardOutput <- true
    cmdInfo.RedirectStandardError <- true
    cmdInfo.UseShellExecute <- false
    cmdInfo.WorkingDirectory <- path
    cmd.StartInfo <- cmdInfo
    let output = new BlockingCollection<string>()
    let append = fun (args: DataReceivedEventArgs) ->
        output.Add args.Data
        if args.Data = "_skafinished_" then cts.SetResult()
    cmd.ErrorDataReceived.Add append
    cmd.OutputDataReceived.Add append
    cmd.Start() |> ignore
    cmd.BeginOutputReadLine();
    cmd.BeginErrorReadLine();
    cmd.StandardInput.WriteLine($"{script} ; echo _skafinished_")
    cts.Task.Wait()
    output |> Seq.toList |> List.filter(fun output -> output <> "_skafinished_")