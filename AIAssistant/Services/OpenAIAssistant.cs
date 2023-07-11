using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using AIAssistant.Models;
using Newtonsoft.Json;

namespace AIAssistant.Services
{
    public class OpenAIAssistant
    {
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly string _modelName;

        public OpenAIAssistant(IConfiguration configuration)
        {
            _apiKey = configuration["OpenAI:ApiKey"];
            _apiUrl = configuration["OpenAI:ApiUrl"];
            _modelName = configuration["OpenAI:ModelName"];
        }
        public async Task<CodeReviewResult> PerformCodeReview(string code)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

                var requestBody = new
                {
                    messages = new[]
                    {
                new { role = "system", content = "You are a code reviewer." },
                new { role = "user", content = code }
            },
                    model = _modelName
                };

                var jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(_apiUrl, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);

                string assistantMessage = responseObject.choices[0].message.content;

                // Split the assistant message into separate points
                string[] points = assistantMessage.Split(" . ", StringSplitOptions.RemoveEmptyEntries);

                // Format the code snippet with proper indentation
                string formattedCode = $"\n```csharp\n{code}\n```\n";

                // Combine points with line breaks
                string formattedPoints = string.Join("\n\n", points.Select(p => p.Trim()));

                // Combine points, code snippet, and overall message
                string formattedOutput = $@"
The code appears to calculate the sum of numbers in the given array. Here are a few suggestions for improvement:

{formattedPoints}

Overall, the code is simple and straightforward, and it correctly calculates the sum of the numbers in the given array.

Code:
{formattedCode}";

                CodeReviewResult reviewResult = new CodeReviewResult
                {
                    Code = code,
                    ReviewText = formattedOutput,
                };

                return reviewResult;
            }
        }


    }
}
