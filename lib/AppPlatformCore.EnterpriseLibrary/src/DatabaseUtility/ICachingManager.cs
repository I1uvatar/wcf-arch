using Microsoft.Practices.EnterpriseLibrary.Caching;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    public interface ICachingManager
    {
        /// <summary>
        /// Add item in Cache
        /// </summary>
        /// <param name="itemKey">Indentifier for item </param>
        /// <param name="item">Item to store in cache</param>
        void Add(string itemKey, object item);

        /// <summary>
        /// Add item in Cache
        /// </summary>
        /// <param name="itemKey">Indentifier for item</param>
        /// <param name="item">Item to add in cache</param>
        /// <param name="priority">Sets priority for item</param>
        /// <param name="refreshAction">Determing refresh action</param>
        /// <param name="expirations">Expirations parametars</param>
        void Add(string itemKey, object item, CacheItemPriority priority, ICacheItemRefreshAction refreshAction,
                 params ICacheItemExpiration[] expirations);

        /// <summary>
        /// Retrieving item from cache
        /// </summary>
        /// <param name="itemKey">Indentifier for item to retrieve from cache</param>
        TDataType Retrieve<TDataType>(string itemKey);

        /// <summary>
        /// Deleting item from cache
        /// </summary>
        /// <param name="itemKey">Indentifier for item to delete from cache</param>
        void Delete(string itemKey);
        
        /// <summary>
        /// Removes all items from the cache
        /// </summary>
        void Flush();

        /// <summary>
        /// Returns true if key refers to item current stored in cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Contains(string key);
    }
}
