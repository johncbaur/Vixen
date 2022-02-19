using System.Globalization;
using System.Windows.Controls;


namespace VixenModules.Editor.FixturePropertyEditor.ViewModels
{
    /// <summary>
    /// Validates the X coordinates in the polygon editor point data grid.
    /// </summary>
    public class DMXValidationRule : ValidationRule
    {		
        #region Public Methods

        /// <summary>
        /// Performs validation checks on a value.
        /// </summary>
        /// <param name="value">The value from the binding target to check.</param>
        /// <param name="cultureInfo">The cuture to use in this rule.</param>
        /// <returns>Validation results</returns>
        public override ValidationResult Validate(
            object value,
            CultureInfo cultureInfo)
        {
            // Default the result to Valid
            ValidationResult result = ValidationResult.ValidResult;

            // Attempt to parse the coordinate into an integer
            int intValue = 0;
            bool valid = int.TryParse((string)value, out intValue);

            // If the string did not parse into a double then...
            if (!valid)
            {
                // Indicate the value is invalid
                result = new ValidationResult(false, "Not a valid value.  Value values are 0-255.");
            }
            // Otherwise check to see if the value is within range
            else if (intValue < 0 || intValue > 255)
            {
                // Indicate the value is invalid
                result = new ValidationResult(false, "Not a valid value.  Value values are 0-255.");
            }
           
            // Return the results of the validation
            return result;            
        }

        #endregion      
    }
}
