using API.DepotEice.UIL.Attributes;
using System.ComponentModel.DataAnnotations;

namespace API.DepotEice.UIL.Models.Forms
{
    public class ScheduleFileForm
    {
        [Display(Name = "File")]
        [Required(ErrorMessage = "Pick a File")]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".txt", ".pdf" }, ErrorMessage = "Your file's filetype is not valid.")]
        public IFormFile File { get; set; }
    }
}
