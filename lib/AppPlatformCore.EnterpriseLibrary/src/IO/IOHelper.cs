using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace AppPlatform.Core.EnterpriseLibrary.IO
{
    public class IOHelper
    {
        private static IOHelper instance = new IOHelper();

        public static IOHelper Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Creates or overwrites a file in the specified path
        /// </summary>
        /// <param name="path">The path and name of the file to create</param>
        public void CreateFile(string path)
        {
            var fs = File.Create(path);
            fs.Close();
        }

        /// <summary>
        /// Writes a new file to a specified path
        /// </summary>
        /// <param name="stream">file content</param>
        /// <param name="fileName">new file name</param>
        /// <returns>created file full path</returns>
        public void WriteStreamToFilename(byte[] stream, string fileName)
        {
            FileStream newFile = new FileStream(fileName, FileMode.Create);

            newFile.Write(stream, 0, stream.Length);
            newFile.Close();
        }

        /// <summary>
        /// Reads a file from a specified path
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <returns>byte array stream</returns>
        public byte[] ReadFile(string fileName)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }

        /// <summary>
        /// Save binary object to specified path
        /// </summary>
        /// <param name="objectToSave"></param>
        /// <param name="path"></param>
        public void SaveObjectBinary(object objectToSave, string path)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            byte[] data;
            formatter.Serialize(stream, objectToSave);
            data = stream.ToArray();

            this.CheckPath(path, true);

            this.WriteStreamToFilename(data, path);
        }

        /// <summary>
        /// Load binary object from specified path
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadObjectBinary<T>(string path)
        {
            try
            {
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                BinaryFormatter formatter = new BinaryFormatter();
                T result = (T)formatter.Deserialize(stream);
                stream.Close();
                return result;
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Checks if specified folder exist, optionally creates it
        /// </summary>
        /// <param name="path"></param>
        /// <param name="createIsNotExists"></param>
        /// <returns></returns>
        public bool CheckPath(string path, bool createIsNotExists)
        {
            DirectoryInfo info = new DirectoryInfo(Path.GetDirectoryName(path));
            if (info.Exists)
            {
                return true;
            }

            if (createIsNotExists)
            {
                info.Create();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Return date of the last modification to the specified file, if file doesn't exists, return null
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public DateTime? GetFileModificationDate(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            if (fileInfo.Exists)
            {
                return fileInfo.LastWriteTime;
            }

            return null;
        }

        /// <summary>
        /// Return time of the creation of the specified file, if file doesn't exists, return null
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public DateTime? GetFileCreationTime(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            if (fileInfo.Exists)
            {
                return fileInfo.CreationTime;
            }
            return null;
        }

        /// <summary>
        /// Sets the creation time of the specified file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public void SetFileCreationTime(string filename, DateTime creationTime)
        {
            FileInfo fileInfo = new FileInfo(filename);
            if (fileInfo.Exists)
            {
                try
                {
                    fileInfo.CreationTime = creationTime;
                }
                catch { }
            }
        }

        /// <summary>
        /// Return length of the specified file, if file doesn't exists, return null
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public long? GetFileLength(string filename)
        {
            FileInfo fileInfo = new FileInfo(filename);
            if (fileInfo.Exists)
            {
                return fileInfo.Length;
            }
            return null;
        }

        /// <summary>
        /// If <para>folderRawPath</para> is absolut, return it. If not, try if is is special folder, or return current folder + folderRawPath
        /// </summary>
        /// <param name="folderRawPath"></param>
        /// <returns></returns>
        public string GetFolderPath(string folderRawPath)
        {
            if (string.IsNullOrEmpty(Path.GetPathRoot(folderRawPath)))
            {
                return this.GetSpecialFolderPathOrDefault(folderRawPath);
            }

            return folderRawPath;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public string GetSpecialFolderPathOrDefault(string folderPath)
        {
            try
            {
                return this.GetSpecialFolderPath(folderPath);
            }
            catch (Exception)
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), folderPath ?? string.Empty);
            }
        }

        private string GetSpecialFolderPath(string folderPath)
        {
            Environment.SpecialFolder specialFolder = (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), folderPath);
            return Environment.GetFolderPath(specialFolder);
        }
    }
}