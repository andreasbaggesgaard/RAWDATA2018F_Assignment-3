﻿using System;
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
        Database _database;

        public HandleRequest(Request request, Database _database)
        {
            this._database = _database;
            this.request = request;
        }

        public Response InputValidation()
        {
            ValidateMethod(request);
            ValidatePath(request);
            ValidateDate(request);
            ValidateBody(request);
            return ReturnStatus();
        }

        private Response ReturnStatus()
        {
            if (!responselist.Any())
            {
                response.Status = "1 Ok";
            }
            else if (responselist.Count == 1)
            {
                response.Status = "";
                response.Status = responselist.First();
            }
            else
            {
                response.Status = "";
                foreach (string s in responselist)
                {
                    response.Status = response.Status + ", " + s;
                }
            }
            return this.response;
        }
        //Method 

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
                                    responselist.Add("4 Missing Method");
                                }
                                else
                                    responselist.Add("4 Illegal Method");
                            }
                        }
                    }
                }
            }
            else
            {
                response.Status = "1 Ok";
            }
        }

        //path  
        private void ValidatePath(Request request)
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
                        if (request.Method == "read" || request.Method == "create")
                        {
                            if (el.Length == 3)
                            {
                                response.Status = "1 Ok";
                            }
                        }
                        else if (request.Method == "delete" || request.Method == "update" || request.Method == "read")
                        {
                            if (el.Length == 4)
                            {
                                response.Status = "1 Ok";
                            }
                            else
                            {
                                responselist.Add("4 Bad Request");
                            }
                        }
                    }
                }
                else
                {
                    responselist.Add("4 Bad Request");
                }
            }
            else if (request.Path == null && request.Method != null)
            {
                if (request.Method == "echo")
                {
                    response.Status = "1 OK";
                }
                else if (request.Method == "create" || request.Method == "read" || request.Method == "update" || request.Method == "delete")
                {
                    responselist.Add("4 Missing Resource");
                }
            }
        }

        private void ValidateDate(Request request)
        {
            if (request.Date == null)
            {
                responselist.Add("4 Missing date");
            }

            /*
             in Unix time (i.e. number of seconds that have elapsed since 1970-01- 01T00:00:00Z)
             */
            else
            {
                try
                {
                    int.Parse(request.Date);
                }
                catch (Exception)
                {
                    responselist.Add("4 Illegal date");

                }
            }
        }

        private void ValidateBody(Request request)
        {

            if (request.Body != null)
            {
                if (request.Method != null)
                {
                    if (request.Method == "echo")
                    {
                        response.Status = "1 OK";
                    }
                    else if (request.Method == "create" || request.Method == "read" || request.Method == "update" || request.Method == "delete")
                    {
                        {
                            try
                            {
                                var obj = JToken.Parse(request.Body);
                            }
                            catch (JsonReaderException jex)
                            {
                                responselist.Add("4 Illegal Body");
                            }
                            catch (Exception ex)
                            {
                                responselist.Add("4 Illegal Body");
                            }

                        }
                    }
                }
            }
            else if (request.Body == null)
            {
                if (request.Method != null)
                {
                    if (request.Method == "create" || request.Method == "update" || request.Method == "echo")
                    {
                        responselist.Add("4 Missing Body");
                    }
                }
            }
        }

        public void Echo()
        {
            if (request.Method == "echo")
            {
                if (request.Body != null)
                {
                    response.Body = request.Body;
                }
                else
                {
                    response.Status = "4 Missing Body";
                }
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
                        response.Body = Util.ToJson(_database.GetAllCategories());
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
                            response.Body = Util.ToJson(_database.GetCategory(pathid));
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
                if (request.Body != null)
                {

                    var el = request.Path.Split('/').Select(x => x.Trim()).ToArray();

                    if (el.Length == 3)
                    {
                        //create new 
                        var newCat = Util.FromJson<Category>(request.Body);
                        newCat.cid = _database.AddCategory(newCat.name).cid;
                        response.Body = Util.ToJson(newCat);
                        response.Status = "2 Created";
                    }
                    if (el.Length == 4)
                    {
                        //bad request
                        response.Status = "4 Bad Request";
                    }
                }
                else
                {
                    response.Status = "4 Missing Body";
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
                if (request.Body != null)
                {
                    var el = request.Path.Split('/').Select(x => x.Trim()).ToArray();

                    if (el.Length <= 3)
                    {
                        response.Status = "4 Bad Request";
                    }
                    else if (el.Length == 4)
                    {
                        var pathid = int.Parse(el[3]);

                        if (_database.CategoryExists(pathid))
                        {
                            if (request.Body != null)
                            {
                                var updatedCat = Util.FromJson<Category>(request.Body);
                                response.Body = Util.ToJson(_database.UpdateCategory(updatedCat));
                                response.Status = "3 Updated";
                            }
                        }
                        else
                        {
                            response.Status = "5 Not Found";
                        }
                    }
                    else
                    {
                        response.Status = "4 Bad Request";
                    }
                }
                else
                {
                    response.Status = "4 Bad Request";
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
                        response.Body = Util.ToJson(_database.DeleteCategory(pathid));
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

