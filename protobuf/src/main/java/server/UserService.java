package server;

import com.chatbotclient.ChatGrpc.ChatImplBase;
import com.chatbotclient.ChatOuterClass.ChatMessage;
import com.google.api.SystemParameterOrBuilder;

import io.grpc.Status;
import io.grpc.stub.StreamObserver;

public class UserService extends ChatImplBase{

	@Override
	public StreamObserver<ChatMessage> chitChat(StreamObserver<ChatMessage> responseObserver) {
		return new StreamObserver<ChatMessage>() {

			@Override
			public void onNext(ChatMessage chatMsg) {
				System.out.println("Message received from the server:" +chatMsg.getMessage());
			}

			@Override
			public void onError(Throwable t) {
				Status status = Status.fromThrowable(t);
                System.err.println("Failed: {0}"+status);
			}

			@Override
			public void onCompleted() {
				 System.out.println("Server sent END");
			}
			
		};
	}
}
