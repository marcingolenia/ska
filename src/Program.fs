open Spectre.Console

let logo = """
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

AnsiConsole.Markup("[link=https://spectreconsole.net]GitHub Repo[/]");
AnsiConsole.MarkupLine(logo)

let whatToCreate = SelectionPrompt<string>()
whatToCreate.Title <- "What should I Create?"
whatToCreate.AddChoice("Go Backend") |> ignore
whatToCreate.AddChoice("NodeJS Backend") |> ignore

let toSkaPrompt = AnsiConsole.Prompt whatToCreate
let features = MultiSelectionPrompt<string>()
features.AddChoice("GRPC").Select() |> ignore
features.AddChoice("GraphQL").Select() |> ignore
features.AddChoice("Sqlx + Goose migrations").Select() |> ignore
features.AddChoice("HTTP API").Select() |> ignore
features.AddChoice("Docker Image + Cloud Run Terraform").Select() |> ignore
features.AddChoice("Github action").Select() |> ignore

let featuresPrompt = AnsiConsole.Prompt features


AnsiConsole.Markup("[red]Done[/] 👍\n");