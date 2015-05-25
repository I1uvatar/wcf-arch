using System.Collections.Generic;
using System.Data;
using AppPlatform.Core.EnterpriseLibrary.DatabaseUtility;

namespace AppPlatform.Core.EnterpriseLibrary.ResourceAccess
{
    /// <summary>
    /// Implements and declare methods for mapping between <typeparamref name="TEntity"/> and ADO.NET objects
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TEntityKey">The type of the entity key.</typeparam>
    public abstract class EntityMapperExtended<TEntity, TEntityKey> : IEntityMapperExtended<TEntity, TEntityKey> 
        where TEntity : class, new()
    {
        #region Implementation of IEntityMapper<TEntity>

        /// <summary>
        /// Creates list of objects from pending data reader
        /// </summary>
        /// <param name="reader">Pending reader</param>
        /// <returns></returns>
        public virtual List<TEntity> CreateListFromReader(IDataReader reader)
        {
            var list = new List<TEntity>();
            while (reader.Read())
            {
                var entity = new TEntity();
                this.FillFromReader(entity, reader);
                list.Add(entity);
            }

            return list;
        }

        /// <summary>
        /// Creates single data object from pending reader
        /// </summary>
        /// <param name="reader">Pending reader</param>
        /// <returns></returns>
        public virtual TEntity CreateSingleFromReader(IDataReader reader)
        {
            if (reader.Read())
            {
                var entity = new TEntity();
                this.FillFromReader(entity, reader);

                return entity;
            }
            return null;
        }

        #endregion

        #region Implementation of IEntityReadMapper<TEntity,TEntityKey>

        /// <summary>
        /// Fill get command
        /// </summary>
        /// <param name="getCommand">command to fill</param>
        /// <param name="key">Entity key identifier</param>
        public abstract void FillGetCommand(IDbCommand getCommand, TEntityKey key);

        #endregion

        #region Implementation of IEntityMapperExtended<TEntity,TEntityKey>

        /// <summary>
        /// Fill insert command with parameters
        /// </summary>
        /// <param name="entity">Data object</param>
        /// <param name="command">Command to fill</param>
        public abstract void FillInsertCommand(TEntity entity, IDbCommand command);

        /// <summary>
        ///  Performs additional proccessing after insert is completed
        /// </summary>
        /// <param name="entity">Data object</param>
        /// <param name="command">Command to fill</param>
        public abstract void ProcessPostInsert(TEntity entity, IDbCommand command);

        /// <summary>
        /// Fill update command with parameters
        /// </summary>
        /// <param name="entity">Data object</param>
        /// <param name="command">Command to fill</param>
        public abstract void FillUpdateCommand(TEntity entity, IDbCommand command);

        /// <summary>
        /// Performs additional proccessing after update is completed
        /// </summary>
        /// <param name="entity">Data which are updated</param>
        /// <param name="command">Command</param>
        public abstract void ProcessPostUpdate(TEntity entity, IDbCommand command);

        /// <summary>
        /// Fill delete command with parameters
        /// </summary>
        /// <param name="entity">Data object</param>
        /// <param name="command">Command to fill</param>
        public abstract void FillDeleteCommand(TEntity entity, IDbCommand command);

        #endregion

        #region Abstract methods

        protected abstract void FillFromReader(TEntity entity, IDataRead reader);

        #endregion

        #region Private methods

        private void FillFromReader(TEntity entity, IDataReader pendingReader)
        {
            this.FillFromReader(entity, new DataReaderWrapper(pendingReader));
        }

        #endregion
    }
}
