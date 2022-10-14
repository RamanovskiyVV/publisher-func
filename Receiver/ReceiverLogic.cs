using Azure.Messaging.ServiceBus;
using ServiceBusShared;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceBusReceiver
{
    public class ReceiverLogic
    {
        public async Task Receive(ServiceBusReceivedMessage message)
        {
            await using ServiceBusClient serviceBusClient = new ServiceBusClient(Configuration.CONNECTION_STRING);
            // Message received
            ApplicationMessage applicationMessage = JsonSerializer.Deserialize<ApplicationMessage>(message.Body);

            // Process the message/Update the Output
            applicationMessage.Output = $"Hello {applicationMessage.Input}!.";

            // Sending the reply
            ServiceBusSender serviceBusSender = serviceBusClient.CreateSender(message.ReplyTo);
            var serviceBusMessage = new ServiceBusMessage(JsonSerializer.Serialize(applicationMessage));
            await serviceBusSender.SendMessageAsync(serviceBusMessage);
        }
    }
}
