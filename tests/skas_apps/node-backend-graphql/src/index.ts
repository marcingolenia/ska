import express from 'express'
import { createRouter } from './recommendation/router'
import { compose } from './recommendation/composition_root'
//+ska_graphql
import { schema, root } from './graphql/graphql'
import { graphqlHTTP } from 'express-graphql'
//ska_graphql

const app = express()
//+ska_graphql
app.use('/graphql', graphqlHTTP({
  schema: schema,
  rootValue: root,
  graphiql: true,
}));
//ska_graphql
const port = 3002

app.use(createRouter(compose()))
app.get('/ping', (_, res) => res.send('pong'))

if (process.env.NODE_ENV !== 'test') {
  app.listen(port, () => console.log(`Listening at http://localhost:${port}`))
}

export default app