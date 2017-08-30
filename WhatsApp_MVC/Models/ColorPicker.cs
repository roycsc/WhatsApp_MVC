using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatsApp_MVC.Models
{
    public class ColorPicker
    {
        public Dictionary<string, string> OwnerColorListing { get; set; } = new Dictionary<string, string>()
        {
            //color-1 is only used by self, which is an empty string under remote_resource
            {"", "-color-1" },
            {"You", "-color-1" }
        };
        public List<string> CssColorList { get; set; } = new List<string>()
        {
            //colors are from css file
            "-color-2",
            "-color-3",
            "-color-4",
            "-color-5",
            "-color-6",
            "-color-7",
            "-color-8",
            "-color-9",
            "-color-10",
            "-color-11",
            "-color-12",
            "-color-13",
            "-color-14",
            "-color-15",
            "-color-16",
            "-color-17",
            "-color-18",
            "-color-19",
            "-color-20"
        };        
    }
}