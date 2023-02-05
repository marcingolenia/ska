import app from '../src/index'
import request from 'supertest'
import {findClothes} from '../src/recommendation/domain' 

it(`I want to receive 1 tshirt & 1 pair of socks and 1 panties per day recommendation`, async () => {
    // Arrange
    const expectedClothes = ['2 x tshirt', '2 x pair of socks', '2 x panties']
    // Act
    const response = await request(app).get('/recommendation?from=2022-10-03&to=2022-10-05')
    // Assert
    expectedClothes.forEach(clothes => 
        expect(response.body).toContain(clothes))
})

it(`I want to receive 1 jacket recommendation if any day in the date range has temperature below 11 C°`, async () => {
    // Arrange
    const expectedClothes = ['2 x tshirt', '2 x pair of socks', '2 x panties', '1 x jacket']
    // Act
    const actualClothes = findClothes([10, 12])
    // Assert
    expect(actualClothes).toStrictEqual(expectedClothes)
})