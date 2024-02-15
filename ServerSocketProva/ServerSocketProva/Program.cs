using System.Net;
using System.Net.Sockets;

namespace ServerSocketProva
{
    class ServerSocketProva
    {
        HttpClient client = new HttpClient();
        static int PORT = 12345;
        static private IPAddress address = IPAddress.Parse("127.0.0.1");

        // En cas de que hi hagi més de un client
        static List<ClientInfo> connectedClients = new List<ClientInfo>();

        static void Main()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 12345);
            listener.Start();
            Console.WriteLine($"Servidor escuchando en {address}:{PORT}");

            // Acceptem client continuament i els controlem amb tasks separades
            while (true)
            {
                TcpClient clientTcp = listener.AcceptTcpClient();
                Console.WriteLine("Connection established with the client");

                // Guardem la informacio a una llista (En cas de que ho fem amb varis usuaris tot i que també funciona amb un de sol)
                ClientInfo newClient = new ClientInfo(clientTcp);
                connectedClients.Add(newClient);

                // Handle the client in a separate task
                Task.Run(() => HandleClient(newClient));
            }
        }

        private static async Task HandleClient(ClientInfo client)
        {
            try
            {
                // Xarxa del client
                NetworkStream stream = client.TcpClient.GetStream();

                // Creem un readeer i writer per llegir i enviar missatges al client
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                // Bucle per llegir els missatges
                while (client.TcpClient.Connected)
                {
                    // Llegeix el missatge de la xarxa (això si el client només envia una línia tingeu en compte)
                    string message = await reader.ReadLineAsync();

                    if (message == "exit") // si envies exit desconectes el client
                    {
                        // Si arribes aquí significa que el client s'ha desconectat
                        Console.WriteLine($"Client {client.TcpClient.Client.RemoteEndPoint} esta out.");

                        // Realizar cualquier limpieza necesaria y quitar el cliente de la lista
                        connectedClients.Remove(client);
                        client.TcpClient.Close();
                    } else if (message != null)
                    {
                        // Mostra el missatge del client
                        Console.WriteLine($"Missatge del client: {message}");

                        // Mostra el missatge al client
                        await writer.WriteLineAsync($"El serveer ha rebut el missatge: {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al manejar cliente: {ex.Message}");
            }

        }

        public class ClientInfo
        {
            public TcpClient TcpClient { get; }

            public ClientInfo(TcpClient tcpClient)
            {
                TcpClient = tcpClient;
            }
        }
    }
}