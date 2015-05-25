using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace AppPlatform.Core.EnterpriseLibrary.Archive
{
    public static class GZip
    {
        public static byte[] GZipPack(string value)
        {
            var enc = Encoding.UTF8;
            return GZipPack(enc.GetBytes(value));

            //var byteArray = enc.GetBytes(value);


            //using (var ms = new MemoryStream())
            //{
            //    using (var sw = new GZipStream(ms, CompressionMode.Compress))
            //    {
            //        //Compress
            //        sw.Write(byteArray, 0, byteArray.Length);
            //        //Close, DO NOT FLUSH cause bytes will go missing...
            //        sw.Close();

            //        //Transform byte[] zip data to string
            //        byteArray = ms.ToArray();
            //        //var zip = enc.GetString(byteArray);


            //        ms.Close();
            //        sw.Dispose();
            //        ms.Dispose();
            //        return byteArray;
            //    }
            //}
        }

        public static byte[] GZipPack(byte[] value)
        {
            using (var ms = new MemoryStream())
            {
                using (var sw = new GZipStream(ms, CompressionMode.Compress))
                {
                    //Compress
                    sw.Write(value, 0, value.Length);
                    //Close, DO NOT FLUSH cause bytes will go missing...
                    sw.Close();

                    //Transform byte[] zip data to string
                    var zipPack = ms.ToArray();
                    //var zip = enc.GetString(byteArray);


                    ms.Close();
                    sw.Dispose();
                    ms.Dispose();
                    return zipPack;
                }
            }
        }

        public static string GZipUnpack(byte[] byteArray)
        {
            ////Transform string into byte[]
            var enc = Encoding.UTF8;
            //var byteArray = enc.GetBytes(value);

            using (var ms = new MemoryStream(byteArray))
            {
                using (var sr = new GZipStream(ms, CompressionMode.Decompress))
                {
                    //Prepare for decompress
                    var buffer = new Byte[4];
                    ms.Position = Convert.ToInt32(ms.Length) - 4;
                    ms.Read(buffer, 0, 4);
                    var originalFileSize = BitConverter.ToInt32(buffer, 0);
                    ms.Position = 0;



                    //Reset variable to collect uncompressed result
                    byteArray = new byte[originalFileSize];

                    //Decompress
                    sr.Read(byteArray, 0, originalFileSize);

                    //Transform byte[] unzip data to string
                    var rtf = enc.GetString(byteArray);

                    sr.Close();
                    ms.Close();
                    sr.Dispose();
                    ms.Dispose();
                    return rtf;
                }
            }
        }
    }
}