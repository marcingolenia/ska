module Views

open Domain

let mainOptionsNames (skas: Ska list) =
    skas |> List.map(fun skas -> skas.Name)
    
let optionsNames (ska: Ska) =
    ska.Options |> List.map(fun option -> option.Name)
    