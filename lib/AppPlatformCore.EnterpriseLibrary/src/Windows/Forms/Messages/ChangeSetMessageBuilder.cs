using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using AppPlatform.Core.EnterpriseLibrary.Comparision;
using AppPlatform.Core.EnterpriseLibrary.Linq.Expressions;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.Messages
{
    /// <summary>
    /// Class for processing the differences between two objects, based on the output of <see cref="DataComparer"/> class.
    /// </summary>
    public class ChangeSetMessageBuilder
    {
        private class FallbackProvider : ILabelProvider
        {
            private readonly string key;

            public FallbackProvider(string key)
            {
                this.key = key;
            }

            public string GetLabel()
            {
                return this.key;
            }
        }
        private readonly Dictionary<string, ILabelProvider> providers = new Dictionary<string, ILabelProvider>();

        /// <summary>
        /// Mapping between a property and corresponding label provider
        /// </summary>
        /// <typeparam name="Source"></typeparam>
        /// <param name="getter"></param>
        /// <param name="aProvider"></param>
        /// <returns></returns>
        public ChangeSetMessageBuilder Configure<Source>(Expression<Func<Source, object>> getter, ILabelProvider aProvider)
        {
            var key = ExpressionUtility.GetPropertyPath(getter);
            this.providers[key] = aProvider;

            return this;
        }

        /// <summary>
        /// Mapping between a property and corresponding label
        /// </summary>
        /// <typeparam name="Source"></typeparam>
        /// <param name="getter"></param>
        /// <param name="aLabel"></param>
        /// <returns></returns>
        public ChangeSetMessageBuilder Configure<Source>(Expression<Func<Source, object>> getter, Label aLabel)
        {
            return this.Configure(getter, new LabelFromLabel(aLabel));
        }

        /// <summary>
        /// Prepares a message out from the differences
        /// </summary>
        /// <param name="differences"></param>
        /// <param name="diffMessageTemplate"></param>
        /// <param name="configuredOnly">Displays only messages for which provider is configured.</param>
        /// <returns></returns>
        public string PrepareMessage(XDocument differences, string diffMessageTemplate, bool configuredOnly)
        {
            return this.PrepareMessage(differences, (lbl, old, neww) => String.Format(diffMessageTemplate, lbl, old, neww), configuredOnly);
        }

        /// <summary>
        /// Prepares a message out from the differences
        /// </summary>
        /// <param name="differences"></param>
        /// <param name="formatMessage"></param>
        /// <param name="configuredOnly">Displays only messages for which provider is configured.</param>
        /// <returns></returns>
        public string PrepareMessage(XDocument differences, Func<string, string, string, string> formatMessage, bool configuredOnly)
        {
            var list = DataComparer.GetListOfChanges(differences);

            StringBuilder message = new StringBuilder();
            foreach (var pair in list)
            {
                var aProvider = this.GetProvider(pair.Key, !configuredOnly);

                if (aProvider != null && AtLeastOneRealValue(pair.Value))
                {
                    var newMessageLine = formatMessage(aProvider.GetLabel(), pair.Value.Old, pair.Value.New);
                    message.AppendLine(newMessageLine);
                }
            }

            return message.ToString();
        }

        private ILabelProvider GetProvider(string key, bool useFallbackProvider)
        {
            return this.providers.ContainsKey(key) ? this.providers[key] : (useFallbackProvider ? new FallbackProvider(key) : null);
        }

        private bool AtLeastOneRealValue(OldNewValuePair pairValue)
        {
            return !String.IsNullOrEmpty(pairValue.Old.Trim()) || !String.IsNullOrEmpty(pairValue.New.Trim());
        }
    }
}
