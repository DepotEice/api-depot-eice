using System.ComponentModel.DataAnnotations;

namespace API.DepotEice.UIL.Attributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            var extension = Path.GetExtension(file.FileName);
            if (file != null)
            {
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            return ValidationResult.Success;
        }

        public override bool IsValid(object value)
        {
            if (value is null)
                return true;

            var file = value as IFormFile;
            string? extension = Path.GetExtension(file?.FileName);

            if (!_extensions.Contains(extension?.ToLower()))
                return false;

            return true;
        }
    }
}
