using System.Xml.Linq;

namespace AppPlatform.Core.EnterpriseLibrary.Comparision
{
    /// <summary>
    /// Declares methods for comparing current instance with new data
    /// </summary>
    public interface IComparableData
    {
        /// <summary>
        /// methods for comparing current instance with new data
        /// </summary>
        /// <returns></returns>
        bool HasSameData(object newData, XElement differentData, PropertyFilter propertyFilter);

    }
}