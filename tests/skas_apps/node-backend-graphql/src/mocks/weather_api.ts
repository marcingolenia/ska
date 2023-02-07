export const fetchTemperaturesMock = (from: Date, to: Date) => {
    const days = (to.getTime() - from.getTime()) / (1000 * 3600 * 24)
    const temperatures = [...new Array(days)].map(_ => Math.round(Math.random() * 30))
    return Promise.resolve(temperatures)
  }