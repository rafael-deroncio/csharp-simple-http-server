using System.Net;
using System.Net.Sockets;
using System.Text;

class ServidorHttp : IServerHttp
{
    private TcpListener Controller { get; set; }
    private int Port { get; set; }
    private int QtdRequests { get; set; }
    private string Html { get; set; }
    private string IP { get; set; }

    public ServidorHttp(string ipAddress = "127.0.0.1", int port = 8080)
    {
        this.IP = ipAddress;
        this.Port = port;

        try
        {
            this.Controller = new TcpListener(IPAddress.Parse(this.IP), this.Port);
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
            int bytesSending = 0;
            byte[] bytesContext = null;
            byte[] bytesHeader = null;    
            byte[] bytesRequest = new byte[1024];
            
            socket.Receive(bytesRequest, bytesRequest.Length, 0);

            HandledRequest request = new HandledRequest(bytesRequest);

            MimeTypesModel mimeTypes = new MimeTypesModel();

            FileInfo fileInfo = new FileInfo(GetFilesPhysicalPath(request.ResourceRequested));

            Console.WriteLine($"{request.RawText}\n");

            if (fileInfo.Exists)
            {
                if (mimeTypes.Types.ContainsKey(fileInfo.Extension.ToLower()))
                {
                    string mimeType = mimeTypes.Types[fileInfo.Extension.ToLower()] + ";charset=utf-8";

                    bytesContext = File.ReadAllBytes(fileInfo.FullName);
                    bytesHeader = GenerateHeader(request.VersionHttp, mimeType, "200", bytesContext.Length);
                }
                else
                {
                    string mimeType = mimeTypes.Types[".html"] + ";charset=utf-8";
                    
                    bytesContext = Encoding.UTF8.GetBytes("<h1>Erro 415 - Type file not suported</h1>");
                    bytesHeader = GenerateHeader(request.VersionHttp, mimeType, "415", bytesContext.Length);
                }
            }
            else
            {
                string mimeType = mimeTypes.Types[".html"] + ";charset=utf-8";

                bytesContext = Encoding.UTF8.GetBytes("<h1>Erro 404 - Page not found</h1>");
                bytesHeader = GenerateHeader(request.VersionHttp, mimeType, "404", bytesContext.Length);
            }

            bytesSending = socket.Send(bytesHeader, bytesHeader.Length, 0);

            bytesSending += socket.Send(bytesContext, bytesContext.Length, 0);

            socket.Close();
            
            Console.WriteLine($"Request {requestNumber} Ended.\nBytes Sending: {bytesSending}");
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

    private string GetFilesPhysicalPath(string file)
    {
        string directory = "C:\\Users\\rafae\\Documents\\Github\\DotNetProjects\\ServidorHttpSimples\\www\\html\\";
        string filePath = directory + file.Replace("/", "\\");

        return filePath;
    }
}