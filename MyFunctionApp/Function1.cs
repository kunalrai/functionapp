using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MyFunctionApp
{
    public static class DurableFunction
    {
        [FunctionName("DurableFunction")]
        public static async Task Run(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            log.LogInformation("Starting Durable Function...");

            // Call the first activity function
            string result1 = await context.CallActivityAsync<string>("ActivityFunction1", "input1");

            // Call the second activity function
            string result2 = await context.CallActivityAsync<string>("ActivityFunction2", "input2");

            log.LogInformation($"Result1 = {result1}, Result2 = {result2}");
        }

        [FunctionName("ActivityFunction1")]
        public static string ActivityFunction1([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Running Activity Function 1...");
            
            return "Result from Activity Function 1";
        }

        [FunctionName("ActivityFunction2")]
        public static string ActivityFunction2([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Running Activity Function 2...");
            // Perform some long-running task here...
            return "Result from Activity Function 2";
        }

        [FunctionName("DurableFunction_HttpStart")]
        public static async Task<IActionResult> HttpStart(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string instanceId = await starter.StartNewAsync("DurableFunction", null);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
