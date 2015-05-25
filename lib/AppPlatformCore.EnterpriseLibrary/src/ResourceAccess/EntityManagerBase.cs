using System.Collections.Generic;
using System.Data.Common;
using AppPlatform.Core.EnterpriseLibrary.DatabaseUtility;

namespace AppPlatform.Core.EnterpriseLibrary.ResourceAccess
{
    /// <summary>
    /// Base class implementing base data access methods
    /// </summary>
    /// <typeparam name="Entity">Type of the data object</typeparam>
    /// <typeparam name="EntityMapper">Type of the mapper object used to prepare data commands</typeparam>
    /// <typeparam name="EntityKey">Type of the key filed for the data object</typeparam>
    public abstract class EntityManagerBase<Entity, EntityMapper, EntityKey>
        : EntityReadManagerBase<Entity, EntityMapper, EntityKey>,
          IEntityManager<Entity, EntityKey>
        where Entity : class
        where EntityMapper : IEntityMapperExtended<Entity, EntityKey>
    {
        private readonly IKeyAccessStatement keyAccessStatement;
        private readonly Dictionary<SqlCommand, EncryptCommand> commandNames;

        private class EncryptCommand
        {
            public string CommandName { get; set; }
            public bool Encrypted { get; set; }
        }

        protected EntityManagerBase(EntityMapper mapper, string connectionStringKey)
            : base(mapper, connectionStringKey)
        {
            this.commandNames = new Dictionary<SqlCommand, EncryptCommand>();
        }

        protected EntityManagerBase(EntityMapper mapper, string connectionStringKey, IKeyAccessStatement keyAccessStatement)
            : base(mapper, connectionStringKey, keyAccessStatement)
        {
            this.keyAccessStatement = keyAccessStatement;
            this.commandNames = new Dictionary<SqlCommand, EncryptCommand>();
        }

        protected sealed override string GetCommandName
        {
            get { return this.commandNames[SqlCommand.Get].CommandName; }
        }

        /// <summary>
        /// Add command to list of available operations
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandName"></param>
        /// <returns></returns>
        protected EntityManagerBase<Entity, EntityMapper, EntityKey> AddCommand(SqlCommand command, string commandName)
        {
            this.AddCommand(command, commandName, false);
            return this;
        }
        
        /// <summary>
        /// Add command to list of available operations
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandName"></param>
        /// <param name="useSqlDataEncription"></param>
        /// <returns></returns>
        protected EntityManagerBase<Entity, EntityMapper, EntityKey> AddCommand(SqlCommand command, string commandName, bool useSqlDataEncription)
        {
            this.commandNames.Add(
                command,
                new EncryptCommand { CommandName = commandName, Encrypted = useSqlDataEncription }
                );
            return this;
        }

        /// <summary>
        /// Insert new object to the database
        /// </summary>
        /// <param name="entity">Object to insert</param>
        public virtual void Insert(Entity entity)
        {
            using (DbCommand insertCommand = this.database.GetStoredProcCommand(this.commandNames[SqlCommand.Insert].CommandName))
            {
                this.entityMapper.FillInsertCommand(entity, insertCommand);

                var logger = this.GetLogger();
                var originalCommandText = insertCommand.CommandText;
                if (this.commandNames[SqlCommand.Insert].Encrypted)
                {
                    DbCommandHelper.TranslateSpCommandToTextCommand(insertCommand, keyAccessStatement.GetAccessStatement());
                    logger.ConfigureCommandFormat(c => originalCommandText);
                }

                this.database.ExecuteNonQuery(insertCommand, logger);
                this.entityMapper.ProcessPostInsert(entity, insertCommand);
            }
        }

        /// <summary>
        /// Update existing object in database
        /// </summary>
        /// <param name="newValue">Object containing new data</param>
        public virtual void Update(Entity newValue)
        {
            using (DbCommand updateCommand = this.database.GetStoredProcCommand(this.commandNames[SqlCommand.Update].CommandName))
            {
                this.entityMapper.FillUpdateCommand(newValue, updateCommand);

                var logger = this.GetLogger();
                this.ConfigureCommandAndLogger(updateCommand, logger, this.commandNames[SqlCommand.Update].Encrypted);
                
                this.database.ExecuteNonQuery(updateCommand, logger);
                this.entityMapper.ProcessPostUpdate(newValue, updateCommand);
            }
        }

        /// <summary>
        /// Delete existing object from database
        /// </summary>
        /// <param name="newValue">Object to delete</param>
        public void Delete(Entity newValue)
        {
            using (DbCommand deleteCommand = this.database.GetStoredProcCommand(this.commandNames[SqlCommand.Delete].CommandName))
            {
                this.entityMapper.FillDeleteCommand(newValue, deleteCommand);
                var logger = this.GetLogger();

                this.ConfigureCommandAndLogger(deleteCommand, logger, this.commandNames[SqlCommand.Delete].Encrypted);
                this.database.ExecuteNonQuery(deleteCommand, logger);
            }
        }
    }
}
