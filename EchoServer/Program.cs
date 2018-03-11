﻿using DomainModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using Newtonsoft.Json.Serialization;


namespace EchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var addr = IPAddress.Parse("127.0.0.1");
            var server = new TcpListener(addr, 5000);
            Response response = new Response();

            server.Start();
            Console.WriteLine("Server started ...");
           

            while (true)
            {

                var client = server.AcceptTcpClient();

                new Thread(() => ProcessData(client)).Start();

            }


        }

        private static void ProcessData(TcpClient client)
        {
            var strm = client.GetStream();

            var buffer = new byte[client.ReceiveBufferSize];
            
            var readCnt = strm.Read(buffer, 0, buffer.Length);

            var payload = Encoding.UTF8.GetString(buffer, 0, readCnt);
            var request = JsonConvert.DeserializeObject<Request>(payload);

            var validate = new ValidateRequest();
            
            //Console.WriteLine(request.Method);
            
            var res = Encoding.UTF8.GetBytes(validate.InputValidation(request).ToJson());

            strm.Write(res, 0, res.Length);
        }

        /*
        private static Response ValidateMethod(Request request)
        {            
            if (request.Method == null)
            {
                return new Response
                {
                    Status = "4 missing method"
                };
            }

            private void MissingMethod()
            {

            };
            
            
            else return null;
        }        
        */
        
    }

    public static class Util
    {
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data,
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

    }
}
