using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel
{
    public class Request
    {
        //public enum Method { create, read, update, delete, echo}
        public string Method { get; set; }
        public string Path { get; set; } 
        public string Body { get; set; }
        public string Date { get; set; }

    }
}
