import express from 'express'
import { createRouter } from './recommendation/router'
import { compose } from './recommendation/composition_root'

const app = express()
const port = 3002

app.use(createRouter(compose()))
app.get('/ping', (_, res) => res.send('pong'))

if (process.env.NODE_ENV !== 'test') {
  app.listen(port, () => console.log(`Listening at http://localhost:${port}`))
}

export default app