using System.Net.Sockets;

namespace ServerSocketProva
{
    class ClientSocketProva
    {
        static async Task Main()
        {
            TcpClient client = new TcpClient();
            string address = "127.0.0.1";
            int port = 12345;

            try
            {
                // Concectar al servidor al a la adressa i port especifics
                await client.ConnectAsync(address, port);
                Console.WriteLine("Conectat al servidor");

                // Obtenim el stream de la xarxa
                NetworkStream stream = client.GetStream();
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

                // Basicament un bucle per enviar missatges
                while (true)
                {
                    Console.Write("Di-li que vols enviar: ");
                    string message = Console.ReadLine();

                    // Enviar el missatge al servidor
                    await writer.WriteLineAsync(message);

                    // Llegir la resposta del servidor (En cas de que retorni si no borreu això)
                    string response = await reader.ReadLineAsync();
                    Console.WriteLine($"Resposta del servidor: {response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en el cliente: {ex.Message}");
            }
            finally
            {
                client.Close();
            }


        }
    }
}