package user;

import java.io.IOException;
import java.util.Scanner;

import com.chatBotServer.Greet.HelloReply;
import com.chatBotServer.Greet.HelloRequest;
import com.chatBotServer.GreeterGrpc;
import com.chatBotServer.GreeterGrpc.GreeterBlockingStub;
import com.chatbotclient.ChatGrpc.ChatBlockingStub;
import com.chatbotclient.ChatGrpc.ChatImplBase;
import com.chatbotclient.ChatOuterClass.ChatMessage;

import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;
import io.grpc.Status;
import io.grpc.stub.StreamObserver;
import server.UserService;

public class GRPCClient extends ChatImplBase{
	
	private ChatBlockingStub chatStub;

	public static void main(String[] args) throws IOException {
		ManagedChannel channel = ManagedChannelBuilder.forAddress("localhost", 9090).usePlaintext().build();
		
		//Generate Stubs from Protofile 
		GreeterBlockingStub stub = GreeterGrpc.newBlockingStub(channel);
		Scanner in = new Scanner(System.in); 
		System.out.println("Enter your Name:");
        String name = in.nextLine();
		HelloRequest request = HelloRequest.newBuilder().setName(name).build();
		HelloReply response = stub.sayHello(request);
		System.out.println("response from server " +response.getMessage());
		
	    StreamObserver<ChatMessage> requestObserver = new StreamObserver<ChatMessage>() {

			@Override
			public void onNext(ChatMessage value) {
				 System.out.println("Message sent to server"+ value.getMessage());
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
	    while(true) {
		    System.out.println("Enter message to the server:");
		    String msg = in.nextLine();
		    new UserService().chitChat(requestObserver).onNext(ChatMessage.newBuilder().setMessage(msg).build());
	}
	}
}
