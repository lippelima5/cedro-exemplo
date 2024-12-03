# Exemplo de Conexão com a API Cedro Crystal via Telnet

Este repositório contém um exemplo funcional em **C#** que demonstra como conectar-se à API Cedro Crystal utilizando o protocolo **Telnet**. Ele inclui funcionalidades básicas como login, assinatura de ativos, cancelamento de assinaturas e obtenção de dados em tempo real.

## 🚀 Funcionalidades

1. **Conexão via Telnet**:
   - Estabelece uma conexão TCP/IP com o servidor da API Cedro Crystal.
   - Lida com mensagens de login e autenticação.

2. **Streaming de Dados em Tempo Real**:
   - Exibe continuamente os dados recebidos do servidor após a assinatura de um ativo.

3. **Menu Interativo**:
   - Permite que o usuário selecione entre as opções:
     - Assinar um ativo (`Subscribe`).
     - Cancelar a assinatura de um ativo (`Unsubscribe`).
     - Obter a lista de players (`Get Players`).

4. **Gerenciamento de Conexão**:
   - Liberação de recursos (streams e conexões) ao encerrar o programa.

## 🛠️ Configuração

Antes de executar o código, você deve configurar as informações de conexão no arquivo **App.config**:

```xml
<configuration>
  <appSettings>
    <add key="host" value="datafeed.seu-endereco.com" />
    <add key="port" value="81" />
    <add key="username" value="seu_usuario" />
    <add key="password" value="sua_senha" />
  </appSettings>
</configuration>
```

- **host**: Endereço do servidor Telnet da Cedro Crystal.
- **port**: Porta utilizada para a conexão (normalmente 81).
- **username**: Nome de usuário fornecido pela Cedro.
- **password**: Senha fornecida pela Cedro.

## 💻 Como Usar

1. Clone este repositório:
   ```bash
   git clone https://github.com/seu-usuario/cedro-telnet-exemplo.git
   ```

2. Abra o projeto em um editor C# ou no Visual Studio.

3. Atualize o arquivo `App.config` com suas credenciais de acesso.

4. Compile e execute o projeto.

5. Ao rodar o programa, você verá um menu interativo no terminal. Siga os passos para interagir com a API.

### Exemplo de Fluxo de Execução

- Após o login bem-sucedido:
  ```
  --- Menu de Opções ---
  1. Assinar ativo (Subscribe)
  2. Cancelar assinatura (Unsubscribe)
  3. Obter lista de players (Get Players)
  4. Sair
  Escolha uma opção:
  ```

- Para assinar um ativo, escolha a opção `1` e insira o nome do ativo (exemplo: `PETR4`).

- Para cancelar a assinatura de um ativo, escolha a opção `2`.

- Para obter os players disponíveis, escolha a opção `3`.

- Para encerrar o programa, escolha a opção `4`.

## 🧩 Estrutura do Código

- **Login e Autenticação**:
  - Realiza um loop para lidar com mensagens interativas do servidor (`Username:`, `Password:`, etc.).
  - Confirma a mensagem "You are connected" antes de prosseguir.

- **Menu Interativo**:
  - Implementado em um loop que processa as entradas do usuário e executa os comandos apropriados (`sqt`, `usq`, `gpn`).

- **Streaming Contínuo**:
  - Executa em um processo separado (`ListenToServer`) para exibir mensagens do servidor em tempo real.

- **Gerenciamento de Recursos**:
  - Finaliza a conexão e libera streams ao encerrar o programa.

## 📚 Requisitos

- .NET Core 3.1 ou superior.
- Credenciais válidas para acesso à API Cedro Crystal.

## 🔧 Extensões Futuras

- Adicionar suporte a mais comandos da API Cedro Crystal, como `SAB` (Subscribe Aggregated Book) e `GQT` (Get Quote Trade).
- Melhorar o manuseio de erros, como perda de conexão ou credenciais inválidas.
- Exportar os dados recebidos para arquivos CSV ou banco de dados.

## 📄 Licença

Este projeto é apenas para fins educacionais e serve como exemplo de integração com a API Cedro Crystal. Consulte os termos de uso da Cedro Technologies antes de usar esta integração em ambientes produtivos.

---

Com este exemplo, você terá um ponto de partida robusto para se conectar à API Cedro Crystal e explorar seus recursos de streaming em tempo real.