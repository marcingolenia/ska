import app from '../src/index'
import request from 'supertest'

it(`When GET /ping then return pong`, async () => {
  const response = await request(app).get(`/ping`)
  expect(response.text).toBe('pong')
})