using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ServiceBusReceiver;
using System;
using System.Threading.Tasks;

namespace Receiver
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run([ServiceBusTrigger("integrationplatformtopic", "TestSub", Connection = "IntegrationPlatformConnection")] 
        ServiceBusReceivedMessage message, ILogger log)
        {
            log.LogInformation($"C# ServiceBus topic trigger function processed message: {message}");
            Console.WriteLine(message);
            var receiver = new ReceiverLogic();
            await receiver.Receive(message);
        }
    }
}
