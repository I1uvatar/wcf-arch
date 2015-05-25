using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using AppPlatform.Core.EnterpriseLibrary.DatabaseUtility;
using AppPlatform.Core.EnterpriseLibrary.Unity;
using System.Runtime.Remoting.Messaging;

namespace AppPlatform.Core.EnterpriseLibrary.ResourceAccess
{
    /// <summary>
    /// Base class implementing basic read data access methods
    /// </summary>
    /// <typeparam name="Entity">Type of the data object</typeparam>
    /// <typeparam name="EntityMapper">Type of the mapper object used to prepare data commands</typeparam>
    /// <typeparam name="EntityKey">Type of the key filed for the data object</typeparam>
    public abstract class EntityReadManagerBase<Entity, EntityMapper, EntityKey> : IEntityReadManager<Entity, EntityKey>
        where EntityMapper : IEntityReadMapper<Entity, EntityKey>
        where Entity : class
    {
        private readonly IKeyAccessStatement keyAccessStatement;
        protected readonly IDatabase database;
        protected readonly EntityMapper entityMapper;

        protected EntityReadManagerBase(EntityMapper mapper, string connectionStringKey)
            : this(
                Database.Create(connectionStringKey),
                mapper,
                null)
        { }

        protected EntityReadManagerBase(EntityMapper mapper, string connectionStringKey, IKeyAccessStatement keyAccessStatement)
            : this(
                Database.Create(connectionStringKey),
                mapper,
                keyAccessStatement)
        { }

        protected EntityReadManagerBase(IDatabase database, EntityMapper entityMapper, IKeyAccessStatement keyAccessStatement)
        {
            this.database = database;
            this.entityMapper = entityMapper;
            this.keyAccessStatement = keyAccessStatement;
        }

        protected virtual IDatabaseActivityLogger GetLogger()
        {
            try
            {
                var getLogger = CallContext.GetData(this.LoggerKey) as Func<IDatabaseActivityLogger>;
                if (getLogger != null)
                {
                    return getLogger();
                }

                return IocProvider.Container.Resolve<IDatabaseActivityLogger>();
            }
            catch
            {
                return null;
            }
        }

        public IDisposable OverrideLogger(Func<IDatabaseActivityLogger> getter)
        {
            CallContext.SetData(this.LoggerKey, getter);
            return new CleanupLogger(this.LoggerKey);
        }

        private class CleanupLogger : IDisposable
        {
            private readonly string key;

            public CleanupLogger(string key)
            {
                this.key = key;
            }

            public void Dispose()
            {
                CallContext.FreeNamedDataSlot(this.key);
            }
        }

        private string LoggerKey
        {
            get { return this.GetType().FullName + ": Logger"; }
        }


        protected abstract string GetCommandName { get; }

        /// <summary>
        /// Action to be executed after entity is creted from reader
        /// </summary>
        protected virtual void ProcessPostItemRead(Entity entity) { }

        /// <summary>
        /// Retrieves object from databse usind object unique key
        /// </summary>
        /// <param name="key">Object unique key</param>
        /// <returns>Object from database</returns>
        public Entity GetById(EntityKey key)
        {
            return this.GetById(key, false);
        }

        /// <summary>
        /// Retrieves object from databse usind object unique key
        /// </summary>
        /// <param name="key">Object unique key</param>
        /// <param name="useSqlDataEncription"></param>
        /// <returns>Object from database</returns>
        public Entity GetById(EntityKey key, bool useSqlDataEncription)
        {
            return this.GetByStoredProcedure(
                this.GetCommandName,
                cmd => this.entityMapper.FillGetCommand(cmd, key),
                useSqlDataEncription
                );
        }

        /// <summary>
        /// Retrieves object from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command used to fill command parameter</param>
        /// <returns>Object from database</returns>
        protected Entity GetByStoredProcedure(string spName, FillGetCommand setupParameters)
        {
            return this.GetByStoredProcedure(spName, setupParameters, false);
        }

        /// <summary>
        /// Retrieves object from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command used to fill command parameter</param>
        /// <param name="useSqlDataEncription"></param>
        /// <returns>Object from database</returns>
        protected Entity GetByStoredProcedure(string spName, FillGetCommand setupParameters, bool useSqlDataEncription)
        {
            return this.GetByStoredProcedure(spName, setupParameters, (entity, reader) => { }, useSqlDataEncription);
        }

        /// <summary>
        /// Retrieves object from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command used to fill command parameter</param>
        /// <param name="readerHandler">Proccess additional task to opened datareader</param>
        /// <returns>Object from database</returns>
        protected Entity GetByStoredProcedure(string spName, FillGetCommand setupParameters, ProccessReader<Entity> readerHandler)
        {
            return this.GetByStoredProcedure(spName, setupParameters, readerHandler, false);
        }

        /// <summary>
        /// Retrieves object from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command used to fill command parameter</param>
        /// <param name="readerHandler">Proccess additional task to opened datareader</param>
        /// <returns>Object from database</returns>
        protected Entity GetByStoredProcedureWithSearchLimitation(string spName, FillGetCommand setupParameters, ProccessReader<Entity> readerHandler)
        {
            return this.GetByStoredProcedureWithSearchLimitation(spName, setupParameters, readerHandler, false);
        }

        /// <summary>
        /// Gets the by stored procedure.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="setupParameters">The setup parameters.</param>
        /// <param name="readerHandler">The reader handler.</param>
        /// <param name="useSqlDataEncryption">if set to <c>true</c> [use SQL data encryption].</param>
        /// <returns></returns>
        protected Entity GetByStoredProcedure(
            string spName,
            FillGetCommand setupParameters,
            ProccessReader<Entity> readerHandler,
            bool useSqlDataEncryption
            )
        {
            return this.GetByStoredProcedure(
                spName,
                setupParameters,
                this.entityMapper.CreateSingleFromReader,
                readerHandler,
                this.ProcessPostItemRead,
                useSqlDataEncryption
                );
        }

        /// <summary>
        /// Gets the by stored procedure.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="setupParameters">The setup parameters.</param>
        /// <param name="readerHandler">The reader handler.</param>
        /// <param name="useSqlDataEncryption">if set to <c>true</c> [use SQL data encryption].</param>
        /// <returns></returns>
        protected Entity GetByStoredProcedureWithSearchLimitation(
            string spName,
            FillGetCommand setupParameters,
            ProccessReader<Entity> readerHandler,
            bool useSqlDataEncryption
            )
        {
            return this.GetByStoredProcedureWithSearchLimitation(
                spName,
                setupParameters,
                this.entityMapper.CreateSingleFromReader,
                readerHandler,
                this.ProcessPostItemRead,
                useSqlDataEncryption
                );
        }

        /// <summary>
        /// Retrieves object from database by specifying get procedure
        /// </summary>
        /// <typeparam name="EntityType">The type of the ntity type.</typeparam>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command used to fill command parameter</param>
        /// <param name="map">The map.</param>
        /// <param name="readerHandler">Proccess additional task to opened datareader</param>
        /// <param name="processPostItemRead">The process post item read.</param>
        /// <param name="useSqlDataEncryption">if set to <c>true</c> [use SQL data encryption].</param>
        /// <returns>Object from database</returns>
        protected EntityType GetByStoredProcedure<EntityType>(
            string spName,
            FillGetCommand setupParameters,
            Func<IDataReader, EntityType> map,
            ProccessReader<EntityType> readerHandler,
            Action<EntityType> processPostItemRead,
            bool useSqlDataEncryption
            )
        {
            using (DbCommand getCommand = this.database.GetStoredProcCommand(spName))
            {
                setupParameters(getCommand);

                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(getCommand, logger, useSqlDataEncryption);

                EntityType entity;

                using (IDataReader entityReader = this.database.ExecuteReader(getCommand, logger))
                {
                    entity = map(entityReader);

                    if (readerHandler != null)
                    {
                        readerHandler(entity, entityReader);
                    }
                }

                if (entity != null && processPostItemRead != null)
                {
                    processPostItemRead(entity);
                }

                return entity;
            }
        }

        /// <summary>
        /// Retrieves object from database by specifying get procedure
        /// </summary>
        /// <typeparam name="EntityType">The type of the ntity type.</typeparam>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command used to fill command parameter</param>
        /// <param name="map">The map.</param>
        /// <param name="readerHandler">Proccess additional task to opened datareader</param>
        /// <param name="processPostItemRead">The process post item read.</param>
        /// <param name="useSqlDataEncryption">if set to <c>true</c> [use SQL data encryption].</param>
        /// <returns>Object from database</returns>
        protected EntityType GetByStoredProcedureWithSearchLimitation<EntityType>(
            string spName,
            FillGetCommand setupParameters,
            Func<IDataReader, EntityType> map,
            ProccessReader<EntityType> readerHandler,
            Action<EntityType> processPostItemRead,
            bool useSqlDataEncryption
            )
        {
            using (DbCommand getCommand = this.database.GetStoredProcCommand(spName))
            {
                setupParameters(getCommand);
                long limit;
                this.SetSearchLimit(getCommand, out limit);

                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(getCommand, logger, useSqlDataEncryption);

                EntityType entity;

                using (IDataReader entityReader = this.database.ExecuteReader(getCommand, logger))
                {
                    entity = map(entityReader);

                    if (readerHandler != null)
                    {
                        readerHandler(entity, entityReader);
                    }
                }

                if (entity != null && processPostItemRead != null)
                {
                    processPostItemRead(entity);
                }

                return entity;
            }
        }

        /// <summary>
        /// Retrieves list of objects from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <returns>Object list from database</returns>
        protected List<Entity> GetListByStoredProcedure(string spName, FillGetCommand setupParameters)
        {
            return this.GetListByStoredProcedure(spName, setupParameters, false, this.ProcessPostItemRead);
        }

        /// <summary>
        /// Retrieves list of objects from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <returns>Object list from database</returns>
        protected List<Entity> GetListByStoredProcedure(string spName, FillGetCommand setupParameters, int searchLevel, Enum displayHistoryNodeType)
        {
            return this.GetListByStoredProcedure(spName, setupParameters, false, this.ProcessPostItemRead, searchLevel, displayHistoryNodeType);
        }

        /// <summary>
        /// Gets the list by stored procedure.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="setupParameters">The setup parameters.</param>
        /// <param name="readerHandler">The reader handler.</param>
        /// <returns></returns>
        protected List<Entity> GetListByStoredProcedure(string spName, FillGetCommand setupParameters, ProccessReader<List<Entity>> readerHandler)
        {
            return this.GetListByStoredProcedure(spName, setupParameters, false, this.ProcessPostItemRead, readerHandler);
        }

        /// <summary>
        /// Retrieves list of objects from database by specifying get procedure, returning result (message)
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <returns>Object list from database</returns>
        protected ListQueryResult<Entity> GetListByStoredProcedureWithSearchLimitation(string spName, FillGetCommand setupParameters)
        {
            return this.GetListByStoredProcedureWithSearchLimitation(spName, setupParameters, false, this.ProcessPostItemRead);
        }

        /// <summary>
        /// Retrieves list of objects from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <param name="useSqlDataEncription">if set to <c>true</c> [use SQL data encription].</param>
        /// <returns>Object list from database</returns>
        protected List<Entity> GetListByStoredProcedure(string spName, FillGetCommand setupParameters, bool useSqlDataEncription)
        {
            return this.GetListByStoredProcedure(spName, setupParameters, useSqlDataEncription, this.ProcessPostItemRead);
        }

        /// <summary>
        /// Gets the list by stored procedure.
        /// </summary>
        /// <typeparam name="ListItem">The type of the ist item.</typeparam>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="setupParameters">The setup parameters.</param>
        /// <param name="map">The map.</param>
        /// <param name="useSqlDataEncription">if set to <c>true</c> [use SQL data encription].</param>
        /// <returns></returns>
        protected List<ListItem> GetListByStoredProcedure<ListItem>(
            string spName,
            FillGetCommand setupParameters,
            Func<IDataReader, List<ListItem>> map,
            bool useSqlDataEncription
            )
        {
            return this.GetListByStoredProcedure(
                spName,
                setupParameters,
                map,
                useSqlDataEncription,
                null,
                null
                );
        }

        /// <summary>
        /// Retrieves list of objects from database by specifying get procedure
        /// </summary>
        /// <typeparam name="ListItem">The type of the ist item.</typeparam>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <param name="map">The map.</param>
        /// <param name="useSqlDataEncription">if set to <c>true</c> [use SQL data encription].</param>
        /// <param name="processPostItemRead">Execute after item is created.</param>
        /// <param name="readerHandler">The reader handler.</param>
        /// <returns>Object list from database</returns>
        protected List<ListItem> GetListByStoredProcedure<ListItem>(
            string spName,
            FillGetCommand setupParameters,
            Func<IDataReader, List<ListItem>> map,
            bool useSqlDataEncription,
            Action<ListItem> processPostItemRead,
            ProccessReader<List<ListItem>> readerHandler)
        {
            return this.GetListByStoredProcedureWithSearchLimitation(spName, setupParameters, map, useSqlDataEncription, processPostItemRead, readerHandler).Result;
        }

        /// <summary>
        /// Retrieves list of objects from database by specifying get procedure
        /// </summary>
        /// <typeparam name="ListItem">The type of the ist item.</typeparam>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <param name="map">The map.</param>
        /// <param name="useSqlDataEncription">if set to <c>true</c> [use SQL data encription].</param>
        /// <param name="processPostItemRead">Execute after item is created.</param>
        /// <param name="readerHandler">The reader handler.</param>
        /// <param name="searchLevel"></param>
        /// <param name="displayHistoryNodeType"></param>
        /// <returns>Object list from database</returns>
        protected List<ListItem> GetListByStoredProcedure<ListItem>(
            string spName,
            FillGetCommand setupParameters,
            Func<IDataReader, int, Enum, List<ListItem>> map,
            bool useSqlDataEncription,
            Action<ListItem> processPostItemRead,
            ProccessReader<List<ListItem>> readerHandler,
            int searchLevel,
            Enum displayHistoryNodeType)
        {
            return this.GetListByStoredProcedureWithSearchLimitation(spName, setupParameters, map, useSqlDataEncription, processPostItemRead, readerHandler, searchLevel, displayHistoryNodeType).Result;
        }

        /// <summary>
        /// Retrieves list of objects amd search limit from database by specifying get procedure
        /// </summary>
        /// <typeparam name="ListItem">The type of the list item.</typeparam>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <param name="map">The map.</param>
        /// <param name="useSqlDataEncription">if set to <c>true</c> [use SQL data encription].</param>
        /// <param name="processPostItemRead">Execute after item is created.</param>
        /// <param name="readerHandler">The reader handler.</param>
        /// <returns>Object list from database</returns>
        protected ListQueryResult<ListItem> GetListByStoredProcedureWithSearchLimitation<ListItem>(
            string spName,
            FillGetCommand setupParameters,
            Func<IDataReader, List<ListItem>> map,
            bool useSqlDataEncription,
            Action<ListItem> processPostItemRead,
            ProccessReader<List<ListItem>> readerHandler)
        {
            using (DbCommand getCommand = this.database.GetStoredProcCommand(spName))
            {
                setupParameters(getCommand);
                long limit;
                this.SetSearchLimit(getCommand, out limit);

                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(getCommand, logger, useSqlDataEncription);

                List<ListItem> items;

                using (IDataReader entityReader = this.database.ExecuteReader(getCommand, logger))
                {
                    items = map(entityReader);

                    if (readerHandler != null)
                    {
                        readerHandler(items, entityReader);
                    }
                }

                if (processPostItemRead != null && items != null)
                {
                    items.ForEach(processPostItemRead);
                }

                return new ListQueryResult<ListItem>(items, limit);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ListItem"></typeparam>
        /// <param name="spName"></param>
        /// <param name="setupParameters"></param>
        /// <param name="map"></param>
        /// <param name="useSqlDataEncription"></param>
        /// <param name="processPostItemRead"></param>
        /// <param name="readerHandler"></param>
        /// <param name="searchLevel"></param>
        /// <param name="displayHistoryNodeType"></param>
        /// <returns></returns>
        protected ListQueryResult<ListItem> GetListByStoredProcedureWithSearchLimitation<ListItem>(
            string spName,
            FillGetCommand setupParameters,
            Func<IDataReader, int, Enum, List<ListItem>> map,
            bool useSqlDataEncription,
            Action<ListItem> processPostItemRead,
            ProccessReader<List<ListItem>> readerHandler,
            int searchLevel,
            Enum displayHistoryNodeType)
        {
            using (DbCommand getCommand = this.database.GetStoredProcCommand(spName))
            {
                setupParameters(getCommand);
                long limit;
                this.SetSearchLimit(getCommand, out limit);

                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(getCommand, logger, useSqlDataEncription);

                List<ListItem> items;

                using (IDataReader entityReader = this.database.ExecuteReader(getCommand, logger))
                {
                    items = map(entityReader, searchLevel, displayHistoryNodeType);

                    if (readerHandler != null)
                    {
                        readerHandler(items, entityReader);
                    }
                }

                if (processPostItemRead != null && items != null)
                {
                    items.ForEach(processPostItemRead);
                }

                return new ListQueryResult<ListItem>(items, limit);
            }
        }

        protected ListQueryResult<Entity> GetListByStoredProcedureWithSearchLimitation(string spName, FillGetCommand setupParameters, bool useSqlDataEncription, Action<Entity> processPostItemRead)
        {
            return this.GetListByStoredProcedureWithSearchLimitation(spName, setupParameters, this.entityMapper.CreateListFromReader, useSqlDataEncription, processPostItemRead, null);
        }

        protected List<Entity> GetListByStoredProcedure(string spName, FillGetCommand setupParameters, bool useSqlDataEncription, Action<Entity> processPostItemRead)
        {
            return this.GetListByStoredProcedure(spName, setupParameters, this.entityMapper.CreateListFromReader, useSqlDataEncription, processPostItemRead, null);
        }

        protected List<Entity> GetListByStoredProcedure(string spName, FillGetCommand setupParameters, bool useSqlDataEncription, Action<Entity> processPostItemRead, int searchLevel, Enum displayHistoryNodeType)
        {
            return this.GetListByStoredProcedure(spName, setupParameters, (this.entityMapper as IEntityReadLevelMapper<Entity, EntityKey>).CreateListFromReader, useSqlDataEncription, processPostItemRead, null, searchLevel, displayHistoryNodeType);
        }

        protected List<Entity> GetListByStoredProcedure(string spName, FillGetCommand setupParameters, bool useSqlDataEncription, Action<Entity> processPostItemRead, ProccessReader<List<Entity>> readerHandler)
        {
            return this.GetListByStoredProcedure(spName, setupParameters, this.entityMapper.CreateListFromReader, useSqlDataEncription, processPostItemRead, readerHandler);
        }

        /// <summary>
        /// Retrieves single data vlaue from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <returns>Procedure result</returns>
        protected T GetSingleValueByStoredProcedure<T>(string spName, FillGetCommand setupParameters)
        {
            return this.GetSingleValueByStoredProcedure<T>(spName, setupParameters, false);
        }

        /// <summary>
        /// Retrieves single data vlaue from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <param name="useSqlDataEncription"></param>
        /// <returns>Procedure result</returns>
        protected T GetSingleValueByStoredProcedure<T>(string spName, FillGetCommand setupParameters, bool useSqlDataEncription)
        {
            using (DbCommand getCommand = this.database.GetStoredProcCommand(spName))
            {
                setupParameters(getCommand);

                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(getCommand, logger, useSqlDataEncription);

                var scalar = this.database.ExecuteScalar(getCommand, logger);
                return (T)scalar;
            }
        }

        /// <summary>
        /// Retrieves single data vlaue from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <param name="useSqlDataEncription"></param>
        /// <param name="timeout"></param>
        /// <returns>Procedure result</returns>
        protected T GetSingleValueByStoredProcedureWithTimeout<T>(string spName, FillGetCommand setupParameters, bool useSqlDataEncription, int timeout)
        {
            using (DbCommand getCommand = this.database.GetStoredProcCommand(spName))
            {
                getCommand.CommandTimeout = timeout;
                setupParameters(getCommand);

                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(getCommand, logger, useSqlDataEncription);

                var scalar = this.database.ExecuteScalar(getCommand, logger);
                return (T)scalar;
            }
        }

        /// <summary>
        /// Retrieves single nullable data value from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <returns>Procedure result</returns>
        protected T? GetNullableSingleValueByStoredProcedure<T>(string spName, FillGetCommand setupParameters) where T : struct
        {
            return this.GetNullableSingleValueByStoredProcedure<T>(spName, setupParameters, false);
        }


        /// <summary>
        /// Retrieves single nullable data vlaue from database by specifying get procedure
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <param name="useSqlDataEncription"></param>
        /// <returns>Procedure result</returns>
        protected T? GetNullableSingleValueByStoredProcedure<T>(string spName, FillGetCommand setupParameters, bool useSqlDataEncription) where T : struct
        {
            using (DbCommand getCommand = this.database.GetStoredProcCommand(spName))
            {

                setupParameters(getCommand);


                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(getCommand, logger, useSqlDataEncription);

                var scalar = this.database.ExecuteScalar(getCommand, logger);
                if (scalar != null && scalar != DBNull.Value)
                {
                    return (T?)scalar;
                }

                return null;
            }
        }

        /// <summary>
        /// Retrieves single nullable data vlaue from database by specifying get procedure with timeout
        /// </summary>
        /// <param name="spName">Name of the procedure to retrieve data</param>
        /// <param name="setupParameters">Command uset to fill command parameter</param>
        /// <param name="useSqlDataEncription"></param>
        /// <param name="timeout"></param>
        /// <returns>Procedure result</returns>
        protected T? GetNullableSingleValueByStoredProcedureWithTimeout<T>(string spName, FillGetCommand setupParameters, bool useSqlDataEncription, int timeout) where T : struct
        {
            using (DbCommand getCommand = this.database.GetStoredProcCommand(spName))
            {
                getCommand.CommandTimeout = timeout;
                setupParameters(getCommand);


                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(getCommand, logger, useSqlDataEncription);

                var scalar = this.database.ExecuteScalar(getCommand, logger);
                if (scalar != null && scalar != DBNull.Value)
                {
                    return (T?)scalar;
                }

                return null;
            }
        }


        /// <summary>
        /// Execute non-query command
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="setupParameters"></param>
        protected void ExecuteNonQueryCommand(string spName, FillGetCommand setupParameters)
        {
            this.ExecuteNonQueryCommand(spName, setupParameters, false);
        }

        /// <summary>
        /// Execute non-query command
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="setupParameters">The setup parameters.</param>
        /// <param name="postAction">The post action.</param>
        protected void ExecuteNonQueryCommand(string spName, FillGetCommand setupParameters, Action<IDbCommand> postAction)
        {
            this.ExecuteNonQueryCommand(spName, setupParameters, postAction, false);
        }

        /// <summary>
        /// Executes the non query command.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="setupParameters">The setup parameters.</param>
        /// <param name="useSqlDataEncription">if set to <c>true</c> [use SQL data encription].</param>
        protected void ExecuteNonQueryCommand(string spName, FillGetCommand setupParameters, bool useSqlDataEncription)
        {
            this.ExecuteNonQueryCommand(spName, setupParameters, null, useSqlDataEncription);
        }

        /// <summary>
        /// Execute non-query command
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="setupParameters">The setup parameters.</param>
        /// <param name="postAction">The post action.</param>
        /// <param name="useSqlDataEncription">if set to <c>true</c> [use SQL data encription].</param>
        protected void ExecuteNonQueryCommand(string spName, FillGetCommand setupParameters, Action<IDbCommand> postAction, bool useSqlDataEncription)
        {
            using (DbCommand command = this.database.GetStoredProcCommand(spName))
            {
                setupParameters(command);

                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(command, logger, useSqlDataEncription);

                this.database.ExecuteNonQuery(command, logger);

                if (postAction != null)
                {
                    postAction(command);
                }
            }
        }

        /// <summary>
        /// Execute non-query command with specific timeout
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="setupParameters">The setup parameters.</param>
        /// <param name="postAction">The post action.</param>
        /// <param name="useSqlDataEncription">if set to <c>true</c> [use SQL data encription].</param>
        /// <param name="timeout">Time out</param>
        protected void ExecuteNonQueryCommandWithTimeOut(string spName, FillGetCommand setupParameters, Action<IDbCommand> postAction, bool useSqlDataEncription, int timeout)
        {
            using (DbCommand command = this.database.GetStoredProcCommand(spName))
            {
                command.CommandTimeout = timeout;
                setupParameters(command);

                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(command, logger, useSqlDataEncription);

                this.database.ExecuteNonQuery(command, logger);

                if (postAction != null)
                {
                    postAction(command);
                }
            }
        }

        protected void ConfigureCommandAndLogger(DbCommand aCommand, IDatabaseActivityLogger aLogger, bool useSqlDataEncription)
        {
            var originalCommandText = aCommand.CommandText;
            if (useSqlDataEncription)
            {
                DbCommandHelper.TranslateSpCommandToTextCommand(aCommand, keyAccessStatement.GetAccessStatement());
                if (aLogger != null)
                {
                    aLogger.ConfigureCommandFormat(c => originalCommandText);
                }
            }
        }

        [Obsolete]
        protected void DoExecuteAndLog(IDatabaseActivityLogger aLogger, DbCommand aCommand, Func<long?> doExecute)
        {
            var watch = new Stopwatch();
            var startTime = DateTime.UtcNow;
            Exception exception = null;

            long? recordsAffected = null;
            try
            {
                if (aLogger != null)
                {
                    aLogger.LogPreExecute(aCommand);
                }
                watch.Start();
                startTime = DateTime.UtcNow;
                recordsAffected = doExecute();
            }
            catch (Exception ex)
            {
                //if (aLogger != null)
                //{
                //    aLogger.LogException(aCommand, ex);
                //}

                exception = ex;
                throw;
            }
            finally
            {
                watch.Stop();
                if (aLogger != null)
                {
                    aLogger.LogPostExecute(aCommand, startTime, watch.ElapsedMilliseconds, recordsAffected, exception);
                }
            }
        }

        protected delegate void FillGetCommand(DbCommand command);

        protected delegate void ProccessReader(Entity entity, IDataReader reader);
        protected delegate void ProccessReader<EntityType>(EntityType entity, IDataReader reader);

        protected virtual void SetSearchLimit(DbCommand command, out long limit)
        {
            limit = long.MaxValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ListItem"></typeparam>
        /// <param name="spName"></param>
        /// <param name="setupParameters"></param>
        /// <param name="map"></param>
        /// <param name="useSqlDataEncription"></param>
        /// <param name="processPostItemRead"></param>
        /// <param name="readerHandler"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        protected List<ListItem> GetListByStoredProcedureWithTimeout<ListItem>(
            string spName,
            FillGetCommand setupParameters,
            Func<IDataReader, List<ListItem>> map,
            bool useSqlDataEncription,
            Action<ListItem> processPostItemRead,
            ProccessReader<List<ListItem>> readerHandler,
            int? timeout)
        {
            using (DbCommand getCommand = this.database.GetStoredProcCommand(spName))
            {
                if (timeout.HasValue)
                {
                    getCommand.CommandTimeout = timeout.Value;
                }
                setupParameters(getCommand);

                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(getCommand, logger, useSqlDataEncription);

                List<ListItem> items;

                using (IDataReader entityReader = this.database.ExecuteReader(getCommand, logger))
                {
                    items = map(entityReader);

                    if (readerHandler != null)
                    {
                        readerHandler(items, entityReader);
                    }
                }

                if (processPostItemRead != null && items != null)
                {
                    items.ForEach(processPostItemRead);
                }

                return new List<ListItem>(items);
            }
        }
    }
}
