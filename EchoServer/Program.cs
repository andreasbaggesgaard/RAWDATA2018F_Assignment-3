using DomainModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Newtonsoft.Json.Serialization;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace EchoServer
{
    class Program
    {
        private TcpListener _server;
        private Database _database;


        static void Main(string[] args)
        {
            new Program().StartServer();
        }

        public Program()
        {
            var addr = IPAddress.Parse("127.0.0.1");
            _server = new TcpListener(addr, 5000);
            _database = new Database();
        }


        public void StartServer()
        {
            _server.Start();
            Console.WriteLine("Server started ...");

            while (true)
            {
                var client = _server.AcceptTcpClient();
                new Thread(() => ProcessData(client)).Start();

            }
        }

        private void ProcessData(TcpClient client)
        {
            try
            {
                var strm = client.GetStream();

                var buffer = new byte[client.ReceiveBufferSize];

                var readCnt = strm.Read(buffer, 0, buffer.Length);

                var payload = Encoding.UTF8.GetString(buffer, 0, readCnt);
                var request = JsonConvert.DeserializeObject<Request>(payload);

                var handleRequest = new HandleRequest(request, _database);

                var response = handleRequest.InputValidation();
                var res = new byte[0];

                if (response.Status.ToLower().Contains("ok"))
                {
                    // do the method
                    switch (request.Method)
                    {
                        case "read":
                            handleRequest.Read();
                            break;
                        case "create":
                            handleRequest.Create();
                            break;
                        case "update":
                            handleRequest.Update();
                            break;
                        case "delete":
                            handleRequest.Delete();
                            break;
                        case "echo":
                            handleRequest.Echo();
                            break;
                        default:
                            break;
                    }
                } 
                res = Encoding.UTF8.GetBytes(response.ToJson()); 
                strm.Write(res, 0, res.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("something went wrong" + e.Message + e.StackTrace); 
            }
        }  
    } 
}
