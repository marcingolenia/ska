module Domain

type Ska =
    { Name: string
      Path: string
      Scripts: string list
      Options: Ska list }
