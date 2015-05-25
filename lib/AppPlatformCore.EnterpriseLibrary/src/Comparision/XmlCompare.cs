//using System.IO;
//using System.Xml;
//using System.Xml.Linq;
//using Microsoft.XmlDiffPatch;

//namespace AppPlatform.Core.EnterpriseLibrary.Comparision
//{
//    public class XmlCompare
//    {
//        //public static bool HasSameData(object oldData, object newData)
//        //{
//        //    if (oldData == null)
//        //    {
//        //        return newData == null;
//        //    }
            
//        //    if (newData != null)
//        //    {
//        //        return false;
//        //    }

//        //    if (oldData.Equals(newData))
//        //    {
//        //        return true;
//        //    }

//        //    var type = oldData.GetType();

//        //    //if (newData)

//        //    //var comparer = new XmlDiff(GetComparisionOption());

//        //    //return comparer.Compare(
//        //    //    XmlReader.Create(new StringReader(oldData.OuterXml)),
//        //    //    XmlReader.Create(new StringReader(newData.OuterXml))
//        //    //    );
//        //}

//        public static bool HasSameData(XmlNode oldData, XmlNode newData)
//        {
//            var comparer = new XmlDiff(GetComparisionOption());

//            return comparer.Compare(
//                XmlReader.Create(new StringReader(oldData.OuterXml)),
//                XmlReader.Create(new StringReader(newData.OuterXml))
//                );
//        }

//        public static bool HasSameData(XNode oldData, XNode newData)
//        {
//            var comparer = new XmlDiff();

//            return comparer.Compare(XmlReader.Create(new StringReader(oldData.ToString())),
//                XmlReader.Create(new StringReader(newData.ToString())));
//        }

//        private static XmlDiffOptions GetComparisionOption()
//        {
//            return XmlDiffOptions.IgnorePI
//                   | XmlDiffOptions.IgnoreChildOrder
//                   | XmlDiffOptions.IgnoreComments
//                   | XmlDiffOptions.IgnoreDtd
//                   | XmlDiffOptions.IgnoreNamespaces
//                   | XmlDiffOptions.IgnorePrefixes
//                   | XmlDiffOptions.IgnoreWhitespace
//                   | XmlDiffOptions.IgnoreXmlDecl;
//        }
//    }
//}
