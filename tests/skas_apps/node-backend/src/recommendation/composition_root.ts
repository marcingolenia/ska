import { fetchTemperaturesMock } from '../mocks/weather_api'
import { findClothes } from './domain'

export type CompositionRoot = {
    recommendClothes: (from: Date, to: Date) => Promise<string[]>
  }

export let compose = () => {
    let dependencies: CompositionRoot = {
        recommendClothes: async (from: Date, to: Date) => {
            const temperatures = await fetchTemperaturesMock(from, to)
            return findClothes(temperatures)
        }
    }
    return dependencies
}