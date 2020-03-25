using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace swvo_websocket_terminator
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: swvo-websocket-terminator SessId Host:Port BookmarkName LocalPort");
                Console.WriteLine("Example: swvo-websocket-terminator 44E4224D3E4B1C74FCADE675803C1162 sonicwall.company.com:4433 SSH 2222");
                return;
            }

            var sessId = args[0];
            var hostPort = args[1];
            var bookmarkName = args[2];
            var localPort = Int32.Parse(args[3]);

            var domain = hostPort.Split(':')[0];
            
            var socket = new ClientWebSocket();
            socket.Options.AddSubProtocol("binary");
            socket.Options.Cookies = new CookieContainer();
            socket.Options.Cookies.Add(new Cookie("SessId", sessId){Domain = domain});
            try
            {
                await socket.ConnectAsync(new Uri($"wss://{hostPort}/ws?bookmark={WebUtility.UrlEncode(bookmarkName)}"), default);
            }
            catch
            {
                Console.WriteLine("Unable to connect to server");
                return;
            }

            Console.WriteLine("Connected to server");

            var tl = new TcpListener(IPAddress.Loopback, localPort);
            tl.Start();
            Console.WriteLine("Waiting for local connection");
            var client = await tl.AcceptTcpClientAsync();

            Console.WriteLine("Starting IO tasks");
            var t1 = Task.Run(async () =>
            {
                var buffer = new Byte[16777216];
                var i = client.GetStream();
                Int32 bytesRead;

                while ((bytesRead = i.Read(buffer)) > 0)
                {
                    //Trace.WriteLine($"{bytesRead} to server");
                    await socket.SendAsync(buffer.AsMemory(0, bytesRead), WebSocketMessageType.Binary, true,
                        default);
                }
            });
            
            var t2 = Task.Run(async () =>
            {
                var buffer = new Byte[16777216];
                var o = client.GetStream();
                WebSocketReceiveResult result;
                while ((result = await socket.ReceiveAsync(buffer, default)).Count > 0)
                {
                    //Trace.WriteLine($"{result.Count} to client");
                    o.Write(buffer.AsSpan(0, result.Count));
                }
            });

            try
            {
                await t1;
                await t2;
            }
            catch
            {
                // Don't care.
            }
            Console.WriteLine("Program complete");
        }
    }
}