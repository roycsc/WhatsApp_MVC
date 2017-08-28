using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatsApp_MVC.Models
{
    static public class ChatFormatter
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
            locationReply = 8,
            audio = 9,
            audioReply = 10,
            system = 100
        }

        static public void FormatChat(ChatViewModel chatViewModel, List<TableRow_Messages> msg, List<TableRow_Messages_Quotes> msgQuote)
        {
            //for each item in msg list
            //we decide the following factors 1 by 1
            //is key_from_me?
            //is it chat header?
            //is it normal msg or quoted msg?
            //chat type, normal or img or video?
            string chatOwner;
            bool isKeyFromMe;
            bool isChatHeader = true; //initialize true for 1st msg to show owner name
            bool isQuotedMsg;
            ChatTypeEnum chatType;

            for (int i = 0; i < msg.Count; i++)
            {
                chatOwner = msg[i].key_remote_jid;

                isKeyFromMe = msg[i].key_from_me;

                if (i > 0)
                    isChatHeader = (msg[i].key_remote_jid == msg[i - 1].key_remote_jid) ? false : true;

                if (msg[i].quoted_row_id != 0)
                {
                    isQuotedMsg = true;
                    msg[i].Table_Messsages_Quotes = msgQuote.Find(x => x.key_id == msg[i].key_id);
                }
                else
                {
                    isQuotedMsg = false;
                }

                switch (msg[i].mediam_mime_type)
                {
                    case null:
                        chatType = (isQuotedMsg) ? ChatTypeEnum.chatReply : ChatTypeEnum.normalChat; break;
                    case @"image/jpeg":
                        chatType = (isQuotedMsg) ? ChatTypeEnum.imageReply : ChatTypeEnum.image; break;
                    case @"video/mp4":
                        chatType = (isQuotedMsg) ? ChatTypeEnum.videoReply : ChatTypeEnum.video; break;
                    case @"audio/ogg; codecs=opus":
                        chatType = (isQuotedMsg) ? ChatTypeEnum.audioReply : ChatTypeEnum.audio; break;
                    default:
                        throw new InvalidOperationException("media_mime_type not match");
                }

                chatViewModel.FormattedMessage.Add(ChatTemplater(msg[i], chatType, isChatHeader, isKeyFromMe));
            }
        }

        static public string ChatTemplater(TableRow_Messages msg, ChatTypeEnum chatType, bool isChatHeader, bool isKeyFromMe)
        {
            switch (chatType)
            {
                case ChatTypeEnum.normalChat:
                    return FormatNormalChat(msg, isChatHeader, isKeyFromMe);
                default:
                    return @"<li>temporarily do nothing</li>";
                    //throw new InvalidOperationException("ChatTemplate must have a valid value as listed in ChatTypeEnum");
            }
        }

        static public string FormatNormalChat(TableRow_Messages msg, bool isChatHeader, bool isKeyFromMe)
        {
            string liOpenTag;
            string liCloseTag;
            string divOpenTag;
            string divCloseTag;
            string chatHeader;
            string msgBody;
            string timestamp;

            if (isChatHeader && isKeyFromMe)
            {
                liOpenTag = @"<li class='self-header'>";
                divOpenTag = @"<div class='msg-bubble msg-self'>";  
            }
            else if (isChatHeader && !isKeyFromMe)
            {
                liOpenTag = @"<li class='others-header'>";
                divOpenTag = @"<div class='msg-bubble msg-others'>";
            }
            else if (!isChatHeader && isKeyFromMe)
            {
                liOpenTag = @"<li class='self'>";
                divOpenTag = @"<div class='msg-bubble msg-self'>";
            }
            else
            {
                liOpenTag = @"<li class='others'>";
                divOpenTag = @"<div class='msg-bubble msg-others'>";
            }

            if (isChatHeader)
                chatHeader = @"<strong>" + msg.key_remote_jid + @"</strong><br/>";
            else
                chatHeader = "";

            liCloseTag = @"</li>";
            divCloseTag = @"</div>";
            msgBody = msg.Data;
            timestamp = @"<time><br/>" + msg.timestamp + @"</time>";
            
            var htmlTemplate = 
                liOpenTag + 
                divOpenTag + 
                chatHeader + 
                msgBody + 
                timestamp + 
                divCloseTag + 
                liCloseTag;

            return htmlTemplate;
        }
    }
}