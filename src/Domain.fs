module Domain

type Ska =
    { Name: string
      Path: string
      Scripts: string list
      Variables: Map<string, string>
      Options: Ska list }
