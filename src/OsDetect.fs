module OsDetect

open System.Runtime.InteropServices

type Platform =
    | Linux
    | MacOs
    | Windows

let os =
    match (RuntimeInformation.IsOSPlatform(OSPlatform.Windows), RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) with
    | true, _ -> Windows
    | _, true -> MacOs
    | _ -> Linux
