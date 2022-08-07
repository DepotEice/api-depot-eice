using System.ComponentModel.DataAnnotations;

namespace API.DepotEice.UIL.Models.Forms
{
    public class ModuleForm
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
