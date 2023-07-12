using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealState.Models
{
    public class ViewModel
    {
        public List<RequestToOwner> requests { get; set; }
        public List<State> states { get; set; }
        public State state { get; set; }
    }
}