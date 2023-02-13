module SimpleSkaWithOptions

open System
open System.IO
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Working nodejs with tests with graphql option is possible`` () =
    // Arrange
    let skas = YamlDao.listBy "skas_apps"
    let nodeSka = skas |> List.find(fun ska -> ska.Name = "Node Backend Graphql")
    let toPath =
        Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString())
    // Act
    SkaEngine.run nodeSka toPath []
    // Assert
    let copiedIndexFile = Directory.GetFiles(Path.Combine(toPath, "src"), "index.ts")
    let copiedSkaOptionFiles =
        Directory.GetFiles(Path.Combine(toPath, "src", "graphql"), "*.*")
        |> Array.map(fun filePath -> filePath.Replace($"{toPath}{Path.DirectorySeparatorChar}", ""))
    copiedSkaOptionFiles |> should contain "src/graphql/graphql.ts"
    let indexContent = File.ReadAllText(copiedIndexFile[0])
    let graphQlSpecificBlock1 =
        """
import { schema, root } from './graphql/graphql'
import { graphqlHTTP } from 'express-graphql'
"""
    let graphQlSpecificBlock2 =
        """
app.use('/graphql', graphqlHTTP({
  schema: schema,
  rootValue: root,
  graphiql: true,
}));
"""
    indexContent.IndexOf graphQlSpecificBlock1 |> should greaterThan -1
    indexContent.IndexOf graphQlSpecificBlock2 |> should greaterThan -1
    indexContent.IndexOf "//+ska_graphql" |> should equal -1
    indexContent.IndexOf "//ska_graphql" |> should equal -1

[<Fact>]
let ``Working nodejs with tests with graphql option not selected is possible`` () =
    // Arrange
    let skas = YamlDao.listBy "skas_apps"
    let nodeSka = skas |> List.find(fun ska -> ska.Name = "Node Backend Graphql")
    let nodeSkaWithoutGraphql = { nodeSka with Options = [] }
    let toPath =
        Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString())
    // Act
    SkaEngine.run nodeSkaWithoutGraphql toPath []
    // Assert
    let copiedIndexFile = Directory.GetFiles(Path.Combine(toPath, "src"), "index.ts")
    let indexContent = File.ReadAllText(copiedIndexFile[0])
    let graphQlSpecificBlock1 =
        """
import { schema, root } from './graphql/graphql'
import { graphqlHTTP } from 'express-graphql'
"""
    let graphQlSpecificBlock2 =
        """
app.use('/graphql', graphqlHTTP({
  schema: schema,
  rootValue: root,
  graphiql: true,
}));
"""
    let srcDirectories =
        Path.Combine(toPath, "src")
        |> Directory.GetDirectories
        |> Array.map(fun filePath ->
            filePath.Replace($"{toPath}{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}", ""))
    srcDirectories |> should not' (contain "graphql")
    indexContent.IndexOf graphQlSpecificBlock1 |> should equal -1
    indexContent.IndexOf graphQlSpecificBlock2 |> should equal -1
    indexContent.IndexOf "//+ska_graphql" |> should equal -1
    indexContent.IndexOf "//ska_graphql" |> should equal -1
