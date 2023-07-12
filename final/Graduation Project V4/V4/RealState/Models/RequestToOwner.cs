using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealState.Models
{
    public class RequestToOwner
    {
        public string RequestId{ get; set; }
        public string StateId{ get; set; }
        public string requestUserId { get; set; }
        public string NameUserRequest { get; set; }
    }
}