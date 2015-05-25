using System;
using System.Windows.Forms;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Messages
{
    /// <summary>
    /// 
    /// </summary>
    class LabelFromLabel : ILabelProvider
    {
        private readonly Label aLabel;

        /// <summary>
        /// .ctor
        /// </summary>
        /// <param name="aLabel"></param>
        public LabelFromLabel(Label aLabel)
        {
            this.aLabel = aLabel;
        }

        /// <summary>
        /// Get display label
        /// </summary>
        /// <returns></returns>
        public string GetLabel()
        {
            return this.aLabel.Text;
        }
    }
}
