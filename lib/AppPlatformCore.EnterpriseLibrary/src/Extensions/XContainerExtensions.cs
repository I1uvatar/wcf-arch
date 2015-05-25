using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AppPlatform.Core.EnterpriseLibrary.Extensions
{
    /// <summary>
    /// XConatinre extensions
    /// </summary>
    public static class XContainerExtensions
    {
        /// <summary>
        /// Finds the element.
        /// </summary>
        /// <param name="xContainer">XContainer.</param>
        /// <param name="elementName">Name of the single element to find.</param>
        /// <returns></returns>
        public static XElement FindSingleElement(this XContainer xContainer, string elementName)
        {
            return xContainer.FindElements(elementName).Single();
        }

        /// <summary>
        /// Finds the element.
        /// </summary>
        /// <param name="xContainer">XContainer.</param>
        /// <param name="elementName">Name of the single element to find.</param>
        /// <returns></returns>
        public static XElement FindSingleElementOrDefault(this XContainer xContainer, string elementName)
        {
            var elements = xContainer.FindElements(elementName);
            if (elements != null)
            {
                return xContainer.FindElements(elementName).Single();
            }
            else
            {
                return new XElement(elementName);
            }
        }

        /// <summary>
        /// Find XML elements.
        /// </summary>
        /// <param name="xContainer">The XContainer.</param>
        /// <param name="elementName">Name of the elements to find.</param>
        /// <returns>XElement list.</returns>
        public static IEnumerable<XElement> FindElements(this XContainer xContainer, string elementName)
        {
            var elements = xContainer.Elements();
            //var elementsLIst = elements.ToList();
            if (elements.Count() == 0)
            {
                return null;
            }

            var wantedElements = elements.Where(elem => elem.Name.LocalName == elementName);
            //var wantedElementsList = wantedElements.ToList();
            if (wantedElements.Count() > 0)
            {
                return wantedElements;
            }
            foreach (XElement element in elements)
            {
                wantedElements = element.FindElements(elementName);
                if (wantedElements != null)
                {
                    return wantedElements;
                }
            }
            return null;
        }

        ///// <summary>
        ///// Find XML elements.
        ///// </summary>
        ///// <param name="xContainer">The XContainer.</param>
        ///// <param name="elementName">Name of the elements to find.</param>
        ///// <param name="listaNodova"></param>
        ///// <returns>XElement list.</returns>
        //public static void FindAllSameElements(this XContainer xContainer, string elementName, ref List<XElement> listaNodova)
        //{
        //    var elements = xContainer.Elements();
        //    //var elementsLIst = elements.ToList();
        //    if (elements != null && elements.Count() != 0)
        //    {
        //        var wantedElements = elements.Where(elem =>
        //                                                {
        //                                                    return elem.Name.LocalName == elementName;
        //                                                });

        //        if (wantedElements != null && wantedElements.Count() > 0)
        //        {
        //            foreach (var xElement in wantedElements)
        //            {
        //                listaNodova.Add(xElement);
        //            }
        //        }
        //        foreach (XElement elem in elements)
        //        {
        //            elem.FindAllSameElements(elementName, ref listaNodova);
        //        }
        //    }
        //}

        /// <summary>
        /// Find XML elements.
        /// </summary>
        /// <param name="xContainer">The XContainer.</param>
        /// <param name="elementName">Name of the elements to find.</param>
        /// <returns>XElement list.</returns>
        public static IEnumerable<XElement> FindAllSameElements(this XContainer xContainer, string elementName)
        {
            List<XElement> result = new List<XElement>();

            var elements = xContainer.Elements();

            if (elements == null || elements.Count() == 0)
            {
                return null;
            }

            var wantedElements = elements.Where(elem =>
                                                    {
                                                        return elem.Name.LocalName == elementName;
                                                    });
            //var wantedElementsList = wantedElements.ToList();
            if (wantedElements != null && wantedElements.Count() > 0)
            {
                //return wantedElements;
                foreach (var element in wantedElements)
                {
                    result.Add(element);
                }
            }
            foreach (XElement element in elements)
            {
                wantedElements = element.FindAllSameElements(elementName);

                if (wantedElements != null)
                {
                    foreach (var element2 in wantedElements)
                    {
                        result.Add(element2);
                    }
                }
            }
            return result;
        }

    }
}