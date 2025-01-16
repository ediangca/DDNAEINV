using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace DDNAEINV.Models.Entities
{
    public class AuthResponse
    {
        public AuthResponse()
        {
            this.Token = "";
            this.responseMsg = new HttpResponseMessage() { StatusCode = System.Net.HttpStatusCode.Unauthorized };

        }

        public string Token { get; set; }
        public HttpResponseMessage responseMsg { get; set; }
    }
}