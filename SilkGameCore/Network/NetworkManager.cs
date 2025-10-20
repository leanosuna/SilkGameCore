using Riptide;

namespace SilkGameCore.Network
{
    public class NetworkManager
    {
        SilkGameGL _game;
        Client _client;
        public NetworkManager(SilkGameGL game)
        {
            _game = game;
            _client = new Client();
            Console.WriteLine("Network enabled");
        }
        public void Connect()
        {
            var server = "192.168.1.40:7777";
            var res = _client.Connect(server);

            Console.WriteLine(res ? "connect ok" : "connect fail");

            _client.ConnectionFailed += Client_ConnectionFailed;
            _client.Connected += Client_Connected;
            _client.ClientDisconnected += Client_Disconnected;
        }

        public void Update()
        {
            _client.Update();
        }

        private void Client_Disconnected(object? sender, ClientDisconnectedEventArgs e)
        {
            Console.WriteLine("disconnected");
        }

        private void Client_Connected(object? sender, EventArgs e)
        {
            Console.WriteLine("connected");
        }

        private void Client_ConnectionFailed(object? sender, ConnectionFailedEventArgs e)
        {
            Console.WriteLine("con failed");
        }
    }
}
