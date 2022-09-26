package server;

import com.chatBotServer.Greet.HelloReply;
import com.chatBotServer.Greet.HelloRequest;
import com.chatBotServer.GreeterGrpc.GreeterImplBase;
import io.grpc.stub.StreamObserver;

public class GreetService extends GreeterImplBase{

	@Override
	public void sayHello(HelloRequest request, StreamObserver<HelloReply> responseObserver) {
		
		HelloReply response = HelloReply.newBuilder().setMessage(
				"Hello " + request.getName()).build();
		responseObserver.onNext(response);
		responseObserver.onCompleted();
		
	}
}
