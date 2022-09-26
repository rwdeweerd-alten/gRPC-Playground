using ChatBot.gRPC;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBotServer.Services
{
    public class ChatService : ChatBot.gRPC.Chat.ChatBase
    {
        private readonly ILogger _logger;

        public ChatService(ILogger<ChatService> logger)
        {
            _logger = logger;
        }

        public override async Task ChitChat(IAsyncStreamReader<ChatMessage> requestStream, IServerStreamWriter<ChatMessage> responseStream, ServerCallContext context)
        {
            int messageCnt = 0;
            try
            {
                while (await requestStream.MoveNext())
                {
                    messageCnt++;
                    var message = requestStream.Current.Message;
                    _logger.LogInformation($"Received Chit: {message} Count: {messageCnt}");

                    for (int i = 0; i < 10; i++)
                    {
                        await Task.Delay(200);
                        var msg = $"Chat: {message} {messageCnt}.{i}";
                        await responseStream.WriteAsync(new ChatMessage() { Message = msg });

                        _logger.LogInformation($"Sent Chat: {msg}");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogInformation($"ChitChat call stopped: {e.Message}");
            }
        }
    }
}
