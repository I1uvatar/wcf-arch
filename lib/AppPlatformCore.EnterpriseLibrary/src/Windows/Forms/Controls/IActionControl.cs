namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Controls
{
    public interface IActionControl
    {
        /// <summary>
        /// Sets if control is enabled
        /// </summary>
        void SetEnabled(bool isEnabled);

        /// <summary>
        /// Sets if control is visible
        /// </summary>
        void SetVisibility(bool isVisible);

        /// <summary>
        /// Set the state of the action control (visibility and enabled status)
        /// </summary>
        /// <param name="state"></param>
        void SetState(ActionControlStates state);

        /// <summary>
        /// SetsReadOnly mode for button
        /// </summary>
        void SetReadOnly(bool readOnly);
    }
}
