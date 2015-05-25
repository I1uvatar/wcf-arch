using System;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Messages
{
    /// <summary>
    /// Provides diplay labels for messages
    /// </summary>
    public interface ILabelProvider
    {
        /// <summary>
        /// Get display label
        /// </summary>
        /// <returns></returns>
        string GetLabel();
    }
}
