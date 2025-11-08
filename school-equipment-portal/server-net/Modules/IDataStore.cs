using SchoolEquipmentApi.DTOs;

namespace SchoolEquipmentApi.Modules
{
    /// <summary>
    /// Interface for data storage implementations
    /// Provides abstraction over different storage mechanisms (File, MongoDB, etc.)
    /// </summary>
    public interface IDataStore
    {
        /// <summary>
        /// Read all data from the storage
        /// </summary>
        /// <returns>Complete data structure containing all entities</returns>
        Data Read();

        /// <summary>
        /// Write all data to the storage
        /// </summary>
        /// <param name="data">Data structure to persist</param>
        void Write(Data data);
    }
}