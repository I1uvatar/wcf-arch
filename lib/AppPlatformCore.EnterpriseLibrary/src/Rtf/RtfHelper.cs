using System.Collections.Generic;

namespace HermesSoftLab.EnterpriseLibrary.Rtf
{
    public static class RtfHelper
    {
        #region Constants

        private const string UNICODE_CHARACTER_FORMAT = @"\u{0}?";

        #endregion

        #region Constructors

        static RtfHelper()
        {
            //characterMapper = new Dictionary<string, string>();
            InitilazeCharacterMapper();
        }

        #endregion

        #region Private members

        private static readonly Dictionary<string, string > characterMapper = new Dictionary<string, string>();

        #endregion

        #region Private methods

        private static void InitilazeCharacterMapper()
        {
            characterMapper.Add("č", string.Format(UNICODE_CHARACTER_FORMAT, 269));
            characterMapper.Add("Č", string.Format(UNICODE_CHARACTER_FORMAT, 268));
            characterMapper.Add("š", string.Format(UNICODE_CHARACTER_FORMAT, 353));
            characterMapper.Add("Š", string.Format(UNICODE_CHARACTER_FORMAT, 352));
        }

        #endregion

        #region Public methods

        public static void FormatCharacters(string text)
        {
            foreach (var key in characterMapper.Keys)
            {
                text.Replace(key, characterMapper[key]);
            }
            //return text;
        }

        #endregion
    }
}
