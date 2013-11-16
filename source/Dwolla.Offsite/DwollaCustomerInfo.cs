using Dwolla.Validators;
using FluentValidation.Attributes;

namespace Dwolla
{
    [Validator(typeof(DwollaCustomerInfoValidator))]
    public class DwollaCustomerInfo
    {
        /// <summary>
        /// Customer's first name.
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Customer's last name.
        /// </summary>
        public string LastName { get; set; }
        /// <summary>
        /// Customer's email address.
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Customer's city.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Customer's state. Must be specified in 2-characters abbr.
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Customer's zip code.
        /// </summary>
        public string Zip { get; set; }
    }
}