export const findClothes = (temperatures: number[]) => {
    const days = temperatures.length
    const clothes = [`${days} x tshirt`, `${days} x pair of socks`, `${days} x panties`]
    if(temperatures.some(celsius => celsius < 11))
        clothes.push('1 x jacket')
    return clothes
}