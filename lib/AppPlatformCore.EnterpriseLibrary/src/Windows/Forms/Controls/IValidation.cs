namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Controls
{
    /// <summary>
    /// Check if control has correct values set
    /// </summary>
    public interface IValidation
    {
        /// <summary>
        /// Gets or sets a value indicating whether the control is obligatory.
        /// </summary>
        bool Obligatory { get; set;}

        /// <summary>
        /// Check if all obligatory values are set.
        /// </summary>
        /// <returns></returns>
        bool ObligatoryValuesSet(bool markIfFailed);

        /// <summary>
        /// Mark control if obligatory validation failed.
        /// </summary>
        void MarkObligatoryFailed();

        /// <summary>
        /// Identifier of the control. Used to address control (instead of Name).
        /// </summary>
        string ValidationID { get; set;}
    }
}
