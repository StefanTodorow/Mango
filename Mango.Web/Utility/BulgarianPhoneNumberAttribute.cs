using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Mango.Web.Utility
{
    public class BulgarianPhoneNumberAttribute : ValidationAttribute
    {
        private static readonly Regex PhoneNumberRegex = new Regex(@"^(?:\+359|0)?\d{9}$");

        public override bool IsValid(object? value)
        {
            if (value is null || !(value is string))
            { //Required will handle null values
                return true;
            }

            string phoneNumber = (string)value;

            return PhoneNumberRegex.IsMatch(phoneNumber);
        }
    }
}
