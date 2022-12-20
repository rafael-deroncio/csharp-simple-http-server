using System.Net;
using System.Net.Sockets;
using System.Text;

class ServidorHttp : IServerHttp
{
    private TcpListener Controller { get; set; }
    private int Port { get; set; }
    private int QtdRequests { get; set; }
    private string Html { get; set; }

    public ServidorHttp(int port = 8080)
    {
        this.Port = port;
        this.GenerateHtml();

        try
        {
            this.Controller = new TcpListener(IPAddress.Parse("127.0.0.1"), this.Port);
            this.Controller.Start();

            Console.WriteLine($"Server HTTp runing in port: {this.Port}.");
            Console.WriteLine($"To Access, write in browser: http://localhost:{this.Port}.");

            Task serverHttpTask = Task.Run(() => WaitingRequest());
            serverHttpTask.GetAwaiter().GetResult();
        }
        catch (Exception e)
        {
            
            Console.WriteLine($"Error starting server in port {this.Port}:\n{e.Message}");
        }
    }

    private async Task WaitingRequest()
    {
        while (true)
        {
            Socket socket = await this.Controller.AcceptSocketAsync();
            this.QtdRequests ++;

            Task task = Task.Run(() => ProcessRequest(socket, this.QtdRequests));
        }
    }

    private void ProcessRequest(Socket socket, int requestNumber)
    {
        Console.WriteLine($"Processing request #{requestNumber} ... \n");

        if (socket.Connected)
        {
            byte[] bytesRequest = new byte[1024];
            socket.Receive(bytesRequest, bytesRequest.Length, 0);

            string textRequest = Encoding.UTF8.GetString(bytesRequest).Replace((char)0, ' ').Trim();

            if (textRequest.Length > 0)
            {
                Console.WriteLine($"\n{textRequest}\n");

                byte[] bytesContext = ReadFile("/index.html");
                byte[] bytesHeader = GenerateHeader("HTTP/1.1", "text/html;charset=utf-8", "200", bytesContext.Length);

                int bytesSending = socket.Send(bytesHeader, bytesHeader.Length, 0);
                bytesSending += socket.Send(bytesContext, bytesContext.Length, 0);

                socket.Close();
            }

            Console.WriteLine($"\nRequest {requestNumber} Ended.");

        }

    }

    private byte[] GenerateHeader(string httpVersion, string mimeType, string httpCode, int qtdBytes = 0)
    {
        StringBuilder header = new StringBuilder();

        header.Append($"{httpVersion} {httpCode}{Environment.NewLine}");
        header.Append($"Server: Servidor HTTP Simples 1.0{Environment.NewLine}");
        header.Append($"Content-Type: {mimeType}{Environment.NewLine}");
        header.Append($"Content-Length: {qtdBytes}{Environment.NewLine}{Environment.NewLine}");

        return Encoding.UTF8.GetBytes(header.ToString());
    }

    private void GenerateHtml()
    {
        StringBuilder html = new StringBuilder();

        html.Append("<!DOCTYPE html><html lang=\"pt-br\"><head><meta charset=\"UTF-8\">");
        html.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        html.Append("<title>Static Page</title></head><body>");
        html.Append("<h1>Index</h1></body><html>");

        this.Html = html.ToString();
    }

    private byte[] ReadFile(string resource)
    {
        string directory = "C:\\Users\rafae\\Documents\\Github\\DotNetProjects\\ServidorHttpSimples\\www";
        string filePath = directory + resource.Replace("/", "\\");

        if (File.Exists(filePath))
            return File.ReadAllBytes(filePath);
        
        return new byte[0];
    }
}