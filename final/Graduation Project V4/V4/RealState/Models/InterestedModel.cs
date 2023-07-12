using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RealState.Models
{
    public class InterestedModel
    {
        public string Name { get; set; }
        public string icon { get; set; }
        public bool IsCheck { get; set; }

    }

    public class InterestedList
    {
        public List<InterestedModel> interests { get; set; }
    }
}