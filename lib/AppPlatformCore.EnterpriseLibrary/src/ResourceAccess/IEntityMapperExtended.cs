using System.Collections.Generic;
using System.Data;
using System;
namespace AppPlatform.Core.EnterpriseLibrary.ResourceAccess
{

    /// <summary>
    /// Defines methods for sql command - data object mapping
    /// </summary>
    /// <typeparam name="Entity">Type of the data object</typeparam>
    /// <typeparam name="EntityKey">Type of the data object key</typeparam>
    public interface IEntityDocumentSummaryExtendedMapper<Entity, EntityKey> : IEntityMapperExtended<Entity, EntityKey>
    {
        ///// <summary>
        ///// Defines custom mapping
        ///// </summary>
        ///// <param name="reader"></param>
        ///// <returns></returns>
        //Entity CreateSingleFromReaderCustom(IDataReader reader);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="pendingReader"></param>
        void FillFromReaderCustomFullDocument(Entity entity, IDataReader pendingReader);
    }

    /// <summary>
    /// Defines methods for sql command - data object mapping
    /// </summary>
    /// <typeparam name="Entity">Type of the data object</typeparam>
    /// <typeparam name="EntityKey">Type of the data object key</typeparam>
    public interface IEntityReadLevelMapper<Entity, EntityKey> : IEntityMapperExtended<Entity, EntityKey>
    {
        /// <summary>
        /// Creates list of objects from pending data reader
        /// </summary>
        /// <param name="reader">Pending reader</param>
        /// <returns></returns>
        List<Entity> CreateListFromReader(IDataReader reader, int searchLevel, Enum displayHistoryNodeType);
    }

    /// <summary>
    /// Defines methods for sql command - data object mapping
    /// </summary>
    /// <typeparam name="Entity">Type of the data object</typeparam>
    /// <typeparam name="EntityKey">Type of the data object key</typeparam>
    public interface IEntityMapperExtended<Entity, EntityKey> : IEntityReadMapper<Entity, EntityKey>
    {
        /// <summary>
        /// Fill insert command with parameters
        /// </summary>
        /// <param name="entity">Data object</param>
        /// <param name="command">Command to fill</param>
        void FillInsertCommand(Entity entity, IDbCommand command);
        
        /// <summary>
        ///  Performs additional proccessing after insert is completed
        /// </summary>
        /// <param name="entity">Data object</param>
        /// <param name="command">Command to fill</param>
        void ProcessPostInsert(Entity entity, IDbCommand command);

        /// <summary>
        /// Fill update command with parameters
        /// </summary>
        /// <param name="entity">Data object</param>
        /// <param name="command">Command to fill</param>
        void FillUpdateCommand(Entity entity, IDbCommand command);

        /// <summary>
        /// Performs additional proccessing after update is completed
        /// </summary>
        /// <param name="entity">Data which are updated</param>
        /// <param name="command">Command</param>
        void ProcessPostUpdate(Entity entity, IDbCommand command);

        /// <summary>
        /// Fill delete command with parameters
        /// </summary>
        /// <param name="entity">Data object</param>
        /// <param name="command">Command to fill</param>
        void FillDeleteCommand(Entity entity, IDbCommand command);
    }

    /// <summary>
    /// Defines methods for sql command - data object mapping for read proccesses
    /// </summary>
    /// <typeparam name="Entity">Type of the data object</typeparam>
    /// <typeparam name="EntityKey">Type of the data object key</typeparam>
    public interface IEntityReadMapper<Entity, EntityKey> : IEntityMapper<Entity>
    {
        /// <summary>
        /// Fill get command
        /// </summary>
        /// <param name="getCommand">command to fill</param>
        /// <param name="key">Entity key identifier</param>
        void FillGetCommand(IDbCommand getCommand, EntityKey key);
    }

    /// <summary>
    /// Defines methods for sql command - data object mapping for creating object
    /// </summary>
    /// <typeparam name="Entity">Type of the data object</typeparam>
    public interface IEntityMapper<Entity>
    {
        /// <summary>
        /// Creates list of objects from pending data reader
        /// </summary>
        /// <param name="reader">Pending reader</param>
        /// <returns></returns>
        List<Entity> CreateListFromReader(IDataReader reader);

        /// <summary>
        /// Creates single data object from pending reader
        /// </summary>
        /// <param name="reader">Pending reader</param>
        /// <returns></returns>
        Entity CreateSingleFromReader(IDataReader reader);
    }
}
