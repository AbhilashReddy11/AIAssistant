using System.ComponentModel.DataAnnotations;

namespace AIAssistant.Models
{
    public class CodeSubmission
    {
        [Required(ErrorMessage = "Please provide the code for review.")]
        public string Code { get; set; }
    }
}

