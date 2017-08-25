using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatsApp_MVC.Models
{
    public class ChatTemplate
    {
        public enum ChatTypeEnum
        {
            normalChat = 1,
            chatReply = 2,
            image = 3,
            imageReply = 4,
            video = 5,
            videoReply = 6,
            location = 7,
            system = 100
        }

        static public int Id { get; set; }
        public bool ChatLead { get; set; } //flag for the start of a group of chat from same person, to put in person's name

        public string ChatFormatter(ChatTypeEnum chatType, string msg)
        {
            switch ((int)chatType)
            {
                case 1: return FormatNormalChat(msg);
                default: throw new InvalidOperationException("ChatTemplate must have a valid value as listed in ChatTypeEnum");
            }
        }

        public string FormatNormalChat(string msg)
        {
            var htmlTemplate = "";
            return htmlTemplate;
        }
    }
}