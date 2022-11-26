using System.ComponentModel.DataAnnotations;

namespace API.DepotEice.UIL.Models.Forms
{
    public class ModuleForm
    {
        /// <summary>
        /// Represents the required input property of the name in module.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Represents the requried input property of the description in module.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        public string? TeacherId { get; set; } = string.Empty;
    }
}
