using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml.Linq;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;
using AppPlatform.Core.EnterpriseLibrary.Object;

namespace AppPlatform.Core.EnterpriseLibrary.Comparision
{
    /// <summary>
    /// Delegate for filtering type properties
    /// </summary>
    /// <param name="parentProperty">Parent property</param>
    /// <param name="objectType">Type of object whose properties are filtered</param>
    /// <returns></returns>
    public delegate List<PropertyInfo> PropertyFilter(PropertyInfo parentProperty, Type objectType);


    /// <summary>
    /// Utility class for comparing objects by their property values.
    /// </summary>
    public class DataComparer
    {
        /// <summary>
        /// Default PropertyFilter gets Properties for skip
        /// </summary>
        private static PropertyFilter defaultPropertyFilter;

        #region Public methods

        /// <summary>
        /// Check if properties marked with specified attributes have same data, properties which are collection are not considered
        /// </summary>
        /// <param name="firstData">Original data</param>
        /// <param name="secondData">New data</param>
        /// <param name="differentData">XML with differences</param>
        /// <param name="propertyFilter">Function to filter object properties for comparision</param>
        public static bool HasSameData<T>(T firstData, T secondData, out XDocument differentData, PropertyFilter propertyFilter)
        //where T : class
        {
            return HasSameData<T>(firstData, secondData, out differentData, propertyFilter, null, null);
        }

        /// <summary>
        /// Check if properties marked with specified attributes have same data, properties which are collection are not considered
        /// </summary>
        /// <param name="firstData">Original data</param>
        /// <param name="secondData">New data</param>
        /// <param name="differentData">XML with differences</param>
        /// <param name="propertyFilter">Function to filter object properties for comparision</param>
        /// <param name="listComparers">Comparers for specified list element type</param>
        public static bool HasSameData<T>(T firstData, T secondData, out XDocument differentData, PropertyFilter propertyFilter, Dictionary<Type, IListCompare> listComparers)
        //where T : class
        {
            return HasSameData<T>(firstData, secondData, out differentData, propertyFilter, null, listComparers);
        }

        public static bool HasSameData<T>(T firstData, T secondData, ref XDocument differentData, PropertyFilter propertyFilter, long? maxDataOrder)
        {
            return HasSameData<T>(firstData, secondData, out differentData, propertyFilter, maxDataOrder, null);
        }

        /// <summary>
        /// Check if properties marked with specified attributes have same data, properties which are collection are not considered
        /// </summary>
        /// <param name="firstData">Original data</param>
        /// <param name="secondData">New data</param>
        /// <param name="differentData">XML with differences</param>
        /// <param name="propertyFilter">Function to filter object properties for comparision</param>
        /// <param name="maxDataOrder">max Order in DataMember Attribute of property</param>
        /// <param name="listComparers">Comparers for specified list element type</param>
        public static bool HasSameData<T>(T firstData, T secondData, out XDocument differentData, PropertyFilter propertyFilter, long? maxDataOrder, Dictionary<Type, IListCompare> listComparers)
        //where T : class
        {
            differentData = new XDocument(new XElement("ChangedProperties", new XAttribute("culture", Thread.CurrentThread.CurrentCulture)));

            var basicCheck = BasicCheck(firstData, secondData);

            if (basicCheck.HasValue)
            {
                return basicCheck.Value;
            }

            return HasSameValue(null, firstData, secondData, differentData.Root, propertyFilter, maxDataOrder, listComparers);
        }

        public static bool HasSameDataAsComparable<T>(T firstData, T secondData, PropertyFilter propertyFilter)
            where T : IComparableData
        {
            var basicCheck = BasicCheck(firstData, secondData);

            if (basicCheck.HasValue)
            {
                return basicCheck.Value;
            }

            return firstData.HasSameData(secondData, null, propertyFilter);
        }

        public static bool HasSameDataAsComparable<T>(T firstData, T secondData, out XDocument differentData, PropertyFilter propertyFilter)
            where T : IComparableData
        {
            differentData = new XDocument(new XElement("ChangedProperties", new XAttribute("culture", Thread.CurrentThread.CurrentCulture)));

            var basicCheck = BasicCheck(firstData, secondData);

            if (basicCheck.HasValue)
            {
                return basicCheck.Value;
            }

            return firstData.HasSameData(secondData, differentData.Root, propertyFilter);
        }

        public static bool? BasicCheck<T>(T firstData, T secondData)
        {
            if (typeof(T).IsValueType)
            {
                return firstData.Equals(secondData);
            }

            if (firstData == null)
            {
                return secondData == null;
            }
            if (secondData == null)
            {
                return false;
            }

            if (firstData.Equals(secondData))
            {
                return true;
            }

            return null;
        }

        /// <summary>
        /// Check if properties marked with specified attributes have same data, properties which are collection are not considered
        /// </summary>
        [Obsolete]
        public static bool HasSameData<T>(T firstData, T secondData, out List<PropertyInfo> differentData, PropertyFilter propertyFilter)
        {
            differentData = new List<PropertyInfo>();

            if (firstData.Equals(secondData))
            {
                return true;
            }

            return HasSameValue(null, firstData, secondData, differentData, propertyFilter);
        }

        public static ListChanges<TMember> GetListChanges<TMember>(ICollection<TMember> originalList, ICollection<TMember> newList, Func<TMember, object> elementIdentifier, PropertyFilter propertyFilter)
        //where TMember : class
        {
            return GetListChanges(originalList, newList, elementIdentifier, propertyFilter, null, null);
        }

        /// <summary>
        /// Checks for changes in original list and new list
        /// </summary>
        /// <typeparam name="TMember">Type of list members</typeparam>
        /// <param name="originalList">List with original data</param>
        /// <param name="newList">List with new data</param>
        /// <param name="elementIdentifier">Name of the property to uniquely identify data</param>
        /// <param name="propertyFilter">The property filter.</param>
        /// <param name="listComparers">The list comparers.</param>
        /// <param name="useIComparableComparision">IComparableComparision should be used or not.</param>
        /// <returns></returns>
        public static ListChanges<TMember> GetListChanges<TMember>(ICollection<TMember> originalList, ICollection<TMember> newList,
            Func<TMember, object> elementIdentifier, PropertyFilter propertyFilter, Dictionary<Type, IListCompare> listComparers,
            bool? useIComparableComparision) //where TMember : class
        {
            Assert.IsNotNull(elementIdentifier, "Unique identifier function for list elements must be specified");

            List<TMember> originalListData = originalList != null ? new List<TMember>(originalList) : new List<TMember>();
            List<TMember> newListData = newList != null ? new List<TMember>(newList) : new List<TMember>();

            if (elementIdentifier == null)
            {
                throw new ArgumentException("elementIdentifier");
            }

            List<TMember> removedElements = originalListData.FindAll(element => !newListData.Exists(newListElement =>
                                                                   (
                                                                       elementIdentifier(newListElement).Equals(elementIdentifier(element))
                                                                   )
                                                    ));

            List<TMember> addedElements = newListData.FindAll(newListElement => !originalListData.Exists(element =>
                                                                  (
                                                                      elementIdentifier(element).Equals(elementIdentifier(newListElement))
                                                                  )
                                                   ));

            List<TMember> sameListElementsInOldList = originalListData.Except(removedElements).ToList();

            var changes = new ListChanges<TMember>
                                               {
                                                   AddedElements = addedElements,
                                                   RemovedElements = removedElements,
                                                   ModifiedElements = new List<TMember>()
                                               };

            var differentData = new XDocument();

            foreach (var member in newListData.Except(addedElements))
            {
                TMember memberTemp = member;

                var correspondingElement = sameListElementsInOldList.Find(element => (
                                                                                        elementIdentifier(memberTemp) != null &&
                                                                                        elementIdentifier(memberTemp)
                                                                                        .Equals(elementIdentifier(element))
                                                                                    ));
                bool changed;

                if (memberTemp is IComparableData && (!useIComparableComparision.HasValue || useIComparableComparision.Value))
                {
                    changed = !((memberTemp as IComparableData).HasSameData(correspondingElement, differentData.Root, propertyFilter));
                }
                else
                {
                    changed = !HasSameData(
                                   member,
                                   correspondingElement,
                                   out differentData,
                                   propertyFilter,
                                   null,
                                   listComparers
                                   );
                }

                if (changed)
                {
                    changes.ModifiedElements.Add(member);
                }
            }

            return changes;
        }

        /// <summary>
        /// Find all properties in list wich have smo specified declaring type
        /// </summary>
        /// <param name="properties">List to filter</param>
        /// <param name="declaringType">Declaring type</param>
        /// <returns></returns>
        public static List<PropertyInfo> GetPropertiesForDeclaringType(List<PropertyInfo> properties, Type declaringType)
        {
            return properties.FindAll(property => property.DeclaringType == declaringType);
        }

        /// <summary>
        /// Get structured list of changes
        /// </summary>
        /// <param name="changes"></param>
        /// <returns></returns>
        public static Dictionary<string, OldNewValuePair> GetListOfChanges(XDocument changes)
        {
            var list = new Dictionary<string, OldNewValuePair>();
            if (changes == null || changes.FirstNode == null)
            {
                return list;
            }

            foreach (var change in changes.Descendants("oldValue"))
            {
                XElement description = change.Parent;

                var memberChain =
                    from element
                    in description.AncestorsAndSelf()
                    where element.Attribute("name") != null
                    select element.Attribute("name").Value;

                var memberPath = string.Join(".", memberChain.Reverse().ToArray());

                list.Add(memberPath, new OldNewValuePair { Old = ((XElement)change.NextNode).Value, New = change.Value });
            }

            return list;
        }

        /// <summary>
        /// Determinata are list equal.
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="aListChanges">ListChcanges determinated whit function GetListChanges</param>
        /// <returns>Return True if list are equal. Else return False.</returns>
        public static bool AreListEqual<TMember>(ListChanges<TMember> aListChanges)
        {
            if (aListChanges.AddedElements.Count == 0 && aListChanges.ModifiedElements.Count == 0 && aListChanges.RemovedElements.Count == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determinata are list equal.
        /// </summary>
        /// <typeparam name="TMember">Type of list members</typeparam>
        /// <param name="originalList">List containing original data</param>
        /// <param name="newList">List with new data</param>
        /// <param name="elementIdentifier">Name of the property to uniquely identufy data</param>
        /// <param name="propertyFilter"></param>
        /// <returns></returns>
        public static bool AreListEqual<TMember>(ICollection<TMember> originalList, ICollection<TMember> newList, Func<TMember, object> elementIdentifier, PropertyFilter propertyFilter)
            where TMember : class
        {
            return AreListEqual(GetListChanges(originalList, newList, elementIdentifier, propertyFilter));
        }

        #region Methods for comparing single value

        public static bool IsRefValueEqual<T>(T firstValue, T secondValue) where T : class
        {
            if (firstValue == null)
            {
                return secondValue == null;
            }

            if (secondValue == null)
            {
                return false;
            }

            if (firstValue.Equals(secondValue))
            {
                return true;
            }

            return false;
        }

        public static bool IsValueEqual<T>(T firstValue, T secondValue) where T : struct
        {
            if (firstValue.Equals(secondValue))
                return true;

            return false;
        }

        public static bool IsValueEqual<T>(T? firstValue, T? secondValue) where T : struct
        {
            if (firstValue.Equals(secondValue))
                return true;

            return false;
        }

        public static bool IsValueEqual(object firstValue, object secondValue)
        {
            // If both null then they are the same
            if ((firstValue == null || firstValue == DBNull.Value) && (secondValue == null || secondValue == DBNull.Value))
                return true;

            // If only one is null they are different
            if ((firstValue == null || firstValue == DBNull.Value) || (secondValue == null || secondValue == DBNull.Value))
                return true;

            if (firstValue is long) return ((long) firstValue).Equals((long) secondValue);
            if (firstValue is decimal) return ((decimal)firstValue).Equals((decimal)secondValue);
            if (firstValue is bool) return ((bool)firstValue).Equals((bool)secondValue);
            if (firstValue is DateTime) return ((DateTime)firstValue).Equals((DateTime)secondValue);
            if (firstValue is string) return ((string)firstValue).Equals((string)secondValue);

            throw new NotImplementedException(String.Format("Type {0} not supported", firstValue.GetType()));
        }

        #endregion

        public static void SetDefaultPropertyFilter(PropertyFilter filter)
        {
            defaultPropertyFilter = filter;
        }

        #endregion

        #region  Private methods

        private static bool HasSameValue(PropertyInfo parentProperty, object firstData, object secondData, XElement differentData, PropertyFilter propertyFilter)
        {
            return HasSameValue(parentProperty, firstData, secondData, differentData, propertyFilter, null, null);
        }

        public static bool HasSameValue(PropertyInfo parentProperty, object firstData, object secondData, XElement differentData, PropertyFilter propertyFilter, Dictionary<Type, IListCompare> listComparers)
        {
            return HasSameValue(parentProperty, firstData, secondData, differentData, propertyFilter, null, listComparers);
        }

        public static bool HasSameValue(PropertyInfo parentProperty, object firstData, object secondData, XElement differentData, PropertyFilter propertyFilter, long? maxDataOrder)
        {
            return HasSameValue(parentProperty, firstData, secondData, differentData, propertyFilter, maxDataOrder, null);
        }

        public static bool HasSameValue(PropertyInfo parentProperty, object firstData, object secondData, XElement differentData, PropertyFilter propertyFilter, long? maxDataOrder, Dictionary<Type, IListCompare> listComparers)
        {
            bool hasSameValue = true;

            List<PropertyInfo> propertiesToSkip = defaultPropertyFilter != null ?
                                                defaultPropertyFilter(parentProperty, firstData.GetType()) : null;

            List<PropertyInfo> propertiesToCheck;

            if (propertyFilter == null)
            {
                propertiesToCheck = Safe.HasItems(propertiesToSkip) ?
                                         firstData.GetType().GetProperties().Except(propertiesToSkip).ToList()
                                                 : firstData.GetType().GetProperties().ToList();
            }
            else
            {
                propertiesToCheck = Safe.HasItems(propertiesToSkip) ?
                                    propertyFilter(parentProperty, firstData.GetType()).Except(propertiesToSkip).ToList()
                                    : propertyFilter(parentProperty, firstData.GetType()).ToList();
            }

            //check for max Data Order in DataMember Attribute of Properties
            if (maxDataOrder != null)
            {
                propertiesToCheck = propertiesToCheck.FindAll(prop =>
                    {
                        var a = (DataMemberAttribute[])prop.GetCustomAttributes(typeof(DataMemberAttribute), false);
                        return (a.Count(att => att.Order < maxDataOrder) > 0);
                    });
            }

            foreach (var propertyInfo in propertiesToCheck)
            {
                var typeToCheck = GetTypeToCheck(firstData, secondData, propertyInfo);

                var firstObjectData = propertyInfo.GetValue(firstData, null);
                var secondObjectData = propertyInfo.GetValue(secondData, null);

                firstObjectData = PrepareDataToComparisionValues(firstObjectData, propertyInfo.PropertyType);
                secondObjectData = PrepareDataToComparisionValues(secondObjectData, propertyInfo.PropertyType);

                if (typeToCheck.IsValueType
                    || typeToCheck == typeof(string)
                    || IsDateTimeType(typeToCheck))
                {
                    {
                        if (typeToCheck == typeof(string))
                        {
                            var firstValue = firstObjectData as string;
                            var secondValue = secondObjectData as string;

                            if (firstValue != null
                                && secondValue != null
                                && firstValue.Replace("\r\n", "\n") == secondValue.Replace("\r\n", "\n"))
                            {
                                continue;
                            }
                        }

                        if (IsDateTimeType(typeToCheck))
                        {
                            var firstValue = firstObjectData as DateTime?;
                            var secondValue = secondObjectData as DateTime?;

                            if (firstValue.HasValue && secondValue.HasValue
                                && Math.Abs((firstValue.Value - secondValue.Value).TotalMilliseconds) < 4)
                            {
                                continue;
                            }
                        }

                        //if (propertyInfo.GetValue(firstData, null) == propertyInfo.GetValue(secondData, null)) //ne porede se ova 2 polja vec objectData koji su pripremljeni za poredjenje
                        if (firstObjectData == secondObjectData)
                        {
                            continue;
                        }

                        if ((firstObjectData == null && secondObjectData != null)
                            || (firstObjectData != null && secondObjectData == null)
                            || !firstObjectData.Equals(secondObjectData))
                        {
                            AppendChangedDataInfo(differentData, propertyInfo, firstData, secondData);

                            if (differentData == null)
                            {
                                return false;
                            }
                            hasSameValue = false;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else if (typeToCheck.GetInterface("ICollection") != null)
                {
                    if (listComparers == null)
                    {
                        continue;
                    }

                    var firstCollection = propertyInfo.GetValue(firstData, null) as ICollection;
                    var secondCollection = propertyInfo.GetValue(secondData, null) as ICollection;


                    var collectionElemType = GetCollectionElemType(firstCollection, secondCollection, propertyInfo);

                    if (collectionElemType == null)
                    {
                        continue;
                    }

                    if (listComparers.ContainsKey(collectionElemType))
                    {
                        if (listComparers[collectionElemType].Compare(firstCollection, secondCollection, listComparers, propertyFilter))
                        {
                            if (differentData == null)
                            {
                                return false;
                            }

                            hasSameValue = false;
                        }
                    }
                }

                else if (typeToCheck.IsClass || typeToCheck.IsInterface)
                {
                    if (propertyInfo.GetValue(firstData, null) != null
                        && propertyInfo.GetValue(secondData, null) != null)
                    {
                        var subPropertyChangedData = new XElement("property", new XAttribute("name", propertyInfo.Name));

                        if (propertyInfo.PropertyType.GetInterface("IComparableData") != null)
                        {
                            hasSameValue &= (propertyInfo.GetValue(firstData, null) as IComparableData).HasSameData(
                                propertyInfo.GetValue(secondData, null), subPropertyChangedData, propertyFilter);
                        }
                        else if (ObjectHelper.IsSomeKindOfXml(typeToCheck))
                        {
                            //XmlCompare.HasSameData(propertyInfo.GetValue(firstData, null), propertyInfo.GetValue(secondData, null));

                            //very stupid comparision. Only temporary!!!!
                            hasSameValue =
                                ObjectHelper.GetXmlString(propertyInfo.GetValue(firstData, null)).Equals(
                                    ObjectHelper.GetXmlString(propertyInfo.GetValue(secondData, null)));
                        }
                        else
                        {

                            hasSameValue &= HasSameValue(
                                propertyInfo,
                                propertyInfo.GetValue(firstData, null),
                                propertyInfo.GetValue(secondData, null),
                                subPropertyChangedData,
                                propertyFilter,
                                listComparers
                                );
                        }

                        if (!hasSameValue && differentData == null)
                        {
                            return false;
                        }

                        if (subPropertyChangedData.HasElements && differentData != null)
                        {
                            differentData.Add(subPropertyChangedData);
                        }
                    }
                    else
                    {
                        if (propertyInfo.GetValue(firstData, null) != null
                            || propertyInfo.GetValue(secondData, null) != null)
                        {
                            if (differentData == null)
                            {
                                return false;
                            }

                            var subPropertyChangedData = new XElement("property",
                                                                           new XAttribute("name", propertyInfo.Name),
                                                                           new XAttribute("NullValue",
                                                                                          propertyInfo.GetValue(
                                                                                              firstData, null) == null
                                                                                              ? "old"
                                                                                              : "new"));

                            if (propertyInfo.GetValue(firstData, null) == null)
                            {
                                AppendObjectDataInfo(subPropertyChangedData,
                                                     propertyInfo.GetValue(secondData, null),
                                                     true);

                            }
                            else
                            {
                                AppendObjectDataInfo(subPropertyChangedData,
                                                     propertyInfo.GetValue(firstData, null),
                                                     false);
                            }

                            if (differentData != null)
                            {
                                differentData.Add(subPropertyChangedData);
                            }

                            hasSameValue = false;
                            //TODO a sta ako je jedan od njih NULL
                        }
                    }
                }
                else if (propertyInfo.GetValue(firstData, null) == null)
                {
                    if (!(propertyInfo.GetValue(secondData, null) == null))
                    {
                        if (differentData == null)
                        {
                            return false;
                        }

                        AppendChangedDataInfo(differentData, propertyInfo, firstData, secondData);

                        hasSameValue = false;

                        continue;
                    }
                }
                else if (propertyInfo.GetValue(secondData, null) == null)
                {
                    if (differentData == null)
                    {
                        return false;
                    }

                    AppendChangedDataInfo(differentData, propertyInfo, firstData, secondData);

                    hasSameValue = false;

                    continue;
                }
            }
            return hasSameValue;
        }

        private static object PrepareDataToComparisionValues(object data, Type dataType)
        {
            if (dataType.IsValueType)
            {
                if (dataType == typeof(DateTime))
                {
                    return DateTime.ParseExact(((DateTime)data).ToString("MM/dd/yyyy hh:mm:ss.f"), "MM/dd/yyyy hh:mm:ss.f", null);
                }

                if (dataType.IsGenericType && dataType == typeof(Nullable<DateTime>))
                {
                    if (data != null)
                    {
                        return DateTime.ParseExact(((DateTime?)data).Value.ToString("MM/dd/yyyy hh:mm:ss.f"), "MM/dd/yyyy hh:mm:ss.f", null);
                    }
                    return data;
                }

                return data;
            }

            if (dataType == typeof(string))
            {
                if (data != null)
                {
                    if (data as string == string.Empty)
                    {
                        return null;
                    }
                    return ((string)data).Replace("\r\n", "\n");
                }

                return data;
            }

            return data;
        }

        private static bool IsDateTimeType(Type dataType)
        {
            return dataType == typeof(DateTime)
                   || dataType.IsGenericType && dataType == typeof(Nullable<DateTime>);
        }

        private static Type GetTypeToCheck(object firstData, object secondData, PropertyInfo propertyInfo)
        {
            var typeToCheck = propertyInfo.PropertyType;

            if (propertyInfo.GetValue(firstData, null) != null)
            {
                typeToCheck = propertyInfo.GetValue(firstData, null).GetType();
            }
            else if (propertyInfo.GetValue(secondData, null) != null)
            {
                typeToCheck = propertyInfo.GetValue(secondData, null).GetType();
            }
            return typeToCheck;
        }

        private static Type GetCollectionElemType(ICollection firstData, ICollection secondData, PropertyInfo propertyInfo)
        {
            Type elemType = null;

            if (firstData != null)
            {
                foreach (var item in firstData)
                {
                    return item.GetType();
                }
            }

            if (secondData != null)
            {
                foreach (var item in secondData)
                {
                    return item.GetType();
                }
            }

            return elemType;
        }

        private static void AppendChangedDataInfo(XContainer data, PropertyInfo propertyInfo, object firstData, object secondData)
        {
            if (data == null)
            {
                return;
            }

            data.Add(
                new XElement("property",
                             new XAttribute("name", propertyInfo.Name),
                             new XElement("oldValue", propertyInfo.GetValue(firstData, null)),
                             new XElement("newValue", propertyInfo.GetValue(secondData, null)))
                );
        }

        private static void AppendObjectDataInfo(XContainer data, object objectData, bool isFirstNull)
        {
            if (data == null)
            {
                return;
            }

            if (objectData == null) return;

            var objectType = objectData.GetType();

            if (ObjectHelper.IsSomeKindOfXml(objectType))
            {
                data.Add(new XElement("NewValue", objectData.ToString()));

                return;
            }

            IList<PropertyInfo> propertyInfos = objectData.GetType().GetProperties().ToList();
            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetIndexParameters().Length != 0)
                {
                    continue;
                }

                if (propertyInfo.PropertyType.GetInterface("ICollection") != null)
                {
                    continue;
                }
                if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string))
                {
                    data.Add(new XElement("property",
                                          new XAttribute("name", propertyInfo.Name),
                                          new XAttribute("oldValue", isFirstNull ? "null" : Convert.ToString(propertyInfo.GetValue(objectData, null))),
                                          new XAttribute("newValue", !isFirstNull ? "null" : Convert.ToString(propertyInfo.GetValue(objectData, null)))
                                 )
                        );
                }
                else
                {
                    if (propertyInfo.PropertyType.IsClass || propertyInfo.PropertyType.IsInterface)
                    {
                        if (propertyInfo.GetValue(objectData, null) == null)
                        {
                            data.Add(new XElement("property",
                                                  new XAttribute("name", propertyInfo.Name),
                                                  new XAttribute("value", "null")
                                         )
                                );
                        }
                        else
                        {
                            var subProperties =
                                new XElement("property",
                                             new XAttribute("name", propertyInfo.Name));

                            if (ObjectHelper.IsSomeKindOfXml(propertyInfo.PropertyType))
                            {
                                data.Add(new XElement("property",
                                  new XAttribute("name", propertyInfo.Name),
                                  new XAttribute("oldValue", isFirstNull ? "null" : ObjectHelper.GetXmlString(propertyInfo.GetValue(objectData, null)))),
                                  new XAttribute("newValue", !isFirstNull ? "null" : ObjectHelper.GetXmlString(propertyInfo.GetValue(objectData, null))));
                            }
                            else
                            {
                                AppendObjectDataInfo(subProperties, propertyInfo.GetValue(objectData, null), isFirstNull);
                            }

                            data.Add(subProperties);
                        }
                    }
                }
            }
        }

        private static bool HasSameValue(PropertyInfo parentProperty, object firstData, object secondData, List<PropertyInfo> differentData, PropertyFilter propertyFilter)
        {
            return HasSameValue(parentProperty, firstData, secondData, differentData, propertyFilter, null);
        }

        private static bool HasSameValue(PropertyInfo parentProperty, object firstData, object secondData, List<PropertyInfo> differentData, PropertyFilter propertyFilter, long? maxDataOrder)
        {
            bool hasSameValue = true;

            List<PropertyInfo> propertiesToCheck;

            if (propertyFilter == null)
            {
                propertiesToCheck = firstData.GetType().GetProperties().ToList();
            }
            else
            {
                propertiesToCheck = propertyFilter(parentProperty, firstData.GetType());
            }

            //check for max Data Order in DataMember Attribute of Properties
            if (maxDataOrder != null)
            {
                propertiesToCheck.FindAll(prop =>
                {
                    var a = (DataMemberAttribute[])prop.GetCustomAttributes(typeof(DataMemberAttribute), false);
                    return (a.Count<DataMemberAttribute>(att => att.Order < maxDataOrder) > 0);
                });
            }

            foreach (var propertyInfo in propertiesToCheck)
            {
                if (propertyInfo.GetValue(firstData, null) == null)
                {
                    if (!(propertyInfo.GetValue(secondData, null) == null))
                    {
                        differentData.Add(propertyInfo);
                        hasSameValue = false;

                        continue;
                    }
                }
                else if (propertyInfo.GetValue(secondData, null) == null)
                {
                    differentData.Add(propertyInfo);
                    hasSameValue = false;

                    continue;
                }
                else if (propertyInfo.PropertyType.IsValueType || propertyInfo.PropertyType == typeof(string))
                {
                    {
                        if (!propertyInfo.GetValue(firstData, null).Equals(propertyInfo.GetValue(secondData, null)))
                        {
                            differentData.Add(propertyInfo);
                            hasSameValue = false;
                        }
                    }
                }
                else
                {
                    if (propertyInfo.GetValue(firstData, null) is ICollection)
                    {
                        continue;
                    }

                    hasSameValue &= HasSameValue(propertyInfo, propertyInfo.GetValue(firstData, null), propertyInfo.GetValue(secondData, null), differentData, propertyFilter);
                }
            }

            return hasSameValue;
        }

        #endregion

    }
}
