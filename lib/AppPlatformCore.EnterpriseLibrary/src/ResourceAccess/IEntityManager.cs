namespace AppPlatform.Core.EnterpriseLibrary.ResourceAccess
{
    /// <summary>
    /// Defines methods for basic data manipulation functions
    /// </summary>
    /// <typeparam name="Entity">Type of the data object</typeparam>
    /// <typeparam name="EntityKey">Type of the data object key</typeparam>
    public interface IEntityManager<Entity, EntityKey> : IEntityReadManager<Entity, EntityKey>
    {
        /// <summary>
        /// Insert new object to the database
        /// </summary>
        /// <param name="entity">Object to insert</param>
        void Insert(Entity entity);

        /// <summary>
        /// Update existing object in database
        /// </summary>
        /// <param name="entityNewValue">Object containing new data</param>
        void Update(Entity entityNewValue);

        /// <summary>
        /// Delete existing object from database
        /// </summary>
        /// <param name="entityNewValue">Object to delete</param>
        void Delete(Entity entityNewValue);
    }

    /// <summary>
    /// Defines methods for database read functions
    /// </summary>
    /// <typeparam name="Entity">Type of the data object</typeparam>
    /// <typeparam name="EntityKey">Type of the data object key</typeparam>
    public interface IEntityReadManager<Entity, EntityKey>
    {
        /// <summary>
        /// Retrieves object from databse usind object unique key
        /// </summary>
        /// <param name="key">Object unique key</param>
        /// <returns>Object from database</returns>
        Entity GetById(EntityKey key);

        /// <summary>
        /// Retrieves object from databse usind object unique key
        /// </summary>
        /// <param name="key">Object unique key</param>
        /// <param name="useSqlDataEncription"></param>
        /// <returns>Object from database</returns>
        Entity GetById(EntityKey key, bool useSqlDataEncription);

    }
}
