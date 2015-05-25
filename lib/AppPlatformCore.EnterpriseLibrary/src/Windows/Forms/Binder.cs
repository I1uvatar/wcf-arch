using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Forms;
using AppPlatform.Core.EnterpriseLibrary.Linq.Expressions;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms
{
    /// <summary>
    /// Helper class for control binding.
    /// </summary>
    /// <typeparam name="Source"></typeparam>
    public class Binder<Source>
    {
        private readonly object dataSource;
        private bool isList = false;

        public Binder(object dataSource)
        {
            this.dataSource = dataSource;
        }

        public Binder(List<Source> dataSource)
        {
            this.dataSource = dataSource;
            this.isList = true;
        }

        #region TextBox Bind methods

        /// <summary>
        /// Textbox binding method (binds to "Text property", DataSourceUpdateMode = OnValidation).
        /// </summary>
        /// <param name="textBox">The textbox.</param>
        /// <param name="getter">The data member expression.</param>
        /// <returns>Binder</returns>
        public Binder<Source> Bind(TextBox textBox, Expression<Func<Source, object>> getter)
        {
            return this.Bind(textBox, getter, "Text", "");
        }

        /// <summary>
        /// Textbox binding method with formating (binds to "Text property", DataSourceUpdateMode = OnValidation).
        /// </summary>
        /// <param name="textBox">The textbox.</param>
        /// <param name="getter">The data member expression.</param>
        /// <param name="format">Value presentation format.</param>
        /// <returns>Binder</returns>
        public Binder<Source> Bind(TextBox textBox, Expression<Func<Source, object>> getter, string format)
        {
            return this.Bind(textBox, getter, "Text", format);
        }

        /// <summary>
        /// Textbox binding method.
        /// </summary>
        /// <param name="textBox">The textbox.</param>
        /// <param name="getter">The data member expression.</param>
        /// <param name="updateMode">The binding update mode.</param>
        /// <returns>Binder</returns>
        public Binder<Source> Bind(TextBox textBox, Expression<Func<Source, object>> getter, DataSourceUpdateMode updateMode)
        {
            return this.Bind(textBox, getter, "Text", "", updateMode);
        }

        /// <summary>
        /// Textbox binding method with formating.
        /// </summary>
        /// <param name="textBox">The textbox.</param>
        /// <param name="getter">The data member expression.</param>
        /// <param name="format">Value presentation format.</param>
        /// <param name="updateMode">The binding update mode.</param>
        /// <returns>Binder</returns>
        public Binder<Source> Bind(TextBox textBox, Expression<Func<Source, object>> getter, string format, DataSourceUpdateMode updateMode)
        {
            return this.Bind(textBox, getter, "Text", format, updateMode);
        }

        /// <summary>
        /// Textbox binding method (binds to "Text property", DataSourceUpdateMode = OnValidation).
        /// </summary>
        /// <param name="textBox">The textbox.</param>
        /// <param name="attribute">Attribute expression (for getting a row from a list)</param>
        /// <param name="getter">The data member expression.</param>
        /// <returns>Binder</returns>
        public Binder<Source> Bind(TextBox textBox, Predicate<Source> attribute, Expression<Func<Source, object>> getter)
        {
            return this.Bind(textBox, attribute, getter, "Text", "");
        }

        #endregion

        #region base Bind methods - TODO: make methods protected (when specific Bind methods for all controls are implemented)

        /// <summary>
        /// Input control binding method.
        /// </summary>
        /// <param name="inputControl">The input control.</param>
        /// <param name="getter">The data member expression.</param>
        /// <param name="propertyName">The control property to bind to.</param>
        /// <param name="format">Value presentation format.</param>
        /// <returns>Binder</returns>
        public Binder<Source> Bind(Control inputControl, Expression<Func<Source, object>> getter, string propertyName, string format)
        {
            return this.Bind(inputControl, getter, propertyName, format, DataSourceUpdateMode.OnValidation);
        }

        /// <summary>
        /// Input control binding method.
        /// </summary>
        /// <param name="inputControl">The input control.</param>
        /// <param name="getter">The data member expression.</param>
        /// <param name="propertyName">The control property to bind to.</param>
        /// <param name="format">Value presentation format.</param>
        /// <param name="updateMode">Update mode.</param>
        /// <returns>Binder</returns>
        public Binder<Source> Bind(Control inputControl, Expression<Func<Source, object>> getter, string propertyName, string format,
            DataSourceUpdateMode updateMode)
        {
            string dataMember = ExpressionUtility.GetPropertyPath(getter);
            // method GetPropertyName doesn't work if properly is not directly on class (binding of p.Property.PropertyOfPropeprty doesn't work) 
            //string dataMember = ExpressionUtility.GetPropertyName(getter);

            inputControl.DataBindings.Clear();
            inputControl.DataBindings.Add(
                propertyName, this.dataSource, dataMember,
                true, // NOTE - Formatting must be enabled otherwise nullable types cannot be modified - strange MS bug
                updateMode,
                string.Empty,
                format
                );

            return Instance;
        }


        /// <summary>
        /// Input control binding method.
        /// </summary>
        /// <param name="inputControl">The input control.</param>
        /// <param name="attribute">Attribute expression (for getting a row from a list)</param>
        /// <param name="getter">The data member expression.</param>
        /// <param name="propertyName">The control property to bind to.</param>
        /// <param name="format">Value presentation format.</param>
        /// <returns>Binder</returns>
        public Binder<Source> Bind(Control inputControl, Predicate<Source> attribute, Expression<Func<Source, object>> getter, string propertyName, string format)
        {
            return this.Bind(inputControl, attribute, getter, propertyName, format, DataSourceUpdateMode.OnValidation);
        }

        /// <summary>
        /// Input control binding method.
        /// </summary>
        /// <param name="inputControl">The input control.</param>
        /// <param name="attribute">Attribute expression (for getting a row from a list)</param>
        /// <param name="getter">The data member expression.</param>
        /// <param name="propertyName">The control property to bind to.</param>
        /// <param name="format">Value presentation format.</param>
        /// <param name="updateMode">Update mode.</param>
        /// <returns>Binder</returns>
        public Binder<Source> Bind(Control inputControl, Predicate<Source> attribute, Expression<Func<Source, object>> getter, string propertyName, string format,
            DataSourceUpdateMode updateMode)
        {
            string dataMember = ExpressionUtility.GetPropertyPath(getter);

            if (!this.isList) return Instance;
            inputControl.DataBindings.Clear();
            inputControl.DataBindings.Add(
                propertyName, ((List<Source>)this.dataSource).Find(attribute), dataMember,
                true, // NOTE - Formatting must be enabled otherwise nullable types cannot be modified - strange MS bug
                updateMode,
                string.Empty,
                format
                );

            return Instance;
        }

        protected virtual Binder<Source> Instance
        {
            get { return this; }
        }

        #endregion

    }
}
