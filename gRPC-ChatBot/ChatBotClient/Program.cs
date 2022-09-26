using Grpc.Net.Client;
using System;
using ChatBotServer;    // This isn't a reference to the server, but only the namespace used in greet.proto
using ChatBot.gRPC;
using System.Threading.Tasks;
using System.Threading;
using Grpc.Core;

namespace ChatBotClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");

            var greeterClient = new Greeter.GreeterClient(channel);
            var chatClient = new Chat.ChatClient(channel);

            //await StartGreeting(greeterClient);
            await StartChatting(chatClient);
        }

        private static async Task StartGreeting(Greeter.GreeterClient client)
        {
            Console.WriteLine("Start Greeting. 'x' to return");

            var msg = Console.ReadLine();
            while (!msg.Equals("x"))
            {
                var reply = await client.SayHelloAsync(new HelloRequest { Name = "ChatBotClient" });
                Console.WriteLine($"Received Greeting: {reply.Message}");

                msg = Console.ReadLine();
            }
        }

        private static async Task StartChatting(Chat.ChatClient client)
        {
            using var call = client.ChitChat();
            using var cancellationToken = new CancellationTokenSource();

            // Start Async task to get response messages from ChatBot
            var responseTask =
                Task.Run(
                    async () =>
                    {
                        try
                        {
                            while (await call.ResponseStream.MoveNext(cancellationToken.Token))
                            {
                                Console.WriteLine($"Received: {call.ResponseStream.Current.Message}");
                            }
                        }
                        catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
                        {
                            Console.WriteLine($"Cancelled message receival");
                        }
                    });

            // Start chatting to server
            Console.WriteLine("Start Chatting. 'x' to return immediately. 'c' to wait for responses.");
            var msg = Console.ReadLine();
            while (!msg.Equals("x") && !msg.Equals("c"))
            {
                await call.RequestStream.WriteAsync(new ChatMessage() { Message = msg });
                msg = Console.ReadLine();
            }

            if (msg.Equals("c"))
            {
                Console.WriteLine("Cancel pending server responses ...");
                cancellationToken.Cancel();
            }

            // Indicate we're done writing
            Console.WriteLine("Complete RequestStream");
            await call.RequestStream.CompleteAsync();

            // Wait for alle respones
            Console.WriteLine("Wait for Responses");
            await responseTask;

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
