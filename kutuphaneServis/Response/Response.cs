using kutuphaneServis.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kutuphaneServis.Response
{
    public class Response 
    {
        /*
            Neden protected set?
            Dışarıdan kafaya göre değiştirilmesin diye. Yalnızca sınıfın kendisi ve miras alan tipler set edebilsin. Böylece “yarı immutable” bir zarfınız olur.
         */
        public bool IsSuccess { get; protected set; }

        public string Message {  get; protected set; }
        public Response(bool issuccess, string message)
        {
            IsSuccess = issuccess;
            Message = message;  
        }
        public static Response Success(string message = "")
        {
            return new Response(true, message);
        }

        public static Response Error(string message = "")
        {
            return new Response(false, message);
        }
    }
}
