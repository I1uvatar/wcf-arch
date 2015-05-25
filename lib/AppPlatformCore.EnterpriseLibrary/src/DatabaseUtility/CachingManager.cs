using Microsoft.Practices.EnterpriseLibrary.Caching;

namespace AppPlatform.Core.EnterpriseLibrary.DatabaseUtility
{
    public class CachingManager : ICachingManager
    {
        private readonly ICacheManager cacheManager;

        /// <summary>
        /// Constructor for CachingManager class 
        /// </summary>
        /// <param name="cacheManager">Instance of ICacheManager</param>
        public CachingManager(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        /// <summary>
        /// Constructor for CachingManager class : Generate default CacheManager from configuration
        /// </summary>
        public CachingManager()
        {
            this.cacheManager = CacheFactory.GetCacheManager();
        }

        /// <summary>
        /// Constructor for CachingManager class : Generate CacheManager defined in Configuration with cacheManagerName
        /// </summary>
        public CachingManager(string cacheManagerName)
        {
            this.cacheManager = CacheFactory.GetCacheManager(cacheManagerName);
        }

        #region ICachingManager Members

        /// <summary>
        /// Add item in Cache
        /// </summary>
        /// <param name="itemKey">Indentifier for item </param>
        /// <param name="item">Item to store in cache</param>
        public void Add(string itemKey, object item)
        {
            this.cacheManager.Add(itemKey, item);
        }


        /// <summary>
        /// Add item in Cache
        /// </summary>
        /// <param name="itemKey">Indentifier for item</param>
        /// <param name="item">Item to add in cache</param>
        /// <param name="priority">Sets priority for item</param>
        /// <param name="refreshAction">Determing refresh action</param>
        /// <param name="expirations">Expirations parametars</param>
        public void Add(string itemKey, object item,CacheItemPriority priority,ICacheItemRefreshAction refreshAction,params ICacheItemExpiration[] expirations)
        {
            this.cacheManager.Add(itemKey, item, priority, refreshAction, expirations);
        }

        /// <summary>
        /// Retrieving item from cache
        /// </summary>
        /// <param name="itemKey">Indentifier for item to retrieve from cache</param>
        public TDataType Retrieve<TDataType>(string itemKey)
        {
            return (TDataType)this.cacheManager.GetData(itemKey);
        }

        /// <summary>
        /// Deleting item from cache
        /// </summary>
        /// <param name="itemKey">Indentifier for item to delete from cache</param>
        public void Delete(string itemKey)
        {
            this.cacheManager.Remove(itemKey);
        }

        /// <summary>
        /// Removes all items from the cache
        /// </summary>
        public void Flush()
        {
            this.cacheManager.Flush();
        }

        /// <summary>
        /// Returns true if key refers to item current stored in cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return this.cacheManager.Contains(key);
        }

        #endregion
    }
}
