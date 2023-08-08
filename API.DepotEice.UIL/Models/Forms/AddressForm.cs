using System.ComponentModel.DataAnnotations;

namespace API.DepotEice.UIL.Models.Forms
{
    /// <summary>
    /// The form to create the address
    /// </summary>
    public class AddressForm
    {
        /// <summary>
        /// The name of the street. Must be maximum 150 characters.
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The street name is required.")]
        [MaxLength(150, ErrorMessage = "The street name is too long. It should maximum be 150 characters.")]
        public string Street { get; set; } = string.Empty;

        /// <summary>
        /// The number of the house. This must be a number but can contain letters. Must be maximum 15 characters
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The house number is required.")]
        [MaxLength(15, ErrorMessage = "The house number is too long. It should maximum be 15 characters.")]
        public string HouseNumber { get; set; } = string.Empty;

        /// <summary>
        /// The appartment number. This can be a number or a letter. This value is not required. Must be maximum 15
        /// characters
        /// </summary>
        [MaxLength(15, ErrorMessage = "The appartment number is too long. It should maximum be 15 characters.")]
        public string? Appartment { get; set; } = string.Empty;

        /// <summary>
        /// The city name. Must be maximum 100 characters
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The city is required.")]
        [MaxLength(100, ErrorMessage = "The city is too long. It should maximum be 100 characters.")]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// The state name. This value is not required. If provided, must be maximum 100 characters
        /// </summary>
        [MaxLength(100, ErrorMessage = "The state name is too long. It should maximum be 100 characters")]
        public string? State { get; set; } = string.Empty;

        /// <summary>
        /// The postal code (zip code) of the city. Must be maximum 15 characters
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "The zip code is required.")]
        [MaxLength(15, ErrorMessage = "The zipcode is too long. It should maximum be 15 characters")]
        public string ZipCode { get; set; } = string.Empty;

        /// <summary>
        /// The country name. This value is not required. If provided, must be maximum 100 characters
        /// </summary>
        [MaxLength(100, ErrorMessage = "The country name is too long. It should maximum be 100 characters")]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Define if this address is primary
        /// </summary>
        public bool IsPrimary { get; set; }
    }
}
