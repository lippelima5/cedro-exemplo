using System;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace CedroExemplo
{
    class Program
    {
        private static TcpClient client;
        private static NetworkStream stream;
        private static StreamReader reader;
        private static StreamWriter writer;
        private static CancellationTokenSource cts;

        static async Task Main(string[] args)
        {
            // Carregar configurações do App.config
            var appSettings = ConfigurationManager.AppSettings;
            string host = appSettings["host"];
            int port = Convert.ToInt32(appSettings["port"]);
            string username = appSettings["username"];
            string password = appSettings["password"];

            try
            {
                // Inicializar conexão
                InitializeConnection(host, port);

                // Iniciar escuta de dados do servidor
                cts = new CancellationTokenSource();
                Task.Run(() => ListenToServer(cts.Token));

                // Realizar login
                await PerformLogin(username, password);

                // Exibir menu de ações
                await DisplayMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Erro] {ex.Message}");
            }
            finally
            {
                CleanUp();
            }
        }

        /// <summary>
        /// Inicializa a conexão Telnet com o servidor.
        /// </summary>
        /// <param name="host">Endereço do servidor.</param>
        /// <param name="port">Porta do servidor.</param>
        private static void InitializeConnection(string host, int port)
        {
            Console.WriteLine("Conectando ao servidor...");
            client = new TcpClient();
            client.Connect(host, port);
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };
            Console.WriteLine("Conexão estabelecida.");
        }

        /// <summary>
        /// Realiza o processo de login, lidando com respostas assíncronas do servidor.
        /// </summary>
        /// <param name="username">Nome de usuário.</param>
        /// <param name="password">Senha do usuário.</param>
        private static async Task PerformLogin(string username, string password)
        {
            Console.WriteLine("Realizando login...");
            await WriteCommand(""); // Software key (ou Enter)

            while (true)
            {
                string response = await ReadResponseAsync();
                if (response.Contains("Username:"))
                {
                    await WriteCommand(username);
                }
                else if (response.Contains("Password:"))
                {
                    await WriteCommand(password);
                }
                else if (response.Contains("You are connected"))
                {
                    Console.WriteLine("[Sucesso] Login realizado!");
                    break;
                }
                else
                {
                    Console.WriteLine($"[Servidor] {response}");
                }
            }
        }

        /// <summary>
        /// Exibe o menu de opções e processa as escolhas do usuário.
        /// </summary>
        private static async Task DisplayMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Menu de Opções ---");
                Console.WriteLine("1. Assinar ativo (Subscribe)");
                Console.WriteLine("2. Cancelar assinatura (Unsubscribe)");
                Console.WriteLine("3. Obter lista de players (Get Players)");
                Console.WriteLine("4. Sair");
                Console.Write("Escolha uma opção: ");

                string opcao = Console.ReadLine();

                switch (opcao)
                {
                    case "1":
                        Console.Write("Digite o ativo para assinar: ");
                        string ativoSub = Console.ReadLine();
                        await Subscribe(ativoSub);
                        break;

                    case "2":
                        Console.Write("Digite o ativo para cancelar assinatura: ");
                        string ativoUnsub = Console.ReadLine();
                        await Unsubscribe(ativoUnsub);
                        break;

                    case "3":
                        await GetPlayers();
                        break;

                    case "4":
                        Console.WriteLine("Encerrando...");
                        Quit();
                        return;

                    default:
                        Console.WriteLine("[Erro] Opção inválida. Tente novamente.");
                        break;
                }
            }
        }

        /// <summary>
        /// Assina o fluxo de dados para um ativo específico.
        /// </summary>
        /// <param name="ativo">Ativo a ser assinado.</param>
        private static async Task Subscribe(string ativo)
        {
            await WriteCommand($"sqt {ativo}");
            Console.WriteLine($"[Info] Assinatura solicitada para o ativo: {ativo}");
        }

        /// <summary>
        /// Cancela a assinatura de um ativo.
        /// </summary>
        /// <param name="ativo">Ativo a ter a assinatura cancelada.</param>
        private static async Task Unsubscribe(string ativo)
        {
            await WriteCommand($"usq {ativo}");
            Console.WriteLine($"[Info] Cancelamento solicitado para o ativo: {ativo}");
        }

        /// <summary>
        /// Solicita a lista de players ao servidor.
        /// </summary>
        private static async Task GetPlayers()
        {
            await WriteCommand("gpn bmf");
            Console.WriteLine("[Info] Comando 'gpn bmf' enviado para obter lista de players.");
        }

        /// <summary>
        /// Envia um comando para o servidor.
        /// </summary>
        /// <param name="command">Comando a ser enviado.</param>
        private static async Task WriteCommand(string command)
        {
            if (writer == null)
                throw new InvalidOperationException("[Erro] Stream de escrita não inicializado.");

            await writer.WriteLineAsync(command);
            Console.WriteLine($"[Comando] {command}");
        }

        /// <summary>
        /// Lê uma resposta do servidor.
        /// </summary>
        /// <returns>Resposta recebida.</returns>
        private static async Task<string> ReadResponseAsync()
        {
            if (reader == null)
                throw new InvalidOperationException("[Erro] Stream de leitura não inicializado.");

            string response = await reader.ReadLineAsync();
            return response?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Escuta continuamente as mensagens do servidor.
        /// </summary>
        /// <param name="token">Token de cancelamento.</param>
        private static async Task ListenToServer(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (stream.DataAvailable)
                    {
                        string data = await ReadResponseAsync();
                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            Console.WriteLine($"[Servidor] {data}");
                        }
                    }
                    await Task.Delay(100); // Reduz uso excessivo de CPU
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Erro] Falha na escuta do servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Envia o comando para encerrar a conexão.
        /// </summary>
        private static void Quit()
        {
            WriteCommand("quit").Wait();
            Console.WriteLine("[Info] Conexão encerrada.");
        }

        /// <summary>
        /// Libera todos os recursos utilizados.
        /// </summary>
        private static void CleanUp()
        {
            cts?.Cancel();
            reader?.Close();
            writer?.Close();
            stream?.Close();
            client?.Close();
            Console.WriteLine("[Info] Recursos liberados.");
        }
    }
}
