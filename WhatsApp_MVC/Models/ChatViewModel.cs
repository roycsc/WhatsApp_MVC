using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WhatsApp_MVC.Models
{
    public class ChatViewModel
    {
        public List<string> FormattedMessage { get; set; } = new List<string>();              
    }
}