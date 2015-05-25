namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Controls
{
    /// <summary>
    /// Switch a data entry control to readonly or readwrite mode.
    /// </summary>
    public interface IInputControl
    {
        /// <summary>
        /// Sets control into read mode
        /// </summary>
        void SetReadOnly(bool isReadOnly);

        /// <summary>
        /// Gets or sets a value indicating whether the control is displayed
        /// </summary>
        void SetVisible(bool isVisible);

        /// <summary>
        /// Sets if control is enabled
        /// </summary>
        void SetEnabled(bool isEnabled);

        /// <summary>
        /// Clear the value of the control
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether [is read only].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is read only]; otherwise, <c>false</c>.
        /// </returns>
        bool IsReadOnly();
    }
}
