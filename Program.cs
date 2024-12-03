namespace CedroExemplo
{
    using System;
    using System.Net.Sockets;
    using System.Text;

    class Program
    {
        static void Main(string[] args)
        {
            string server = "seu.servidor.com"; // Endereço do servidor
            int port = 81; // Porta definida na documentação
            Console.WriteLine("Bem-vindo ao cliente de streaming!");

            // Conexão inicial
            using (TcpClient client = new TcpClient(server, port))
            {
                NetworkStream stream = client.GetStream();
                Console.WriteLine("Conexão estabelecida.");

                // Login
                if (!Login(stream))
                {
                    Console.WriteLine("Login falhou. Encerrando...");
                    return;
                }

                Console.WriteLine("Escolha uma opção:");
                Console.WriteLine("1 - Subscribe (SQT)");
                Console.WriteLine("2 - Unsubscribe (USQ)");
                Console.WriteLine("3 - Get Player Names (GPN)");
                Console.WriteLine("4 - Sair");

                while (true)
                {
                    Console.Write("Opção: ");
                    string option = Console.ReadLine();

                    switch (option)
                    {
                        case "1":
                            Subscribe(stream);
                            break;
                        case "2":
                            Unsubscribe(stream);
                            break;
                        case "3":
                            GetPlayerNames(stream);
                            break;
                        case "4":
                            Quit(stream);
                            return;
                        default:
                            Console.WriteLine("Opção inválida.");
                            break;
                    }
                }
            }
        }

        static bool Login(NetworkStream stream)
        {
            Console.Write("Digite o username: ");
            string username = Console.ReadLine();
            Console.Write("Digite a senha: ");
            string password = Console.ReadLine();

            string loginCommand = $"{username}\n{password}\n";
            SendCommand(stream, loginCommand);

            string response = ReceiveResponse(stream);
            Console.WriteLine($"Resposta: {response}");
            return response.Contains("You are connected");
        }

        static void Subscribe(NetworkStream stream)
        {
            Console.Write("Digite o ativo para assinar (ex: PETR4): ");
            string ativo = Console.ReadLine();
            string command = $"sqt {ativo}\n";
            SendCommand(stream, command);

            string response = ReceiveResponse(stream);
            Console.WriteLine($"Resposta: {response}");
        }

        static void Unsubscribe(NetworkStream stream)
        {
            Console.Write("Digite o ativo para cancelar (ex: PETR4): ");
            string ativo = Console.ReadLine();
            string command = $"usq {ativo}\n";
            SendCommand(stream, command);

            string response = ReceiveResponse(stream);
            Console.WriteLine($"Resposta: {response}");
        }

        static void GetPlayerNames(NetworkStream stream)
        {
            Console.Write("Digite o nome do mercado (ex: BMF): ");
            string market = Console.ReadLine();
            string command = $"gpn {market}\n";
            SendCommand(stream, command);

            string response = ReceiveResponse(stream);
            Console.WriteLine($"Resposta: {response}");
        }

        static void Quit(NetworkStream stream)
        {
            SendCommand(stream, "quit\n");
            Console.WriteLine("Desconectado.");
        }

        static void SendCommand(NetworkStream stream, string command)
        {
            byte[] data = Encoding.ASCII.GetBytes(command);
            stream.Write(data, 0, data.Length);
        }

        static string ReceiveResponse(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.ASCII.GetString(buffer, 0, bytesRead);
        }
    }

}
