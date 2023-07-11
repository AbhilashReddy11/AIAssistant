
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using AIAssistant.Models;
    using AIAssistant.Services;

    namespace AIAssistant.Controllers
    {
        public class CodeReviewController : Controller
        {
            private readonly OpenAIAssistant _openAIAssistant;

            public CodeReviewController(OpenAIAssistant openAIAssistant)
            {
                _openAIAssistant = openAIAssistant;
            }

            public IActionResult Index()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> SubmitCode(CodeSubmission codeSubmission)
            {
                if (ModelState.IsValid)
                {
                    // Perform code analysis using OpenAIAssistant service
                    CodeReviewResult reviewResult = await _openAIAssistant.PerformCodeReview(codeSubmission.Code);

                    return View("Index",reviewResult);
                }

                return View("Index");
            }
        }
    }

