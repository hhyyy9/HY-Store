using Basket.Core.Entities;
using Basket.Core.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly IDistributedCache _distributedCache;

    public BasketRepository(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }
    public async Task<ShoppingCart> GetBasket(string userName)
    {
        var basket = await _distributedCache.GetStringAsync(userName);
        if (string.IsNullOrEmpty(basket))
        {
            return null;
        }

        return JsonConvert.DeserializeObject<ShoppingCart>(basket);
    }

    public async Task<ShoppingCart> UpdateBasket(ShoppingCart shoppingCart)
    {
        await _distributedCache.SetStringAsync(shoppingCart.Username, JsonConvert.SerializeObject(shoppingCart));
        return await GetBasket(shoppingCart.Username);
    }

    public async Task DeleteBasket(string userName)
    {
        await _distributedCache.RemoveAsync(userName);
    }
}