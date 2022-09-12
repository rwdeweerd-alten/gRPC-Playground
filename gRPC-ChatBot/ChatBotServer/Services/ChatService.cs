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
            while (await requestStream.MoveNext())
            {
                var message = requestStream.Current.Message;
                _logger.LogInformation($"Received ChitChat message: {message}");

                await responseStream.WriteAsync(
                    new ChatMessage() { Message = $"Send response: {message}" });
            }
        }
    }
}
