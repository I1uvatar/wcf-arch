using System;

namespace AppPlatform.Core.EnterpriseLibrary.Patterns.Gof
{
    /// <summary>
    /// Defines interface for the GoF Command pattern.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Execute the command actions.
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// Defines "interface" for the GoF Command pattern.
    /// Execute the command actions.
    /// </summary>
    public delegate void Execute();
}
