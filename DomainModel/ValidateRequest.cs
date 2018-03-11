using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DomainModel
{
    public class HandleRequest
    {
        List<string> responselist = new List<String>();
        Response response = new Response();
        Request request;

        private Service _database;

        public HandleRequest()
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
            ValidatePath(request);
            MissingDate(request);
            MissingBody(request);
            IllegalBody(request);
            EchoRequest(request);
            
            return ReturnStatus();
        } 

        private Response ReturnStatus()
        {            
            foreach (string s in responselist)
            {
                response.Status = response.Status + ", " + s;
            }
            return this.response;
        }
        //method
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


        //path  
        public void ValidatePath(Request request)
        {
            var pathRoot = "/api/categories";
            if (request.Path != null)
            {
                if (request.Path.Contains(pathRoot))
                {
                    if (request.Method != null)
                    {
                        var el = request.Path.Split('/').Select(x => x.Trim()).ToArray();
                        if (request.Method == "update" || request.Method == "update")
                        {  
                            if (el.Length == 2 )
                            {
                                responselist.Add("missing resource");
                            }
                            else if(el.Length > 3)
                            {
                                responselist.Add("illegal path");
                            }
                            else if (el.Length == 3)
                            {
                                try
                                {
                                    var i = int.Parse(el[2]);
                                    responselist.Add("2 ok");
                                }
                                catch (Exception e)
                                {
                                    responselist.Add("Bad Request");
                                } 
                            }
                        }
                        else if (request.Method == "read" || request.Method == "create")
                        {
                            if (el.Length == 2)
                            {
                                responselist.Add("2 ok");
                            }
                        }
                        else
                        {
                            responselist.Add("Bad Request");
                        } 
                    }
                }
                else
                {
                    responselist.Add("illegal path");
                } 
            }
            else
            {
                responselist.Add("missing path");
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
            if (request.Body != null)
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


        public Response Read()
        {
            if (request.Method == "read")
            {
                var el = request.Path.Split('/').Select(x => x.Trim()).ToArray();
                if (el.Length == 2)
                { 
                    if (_database.GetAllCategories() != null)
                    {
                        response.Body = _database.GetAllCategories().ToString();
                    }
                }
                if (el.Length == 3)
                {

                    try
                    {
                        var pathid = int.Parse(el[2]);

                        if (_database.CategoryExists(pathid))
                        {
                            response.Body = _database.GetCategory(pathid).ToString();
                            response.Status = "2 ok";
                        }
                        else
                        {
                            response.Status = "5 Not Found";
                        }
                        
                    }
                    catch (Exception e)
                    {
                        response.Status = "Bad Request";
                    }
                     
                    
                }
            }

            return response;
        }
    }
}

