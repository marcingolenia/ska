module SimpleSkaWithOptions

open System
open System.IO
open Xunit
open FsUnit.Xunit

[<Fact>]
let ``Scaffolding of nodejs with tests, graphql results in copied source files, replaced variables and installed packages as a result of scripts from yaml config.`` () =
    // Arrange
    let skas = YamlDao.listBy "skas_apps"
    let nodeSka = skas |> List.find(fun ska -> ska.Name = "Node Backend Graphql")
    let toPath =
        Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString())
    let expectedPackageJsonNameLine = "\"name\": \"Test Project Name\","
    let expectedIndexPingLine = "app.get('/ping', (_, res) => res.send('pong'))"
    // Act
    SkaEngine.run nodeSka toPath
    // Assert
    let copiedIndexFile = Directory.GetFiles(Path.Combine(toPath, "src"), "index.ts")
    let packageJsonFile = Directory.GetFiles(toPath, "package.json")
    let copiedSkaOptionFiles =
        Directory.GetFiles(Path.Combine(toPath, "src", "graphql"), "*.*")
        |> Array.map(fun filePath -> filePath.Replace($"{toPath}{Path.DirectorySeparatorChar}", ""))
    copiedSkaOptionFiles |> should contain "src/graphql/graphql.ts"
    let indexContent = File.ReadAllText copiedIndexFile[0]
    let packageJsonContent = File.ReadAllText packageJsonFile[0]
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
    packageJsonContent.IndexOf expectedPackageJsonNameLine |> should greaterThan -1
    indexContent.IndexOf expectedIndexPingLine |> should greaterThan -1
    
[<Fact>]
let ``Scaffolding of nodejs with tests and not selected graphql option results in copied files which graphql specific content is removed.`` () =
    // Arrange
    let skas = YamlDao.listBy "skas_apps"
    let nodeSka = skas |> List.find(fun ska -> ska.Name = "Node Backend Graphql")
    let nodeSkaWithoutGraphql = { nodeSka with Options = [] }
    let toPath =
        Path.Combine(Directory.GetCurrentDirectory(), Guid.NewGuid().ToString())
    // Act
    SkaEngine.run nodeSkaWithoutGraphql toPath
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
