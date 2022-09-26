package server;

import io.grpc.ServerBuilder;

import java.io.IOException;

import io.grpc.Server;

public class GRPCServer {

	public static void main(String[] args) throws IOException, InterruptedException {
		Server server = ServerBuilder.forPort(9090).addService(new GreetService()).build();
	
		server.start();
		
		System.out.println("Server running at " + server.getPort());
		
		server.awaitTermination();
		
	}

}
