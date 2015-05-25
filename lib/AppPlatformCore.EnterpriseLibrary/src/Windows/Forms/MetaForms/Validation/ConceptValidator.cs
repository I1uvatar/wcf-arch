using System.Windows.Forms;

namespace AppPlatform.Core.EnterpriseLibrary.Windows.Forms.MetaForms.Validation
{
    public enum NumericRangeValidation
    { 
        Min, 
        Max, 
        Both = Min | Max 
    }

    public delegate string ValidateValueDelegate(Control control, Concept.Concept concept);

    public static class ConceptValidator
    {
        public static bool ValidateNumericRange(int number, int min, int max, NumericRangeValidation rangeType)
        {
            if ((rangeType & NumericRangeValidation.Min) == NumericRangeValidation.Min && number < min)
            {
                return false;
            }
            if ((rangeType & NumericRangeValidation.Max) == NumericRangeValidation.Max && number > max)
            {
                return false;
            }
            return true;
        }

        public static bool ValidateTextLength(int length, int min, int max, NumericRangeValidation rangeType)
        {
            if ((rangeType & NumericRangeValidation.Min) == NumericRangeValidation.Min && length < min)
            {
                return false;
            }
            if ((rangeType & NumericRangeValidation.Max) == NumericRangeValidation.Max && length > max)
            {
                return false;
            }
            return true;
        }

    }
}
