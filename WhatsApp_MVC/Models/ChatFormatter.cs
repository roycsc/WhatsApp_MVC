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
            normalChatReply = 2,
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

        static public void FormatChat(ChatViewModel chatViewModel, List<TableRow_Messages> msg, List<TableRow_Messages_Quotes> msgQuote, ColorPicker chatOwnerColor)
        {
            string chatOwner;
            bool isKeyFromMe;
            bool isChatHeader; //chatheader means the 1st conversation for a continuous msg from the same person, purpose is to display their name
            bool isQuotedMsg;
            ChatTypeEnum chatType;

            for (int i = 0; i < msg.Count; i++)
            {
                isKeyFromMe = msg[i].key_from_me;

                if (!isKeyFromMe && i > 0)
                {
                    isChatHeader = (msg[i].remote_resource == msg[i - 1].remote_resource) ? false : true; //check if current chat same owner as previous chat
                    chatOwner = msg[i].remote_resource;
                }
                else if (isKeyFromMe)
                {
                    isChatHeader = false;
                    chatOwner = chatOwner = msg[i].remote_resource;
                }
                else if (!isKeyFromMe && i == 0)
                {
                    isChatHeader = true;
                    chatOwner = msg[i].remote_resource;
                }
                else
                    isChatHeader = false; //should be logged if reach this section, above logic should cover all possibilities
                
                if (msg[i].quoted_row_id != 0)
                {
                    isQuotedMsg = true;
                    msg[i].Table_Messsages_Quotes = msgQuote.Find(x => x._id == msg[i].quoted_row_id);
                }
                else
                {
                    isQuotedMsg = false;
                }

                if (true)
                {

                }

                switch (msg[i].media_mime_type)
                {
                    case "":
                        if (isQuotedMsg)
                        {
                            switch (msg[i].Table_Messsages_Quotes.media_mime_type)
                            {
                                case "":
                                    chatType = ChatTypeEnum.normalChatReply;
                                    break;
                                case @"image/jpeg":
                                    chatType = ChatTypeEnum.imageReply; break;
                                case @"video/mp4":
                                    chatType = ChatTypeEnum.videoReply; break;
                                case @"audio/ogg; codecs=opus":
                                    chatType = ChatTypeEnum.audioReply; break;
                                default:
                                    throw new InvalidOperationException("media_mime_type not match for quote table");
                                    //should just log instead of throw exception in future
                            }
                        }
                        else
                        {
                            chatType = ChatTypeEnum.normalChat;
                        }
                        break;
                    case @"image/jpeg":
                        chatType = ChatTypeEnum.image; break;
                    case @"video/mp4":
                        chatType = ChatTypeEnum.video; break;
                    case @"audio/ogg; codecs=opus":
                        chatType = ChatTypeEnum.audio; break;
                    default:
                        throw new InvalidOperationException("media_mime_type not match");
                        //should just log instead of throw exception in future
                }

                chatViewModel.FormattedMessage.Add(ChatTemplater(msg[i], chatType, isChatHeader, isKeyFromMe, chatOwnerColor));
            }
        }

        static private string ChatTemplater(TableRow_Messages msg, ChatTypeEnum chatType, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
        {
            switch (chatType)
            {
                case ChatTypeEnum.normalChat:
                    return FormatNormalChat(msg, isChatHeader, isKeyFromMe, chatOwnerColor);
                case ChatTypeEnum.normalChatReply:
                    return FormatNormalChatReply(msg, isChatHeader, isKeyFromMe, chatOwnerColor);
                case ChatTypeEnum.image:
                    return FormatImage(msg, isChatHeader, isKeyFromMe, chatOwnerColor);
                case ChatTypeEnum.imageReply:
                    return FormatImageReply(msg, isChatHeader, isKeyFromMe, chatOwnerColor);
                case ChatTypeEnum.video:
                    return FormatVideo(msg, isChatHeader, isKeyFromMe, chatOwnerColor);
                case ChatTypeEnum.videoReply:
                    return @"<li>video reply | temporarily do nothing</li>";
                case ChatTypeEnum.audio:
                    return @"<li>audio | temporarily do nothing</li>";
                case ChatTypeEnum.audioReply:
                    return @"<li>audio reply | temporarily do nothing</li>";
                default:
                    return @"<li style='color:red'> CHAT TYPE NOT CAUGHT!</li>";
                    //throw new InvalidOperationException("ChatTemplate must have a valid value as listed in ChatTypeEnum");
                    //to be logged
            }
        }

        static private string FormatNormalChat(TableRow_Messages msg, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
        {
            string liOpenTag;
            string liCloseTag;
            string divOpenTag;
            string divCloseTag;
            string chatOwner;
            string msgBody;
            string timestamp;
            string chatNameColor;

            chatOwnerColor.OwnerColorListing.TryGetValue(msg.remote_resource, out chatNameColor);

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
            
            if (isChatHeader && !isKeyFromMe)
                chatOwner = @"<strong " + "class='author" + chatNameColor + "'>" + msg.remote_resource + @"</strong><br/>";
            else
                chatOwner = "";

            msgBody = msg.Data;
            timestamp = @"<time><br/>" + FormatTimeStamp(msg.timestamp) + @"</time>";
            liCloseTag = @"</li>";
            divCloseTag = @"</div>";

            var htmlTemplate = 
                liOpenTag + 
                    divOpenTag + 
                        chatOwner + 
                        msgBody + 
                        timestamp + 
                    divCloseTag + 
                liCloseTag;

            return htmlTemplate;
        }

        static private string FormatNormalChatReply(TableRow_Messages msg, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
        {
            string liOpenTag;
            string liCloseTag;
            string divOpenTag1;
            string divOpenTag2;
            string divCloseTag;
            string chatOwner;
            string chatReplyOwner;
            string msgBody;
            string msgReplyBody;
            string timestamp;
            string chatNameColor;
            string replyNameColor;

            chatOwnerColor.OwnerColorListing.TryGetValue(msg.remote_resource, out chatNameColor);
            chatOwnerColor.OwnerColorListing.TryGetValue(msg.Table_Messsages_Quotes.remote_resource, out replyNameColor);

            //msg reply will always be in header mode, dont need to check for isChatHeader
            if (isKeyFromMe)
            {
                liOpenTag = @"<li class='self-header'>";
                divOpenTag1 = @"<div class='msg-bubble msg-self'>";
                divOpenTag2 = @"<div class='msg-reply-text-bubble-self quoted" + replyNameColor + "'>";
            }
            else
            {
                liOpenTag = @"<li class='others-header'>";
                divOpenTag1 = @"<div class='msg-bubble msg-others'>";
                divOpenTag2 = @"<div class='msg-reply-text-bubble-others quoted" + replyNameColor + "'>";
            }            

            if (!isKeyFromMe)
                chatOwner = @"<strong " + "class='author" + chatNameColor + "'>" + msg.remote_resource + @"</strong><br/>";
            else
                chatOwner = "";

            chatReplyOwner = @"<strong " + "class='author" + replyNameColor + "'>" + msg.Table_Messsages_Quotes.remote_resource + @"</strong><br/>";
            msgBody = msg.Data;
            msgReplyBody = msg.Table_Messsages_Quotes.Data;
            timestamp = @"<time><br/>" + FormatTimeStamp(msg.timestamp) + @"</time>";
            liCloseTag = @"</li>";
            divCloseTag = @"</div>";

            var htmlTemplate =
                liOpenTag +
                    divOpenTag1 +
                        chatOwner +
                        divOpenTag2 + 
                            chatReplyOwner + 
                            msgReplyBody + 
                        divCloseTag + 
                        msgBody +
                        timestamp +
                    divCloseTag +
                liCloseTag;

            return htmlTemplate;
        }        

        static private string FormatImage(TableRow_Messages msg, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
        {
            string liOpenTag;
            string liCloseTag;
            string divOpenTag;
            string divCloseTag;
            string imgTag;
            string chatOwner;
            string caption;
            string timestamp;
            string chatNameColor;

            chatOwnerColor.OwnerColorListing.TryGetValue(msg.remote_resource, out chatNameColor);

            if (isChatHeader && isKeyFromMe)
            {
                liOpenTag = @"<li class='self-header'>";
                divOpenTag = @"<div class='img-container msg-self'>";
            }
            else if (isChatHeader && !isKeyFromMe)
            {
                liOpenTag = @"<li class='others-header'>";  
                divOpenTag = @"<div class='img-container msg-others'>";
            }
            else if (!isChatHeader && isKeyFromMe)
            {
                liOpenTag = @"<li class='self'>";
                divOpenTag = @"<div class='img-container msg-self'>";
            }
            else
            {
                liOpenTag = @"<li class='others'>";
                divOpenTag = @"<div class='img-container msg-others'>";
            }
            
            if (isChatHeader && !isKeyFromMe)
                chatOwner = @"<strong " + "class='author" + chatNameColor + "'>" + msg.remote_resource + @"</strong><br/>";
            else
                chatOwner = "";            

            imgTag = @"<img src='../Content/IMG/image1.jpg' />";
            caption = msg.media_caption;
            timestamp = @"<time><br/>" + FormatTimeStamp(msg.timestamp) + @"</time>";
            liCloseTag = @"</li>";
            divCloseTag = @"</div>";

            var htmlTemplate =
                liOpenTag +
                    divOpenTag +
                        chatOwner + 
                        imgTag + 
                        caption +
                        timestamp +
                    divCloseTag +
                liCloseTag;

            return htmlTemplate;
        }

        static private string FormatImageReply(TableRow_Messages msg, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
        {
            string liOpenTag;
            string liCloseTag;
            string divOpenTag1;
            string divOpenTag2;
            string divOpenTag3;
            string divOpenTag4;
            string divCloseTag;
            string imgTag;
            string chatOwner;
            string chatReplyOwner;
            string msgBody;
            string caption;
            string timestamp;
            string chatNameColor;
            string replyNameColor;

            chatOwnerColor.OwnerColorListing.TryGetValue(msg.remote_resource, out chatNameColor);
            chatOwnerColor.OwnerColorListing.TryGetValue(msg.Table_Messsages_Quotes.remote_resource, out replyNameColor);

            //msg reply will always be in header mode, dont need to check for isChatHeader
            if (isKeyFromMe)
            {
                liOpenTag = @"<li class='self-header'>";
                divOpenTag1 = @"<div class='msg-bubble msg-self'>";
                divOpenTag2 = @"<div class='msg-reply-image-bubble-self quoted" + replyNameColor + "'>";
            }
            else
            {
                liOpenTag = @"<li class='others-header'>";
                divOpenTag1 = @"<div class='msg-bubble msg-others'>";
                divOpenTag2 = @"<div class='msg-reply-image-bubble-others quoted" + replyNameColor + "'>";
            }

            if (!isKeyFromMe)
                chatOwner = @"<strong>" + msg.remote_resource + @"</strong><br/>";
            else
                chatOwner = "";

            chatReplyOwner = @"<strong " + "class='author" + replyNameColor + "'>" + msg.Table_Messsages_Quotes.remote_resource + @"</strong><br/>";            
            divOpenTag3 = @"<div>";
            divOpenTag4 = @"<div class='image-reply-container'>";
            imgTag = @"<img src='../Content/IMG/image1.jpg' class='image-reply' />";
            caption = msg.Table_Messsages_Quotes.media_caption;
            msgBody = msg.Data;
            timestamp = @"<time><br/>" + FormatTimeStamp(msg.timestamp) + @"</time>";
            liCloseTag = @"</li>";
            divCloseTag = @"</div>";

            var htmlTemplate =
                liOpenTag +
                    divOpenTag1 +
                        chatOwner +
                        divOpenTag2 + 
                            divOpenTag3 + 
                                chatReplyOwner +
                                caption + 
                            divCloseTag + 
                            divOpenTag4 + 
                                imgTag + 
                            divCloseTag + 
                        divCloseTag +
                        msgBody +
                        timestamp +
                    divCloseTag +
                liCloseTag;

            return htmlTemplate;
        }

        static private string FormatVideo(TableRow_Messages msg, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
        {
            string liOpenTag;
            string liCloseTag;
            string divOpenTag;
            string divCloseTag;
            string videoTag;
            string chatOwner;
            string caption;
            string timestamp;
            string chatNameColor;

            chatOwnerColor.OwnerColorListing.TryGetValue(msg.remote_resource, out chatNameColor);

            //video chat is always in header mode
            if (isKeyFromMe)
            {
                liOpenTag = @"<li class='self-header'>";
                divOpenTag = @"<div class='video-container msg-self'>";
            }
            else
            {
                liOpenTag = @"<li class='others-header'>";
                divOpenTag = @"<div class='video-container msg-others'>";
            }
            
            if (!isKeyFromMe)
                chatOwner = @"<strong " + "class='author" + chatNameColor + "'>" + msg.remote_resource + @"</strong><br/>";
            else
                chatOwner = "";
            
            videoTag = @"<video controls><source src='../Content/Videos/video1.mp4' type='video/mp4' />Video format not supported</video>";
            caption = msg.media_caption;
            timestamp = @"<time><br/>" + FormatTimeStamp(msg.timestamp) + @"</time>";
            liCloseTag = @"</li>";
            divCloseTag = @"</div>";

            var htmlTemplate =
                liOpenTag +
                    divOpenTag +
                        chatOwner +
                        videoTag +
                        caption +
                        timestamp +
                    divCloseTag +
                liCloseTag;

            return htmlTemplate;
        }

        static private string FormatTimeStamp(long unixTimeStamp)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dt = dt.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dt.ToShortTimeString();
        }
    }
}