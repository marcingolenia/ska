open System.IO
open Spectre.Console
open System

let logo =
    """
[darkred]
 _______  _        _______ 
(  ____ \| \    /\(  ___  )
| (    \/|  \  / /| (   ) |
| (_____ |  (_/ / | (___) |
(_____  )|   _ (  |  ___  |
      ) ||  ( \ \ | (   ) |
/\____) ||  /  \ \| )   ( |
\_______)|_/    \/|/     \|                           
[/]
"""
let skasPath = Environment.GetCommandLineArgs()
               |> Array.tryItem(1)
               |> Option.defaultValue (Path.Combine(Directory.GetCurrentDirectory(), "skas"))
AnsiConsole.MarkupLine($"[red]Skas[/] loaded from: {skasPath}")
let skas = YamlDao.listBy skasPath
AnsiConsole.Markup("[red]Link[/] to project: [link=https://spectreconsole.net]GitHub Repo[/]")
AnsiConsole.MarkupLine(logo)
let whatToCreate = SelectionPrompt<string>()
whatToCreate.Title <- "What should I Create?"

skas
|> Views.mainOptionsNames
|> List.iter(fun ska -> whatToCreate.AddChoice ska |> ignore)

let selectedSkaName = AnsiConsole.Prompt whatToCreate
let selectedSka = (skas |> List.find(fun ska -> ska.Name = selectedSkaName))
let features = MultiSelectionPrompt<string>()
features.Required <- false

Views.optionsNames selectedSka
|> List.iter(fun option -> features.AddChoice(option) |> ignore)

let selectedFeatures = AnsiConsole.Prompt features

let ska =
    { selectedSka with
        Options =
            selectedSka.Options
            |> List.filter(fun ska -> selectedFeatures.Contains(ska.Name)) }

let pathPrompt =
    TextPrompt<string>("[grey][[Press enter for current path]][/] Please write relative target path")

pathPrompt.AllowEmpty <- true
let selectedPath = AnsiConsole.Prompt pathPrompt

let targetPath =
    match String.IsNullOrWhiteSpace selectedPath with
    | true -> AppDomain.CurrentDomain.BaseDirectory
    | false -> System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + selectedPath)

AnsiConsole
    .Status(Spinner = Spinner.Known.Material)
    .Start("Scaffolding...", (fun ctx -> SkaEngine.run ska targetPath))

AnsiConsole.Markup("[red]Done[/] 👍\n")
