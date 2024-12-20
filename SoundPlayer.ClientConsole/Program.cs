using Grpc.Net.Client;

namespace SoundPlayer.ClientConsole
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            var request = new HelloRequest();

            request.Name = Console.ReadLine();

            using var channel = GrpcChannel.ForAddress("https://localhost:32773/");

            // создаем клиент
            var client = new AuthProto.AuthProtoClient(channel);

            var registerResponse = await client.HelloAsync(request);

            Console.WriteLine(registerResponse.Name);
        }
    }
}
