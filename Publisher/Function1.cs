using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Publisher
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string name = req.Query["name"];
            ISender sender;
            if(name == "1")
            {
                sender = new Sender();
            }
            else
            {
                sender = new MultiSender();
            }          

            Stopwatch stopwatch = new Stopwatch();
            var tasks = new Task[1000];
            stopwatch.Start();
            for (var i = 0; i < 1000; i++)
            {
                tasks[i] = Task.Factory.StartNew( ()=> sender.Send());
            }
            await Task.WhenAll(tasks);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);


            string responseMessage =$"Hello, Result is {stopwatch.ElapsedMilliseconds}";

            return new OkObjectResult(responseMessage);
        }
    }
}
