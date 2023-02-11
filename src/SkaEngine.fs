module SkaEngine
    let run (ska: Domain.Ska) (targetPath: string) (features: string list) =
           FilesCopier.copyFiles ska.Path targetPath
           ska.Scripts |> List.iter(fun script -> ScriptRunner.run targetPath script |> ignore)