using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace DomainModel
{
    public static class Util
    {
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data,
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public static T FromJson<T>(this string element)
        {
            return JsonConvert.DeserializeObject<T>(element);
        }

        public static void SendRequest(this TcpClient client, string request)
        {
            var msg = Encoding.UTF8.GetBytes(request);
            client.GetStream().Write(msg, 0, msg.Length);
        }

        public static Response ReadResponse(this TcpClient client)
        {
            var strm = client.GetStream();
            strm.ReadTimeout = 2500;
            byte[] resp = new byte[5840];
            using (var memStream = new MemoryStream())
            {
                int bytesread = 0;
                do
                {
                    bytesread = strm.Read(resp, 0, resp.Length);
                    memStream.Write(resp, 0, bytesread);

                } while (bytesread == 5840);

                var responseData = Encoding.UTF8.GetString(memStream.ToArray());
                return JsonConvert.DeserializeObject<Response>(responseData);
            }
        }
    }
}
