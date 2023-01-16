module ScriptRunner

open System.Diagnostics

let run (script: string) =
    use cmd = new Process()
    let cmdInfo = ProcessStartInfo()
    cmdInfo.FileName <- "bash"
    cmdInfo.RedirectStandardInput <- true
    cmdInfo.RedirectStandardOutput <- true
    cmdInfo.RedirectStandardError <- true
    cmdInfo.UseShellExecute <- false
    cmd.StartInfo <- cmdInfo
    cmd.Start() |> ignore
    cmd.StandardInput.WriteLine(script)
    let output = ResizeArray<string> []
    while cmd.StandardOutput.Peek() > -1 do
        output.Add(cmd.StandardOutput.ReadLine())
    output |> Seq.toList