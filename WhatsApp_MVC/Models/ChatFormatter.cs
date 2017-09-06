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
            bool isChatHeader; //chatheader means the 1st conversation for a continuous msg from the same person, purpose is to display their name in the bubble
            bool isQuotedMsg;
            ChatTypeEnum chatType;

            for (int i = 0; i < msg.Count; i++)
            {
                isKeyFromMe = msg[i].key_from_me;
                chatOwner = msg[i].remote_resource;

                if (!isKeyFromMe && i > 0)
                {
                    isChatHeader = (msg[i].remote_resource == msg[i - 1].remote_resource) ? false : true; //check if current chat same owner as previous chat
                }
                else if (isKeyFromMe)
                {
                    isChatHeader = false;
                }
                else if (!isKeyFromMe && i == 0)
                {
                    isChatHeader = true;
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

                chatViewModel.FormattedMessage.Add(ChatTemplateSelector(msg[i], chatType, isChatHeader, isKeyFromMe, chatOwnerColor));
            }
        }

        static private string ChatTemplateSelector(TableRow_Messages msg, ChatTypeEnum chatType, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
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
            string liClass;
            string divClass;
            string chatOwner;
            string chatNameColor;

            chatOwnerColor.OwnerColorListing.TryGetValue(msg.remote_resource, out chatNameColor);

            if (isChatHeader && isKeyFromMe)
            {
                liClass = "self-header";
                divClass = "msg-bubble msg-self";  
            }
            else if (isChatHeader && !isKeyFromMe)
            {
                liClass = "others-header";
                divClass = "msg-bubble msg-others";
            }
            else if (!isChatHeader && isKeyFromMe)
            {
                liClass = "self";
                divClass = "msg-bubble msg-self";
            }
            else
            {
                liClass = "others";
                divClass = "msg-bubble msg-others";
            }

            if (isChatHeader && !isKeyFromMe)
                chatOwner = $@"<strong class='author{chatNameColor}'>{msg.remote_resource}</strong><br/>";
            else
                chatOwner = "";
            
            var htmlTemplate = $@"<li class='{liClass}'>
                                    <div class='{divClass}'>
                                        {chatOwner}
                                        {msg.Data}
                                        <time><br/>{FormatTimeStamp(msg.timestamp)}</time>
                                    </div>
                                </li>";

            return htmlTemplate;
        }

        static private string FormatNormalChatReply(TableRow_Messages msg, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
        {
            string liClass;
            string divClass1;
            string divClass2;
            string chatOwner;
            string chatNameColor;
            string replyNameColor;

            chatOwnerColor.OwnerColorListing.TryGetValue(msg.remote_resource, out chatNameColor);
            chatOwnerColor.OwnerColorListing.TryGetValue(msg.Table_Messsages_Quotes.remote_resource, out replyNameColor);

            //msg reply will always be in header mode, dont need to check for isChatHeader
            if (isKeyFromMe)
            {
                liClass = "self-header";
                divClass1 = "msg-bubble msg-self";
                divClass2 = "msg-reply-text-bubble-self quoted";
            }
            else
            {
                liClass = "others-header";
                divClass1 = "msg-bubble msg-others";
                divClass2 = "msg-reply-text-bubble-others quoted";
            }

            if (!isKeyFromMe)
                chatOwner = $@"<strong class='author{chatNameColor}'>{msg.remote_resource}</strong><br/>";
            else
                chatOwner = "";

            var htmlTemplate = $@"<li class='{liClass}'>
                                    <div class='{divClass1}'>
                                        {chatOwner}
                                        <div class='{divClass2}{replyNameColor}'>
                                            <strong class='author{replyNameColor}'>{msg.Table_Messsages_Quotes.remote_resource}</strong><br/>
                                            {msg.Table_Messsages_Quotes.Data}
                                        </div>
                                        {msg.Data}
                                        <time><br/>{FormatTimeStamp(msg.timestamp)}</time>
                                    </div>
                                </li>";

            return htmlTemplate;
        }        

        static private string FormatImage(TableRow_Messages msg, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
        {
            string liClass;
            string divClass;
            string chatOwner;
            string chatNameColor;

            chatOwnerColor.OwnerColorListing.TryGetValue(msg.remote_resource, out chatNameColor);

            if (isChatHeader && isKeyFromMe)
            {
                liClass = "self-header";
                divClass = "img-container msg-self";
            }
            else if (isChatHeader && !isKeyFromMe)
            {
                liClass = "others-header";  
                divClass = "img-container msg-others";
            }
            else if (!isChatHeader && isKeyFromMe)
            {
                liClass = "self";
                divClass = "img-container msg-self";
            }
            else
            {
                liClass = "others";
                divClass = "img-container msg-others";
            }

            if (isChatHeader && !isKeyFromMe)
                chatOwner = $@"<strong class='author{chatNameColor}'>{msg.remote_resource}</strong><br/>";
            else
                chatOwner = "";            
            
            var htmlTemplate = $@"<li class='{liClass}'>
                                    <div class='{divClass}'>
                                        {chatOwner}
                                        <img src='../Content/IMG/image1.jpg' />
                                        {msg.media_caption}
                                        <time><br/>{FormatTimeStamp(msg.timestamp)}</time>
                                    </div>
                                </li>";

            return htmlTemplate;
        }

        static private string FormatImageReply(TableRow_Messages msg, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
        {
            string liClass;
            string divClass1;
            string divClass2;
            string chatOwner;
            string chatNameColor;
            string replyNameColor;

            chatOwnerColor.OwnerColorListing.TryGetValue(msg.remote_resource, out chatNameColor);
            chatOwnerColor.OwnerColorListing.TryGetValue(msg.Table_Messsages_Quotes.remote_resource, out replyNameColor);

            //msg reply will always be in header mode, dont need to check for isChatHeader
            if (isKeyFromMe)
            {
                liClass = "self-header";
                divClass1 = "msg-bubble msg-self";
                divClass2 = "msg-reply-image-bubble-self quoted";
            }
            else
            {
                liClass = "others-header";
                divClass1 = "msg-bubble msg-others";
                divClass2 = "msg-reply-image-bubble-others quoted";
            }

            if (!isKeyFromMe)
                chatOwner = $@"<strong class='author{chatNameColor}'>{msg.remote_resource}</strong><br/>";
            else
                chatOwner = "";
            
            var htmlTemplate = $@"<li class='{liClass}'>
                                    <div class='{divClass1}'>
                                        {chatOwner}
                                        <div class='{divClass2}{replyNameColor}'>
                                            <div>
                                                <strong class='author{replyNameColor}'>{msg.Table_Messsages_Quotes.remote_resource}</strong><br/>
                                                {msg.Table_Messsages_Quotes.media_caption}
                                            </div>
                                            <div class='image-reply-container'>
                                                <img src='../Content/IMG/image1.jpg' class='image-reply' />
                                            </div>
                                        </div>
                                        {msg.Data}
                                        <time><br/>{FormatTimeStamp(msg.timestamp)}</time>
                                    </div>
                                </li>";

            return htmlTemplate;
        }

        static private string FormatVideo(TableRow_Messages msg, bool isChatHeader, bool isKeyFromMe, ColorPicker chatOwnerColor)
        {
            string liClass;
            string divClass;
            string chatOwner;
            string chatNameColor;

            chatOwnerColor.OwnerColorListing.TryGetValue(msg.remote_resource, out chatNameColor);

            //video chat is always in header mode
            if (isKeyFromMe)
            {
                liClass = "self-header";
                divClass = "video-container msg-self";
            }
            else
            {
                liClass = "others-header";
                divClass = "video-container msg-others";
            }

            if (!isKeyFromMe)
                chatOwner = $@"<strong class='author{chatNameColor}'>{msg.remote_resource}</strong><br/>";
            else
                chatOwner = "";

            var htmlTemplate = $@"<li class='{liClass}'>
                                    <div class='{divClass}'>
                                        {chatOwner}
                                        <video controls><source src='../Content/Videos/video1.mp4' type='video/mp4' />Video format not supported</video>
                                        {msg.media_caption}
                                        <time><br/>{FormatTimeStamp(msg.timestamp)}</time>
                                    </div>
                                </li>";

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