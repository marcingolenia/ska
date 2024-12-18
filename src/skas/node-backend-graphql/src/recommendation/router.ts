import { Router } from 'express'
import { CompositionRoot } from './composition_root'

export const createRouter = (root: CompositionRoot) => {
  let router = Router()

  router.get('/recommendation', async (req, res) => {
    const from = new Date(req.query.from as string)
    const to = new Date(req.query.to as string)
    const recommendation = await root.recommendClothes(from, to)
    res.send(recommendation)
  })
  return router
}
