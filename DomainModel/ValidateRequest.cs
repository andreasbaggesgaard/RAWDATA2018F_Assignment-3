using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace DomainModel
{
    public class HandleRequest
    {
        List<string> responselist = new List<String>();
        Response response = new Response();
        Request request;
        string _pathRoot = "/api/categories";

        private Service _database;

        public HandleRequest(Request request)
        {
            _database = new Service();
            this.request = request;
        }

        public Response InputValidation()
        {
            ValidateMethod(request);
            ValidatePath(request);
            MissingDate(request);

            //body not required
            //MissingBody(request);

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

        private void ValidateMethod(Request request)
        {
            if (request.Method != "create")
            {
                if (request.Method != "delete")
                {
                    if (request.Method != "read")
                    {
                        if (request.Method != "update")
                        {
                            if (request.Method != "echo")
                            {
                                if (request.Method == null)
                                {
                                    responselist.Add("Missing method");
                                }
                                else
                                    responselist.Add("Illegal method");
                            }
                        }
                    }
                }
            }
            else
            {
                response.Status = "2 Ok";
            }
        }

        //path  
        public void ValidatePath(Request request)
        {

            if (request.Path != null)
            {
                if (request.Path.Contains(_pathRoot))
                {
                    if (request.Method != null)
                    {
                        var el = request.Path.Split('/').Select(x => x.Trim()).ToArray();
                        // "" "api" "categories" "resource"
                        if (el.Length > 4)
                        {
                            responselist.Add("4 Bad Request");
                        }
                        if (request.Method == "update" || request.Method == "delete" || request.Method == "read")
                        {
                            if (el.Length == 3)
                            {
                                response.Status = "2 Ok";
                            }
                            //try
                            //{
                            //    var i = int.Parse(el[2]);
                            //    responselist.Add("2 ok");
                            //}
                            //catch (Exception e)
                            //{
                            //    responselist.Add("Bad Request");
                            //} 

                        }
                        if (request.Method == "read" || request.Method == "create")
                        {
                            if (el.Length == 4)
                            {
                                response.Status = "2 Ok";
                            }
                        }
                    }
                }
                else
                {
                    responselist.Add("4 Illegal path");
                }
            }
            else
            {
                responselist.Add("4 Missing path");
            }
        }


        private void MissingDate(Request request)
        {
            if (request.Date == 0)
            {
                responselist.Add("missing date");
            }
        }
        /* 
         * body is not  required
         */
        //private void MissingBody(Request request)
        //{
        //    if(request.Body == null)
        //    {
        //        responselist.Add("missing body");
        //    }
        //}

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
            }
        }

        private void EchoRequest(Request request)
        {
            if (request.Method == "echo")
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
                if (el.Length == 3)
                {
                    if (_database.GetAllCategories() != null)
                    {
                        response.Body = JsonConvert.SerializeObject(_database.GetAllCategories(), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                        response.Status = "1 Ok";
                    }
                }
                if (el.Length == 4)
                {

                    try
                    {
                        var pathid = int.Parse(el[3]);

                        if (_database.CategoryExists(pathid))
                        {
                            response.Body = JsonConvert.SerializeObject(_database.GetCategory(pathid), new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });  
                            response.Status = "1 Ok";
                        }
                        else
                        {
                            response.Status = "5 Not Found";
                        }
                    }
                    catch (Exception e)
                    {
                        response.Status = "4 Bad Request";
                        return response;
                    }


                }
            }

            return response;
        }



        /*New elements can be added by use of the path and the new element in the body of the
        request. Using path and an id is invalid and should return “4 Bad request”. On successful
        creation return the “2 Created” status plus the newly create element in the body.*/
        public void Create()
        {
            if (request.Method == "create")
            {
                var el = request.Path.Split('/').Select(x => x.Trim()).ToArray();
                if (el.Length == 3)
                {
                    //create new 
                    _database.AddCategory(request.Body);
                    response.Status = "2 Created";
                }
                if (el.Length == 4)
                {
                    //bad request
                    response.Status = "4 Bad Request";
                }
            }
        }


        //All elements can be updated by use of the path extended with the id and the updated element
        //in the body.Updates without an id in the path is not allowed and should return “4 Bad
        //request”. On successful update return the “3 Updated” status

        public void Update()
        {
            if (request.Method == "update")
            {
                var el = request.Path.Split('/').Select(x => x.Trim()).ToArray();
                if (el.Length == 3)
                {
                    response.Status = "4 Bad Request";

                }
                if (el.Length == 4)
                {
                    var pathid = int.Parse(el[3]);

                    if (_database.CategoryExists(pathid))
                    {
                        _database.UpdateCategory(_database.GetCategory(pathid));
                        response.Status = "3 Updated";
                    }
                    else
                    {
                        response.Status = "5 Not Found";
                    }
                }
            }

        }


        /*
        delete
        All elements can be deleted by use of the path extended with the id. If the element is not in
        the database “5 Not found” should be returned otherwise “1 Ok”.*/

        public void Delete()
        {
            if (request.Method == "delete")
            {
                var el = request.Path.Split('/').Select(x => x.Trim()).ToArray();
                if (el.Length == 3)
                {
                    response.Status = "4 Bad Request";
                }
                if (el.Length == 4)
                {
                    var pathid = int.Parse(el[3]);

                    if (_database.CategoryExists(pathid))
                    {
                        _database.DeleteCategory(pathid);
                        response.Status = "1 Ok";
                    }
                    else
                    {
                        response.Status = "5 Not Found";
                    }



                }
            }

        }


    }
}

