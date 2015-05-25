using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace AppPlatform.Core.EnterpriseLibrary.Encryption
{
    public static class Rot13
    {
        /// <summary>
        /// Performs the ROT13 character rotation.
        /// </summary>
        public static string Transform(string value)
        {
            DateTime startTime = DateTime.Now;
            //Debug.WriteLine(string.Format("Rot13 transform started: {0}", startTime));

            char CurrentCharacter;
            StringBuilder encodedTextBuilder = new StringBuilder(value.Length);

            //Iterate through the length of the input parameter  
            for (int i = 0; i < value.Length; i++)
            {
                //Convert the current character to a char  
                CurrentCharacter = value[i];

                //Modify the character code of the character, - this  
                //so that "a" becomes "n", "z" becomes "m", "N" becomes "Y" and so on  
                if (CurrentCharacter >= 97 && CurrentCharacter <= 109)
                {
                    CurrentCharacter = (char)(CurrentCharacter + 13);
                }
                else

                    if (CurrentCharacter >= 110 && CurrentCharacter <= 122)
                    {
                        CurrentCharacter = (char)(CurrentCharacter - 13);
                    }
                    else

                        if (CurrentCharacter >= 65 && CurrentCharacter <= 77)
                        {
                            CurrentCharacter = (char)(CurrentCharacter + 13);
                        }
                        else

                            if (CurrentCharacter >= 78 && CurrentCharacter <= 90)
                            {
                                CurrentCharacter = (char)(CurrentCharacter - 13);
                            }

                //Add the current character to the string to be returned  
                encodedTextBuilder.Append(CurrentCharacter);
            }

            //Debug.WriteLine(string.Format("Rot13 transform finished: {0}, Duration: {1}", startTime, DateTime.Now.Subtract(startTime)));
            return encodedTextBuilder.ToString(); 
        }

        /// <summary>
        /// Transforms content with Rot13 algorithm and writes it to file.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void TransformToFile(string content, string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(Transform(content));
            sw.Close();
            fs.Close();
        }

        public static string TransformFromFile(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string content = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            return Transform(content);
        }

        public static void TrasformFile(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string content = sr.ReadToEnd();
            sr.Close();
            fs.Close();
            TransformToFile(content, fileName);
        }
    }
}