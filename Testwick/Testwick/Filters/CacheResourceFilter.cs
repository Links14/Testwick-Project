using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace Testwick.Filters
{
    public class CacheResourceFilter(IMemoryCache cache) : IResourceFilter
    {
        private readonly IMemoryCache _cache = cache;
        private readonly int _cacheDurationInMinutes = 5;

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            throw new NotImplementedException();
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
