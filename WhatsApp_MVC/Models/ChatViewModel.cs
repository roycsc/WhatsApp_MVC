using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace WhatsApp_MVC.Models
{
    public class ChatViewModel
    {
        // the db data on this line
        static private int _chatId = 0;
        ChatTemplate chatType = new ChatTemplate();
        
        
        public ChatViewModel()
        {
            
        }
    }
}