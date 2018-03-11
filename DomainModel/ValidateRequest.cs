using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DomainModel
{
    public class ValidateRequest
    {
        List<string> responselist = new List<String>();
        Response response = new Response();
        Request request;

        private Service _database;

        public ValidateRequest()
        {
            _database = new Service();
        }

        public Response InputValidation(Request request)
        {

            if(request == null)
            {
                request = new Request();
            }
            MissingMethod(request);
            IlligalMethod(request);
            MissingPath(request);
            MissingDate(request);
            MissingBody(request);
            IllegalBody(request);
            EchoRequest(request);
            ApiRead(request);
            return GenerateResponse();
        }

        private Response GenerateResponse()
        {            
            foreach (string s in responselist)
            {
                response.Status = response.Status + ", " + s;
            }
            return this.response;
        }

        private void MissingMethod(Request request)
        {
            if (request.Method == null)
            {
                responselist.Add("4 missing method");
            }
        }

        private void IlligalMethod(Request request)
        {
            if(request.Method != "create")
            {
                if(request.Method != "delete")
                {
                    if (request.Method != "read")
                    {
                        if (request.Method != "update")
                        {
                            if (request.Method != "echo")
                            {
                                if(request.Method == null)
                                {

                                }
                                else
                                responselist.Add("illegal method");
                            }
                        }                        
                    }
                }                
            }
        }

        private void MissingPath(Request request)
        {
            if (request.Path == null)
            {
                responselist.Add("missing resource");
            }
        }

        private void MissingDate(Request request)
        {
            if(request.Date == 0)
            {
                responselist.Add("missing date");
            }
        }

        private void MissingBody(Request request)
        {
            if(request.Body == null)
            {
                responselist.Add("missing body");
            }
        }

        private void IllegalBody(Request request)
        {            
            request.Body = request.Body.Trim();
            if ((request.Body.StartsWith("{") && request.Body.EndsWith("}")) ||
                (request.Body.StartsWith("[") && request.Body.EndsWith("]"))) 
            {
                try
                {
                    var obj = JToken.Parse(request.Body);                    
                }
                catch (JsonReaderException jex)
                {
                    responselist.Add("illegal body");
                }
                catch (Exception ex) 
                {
                    responselist.Add("illegal body");
                }
            }
            else
            {
                responselist.Add("illegal body");
            }
        }    
        
        private void EchoRequest(Request request)
        {
            if(request.Method == "echo")
            {
                response.Body = request.Body;
            }                
        }

        /* API */
        //check
        private string[] GetPath(Request request)
        {
            string[] array = new string[2];
            array[0] = request.Path.Substring(request.Path.LastIndexOf("/api/") + 1);
            array[1] = request.Path.Substring(request.Path.LastIndexOf("/api/categories/") + 1);
            return array;
        }

        private void ApiRead(Request request)
        {
            var path = GetPath(request);
            var pathid = int.Parse(path[1]);

            if (request.Method == "read")
            {
               if(path[0] == "categories")
               {
                    //var test = JsonConvert.SerializeObject(_database.GetAllCategories());
                    response.Body = _database.GetAllCategories().ToString();
                    responselist.Add("1 OK");
               }
               else if (path[1].Length > 0)
               {
                    if(_database.CategoryExists(pathid))
                    {
                        response.Body = _database.GetCategory(pathid).ToString();
                        responselist.Add("1 OK");
                    }
                    else
                    {
                        responselist.Add("5 Not Found");
                    }
               }
               else 
               { 
                    responselist.Add("4 Bad Request"); 
               }
               
            }
        }


    }
}

