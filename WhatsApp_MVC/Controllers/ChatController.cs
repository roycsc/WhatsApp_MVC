using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SQLite;
using WhatsApp_MVC.Models;

namespace WhatsApp_MVC.Controllers
{
    public class ChatController : Controller
    {
        private SQLiteConnection _sqliteConnection = 
            new SQLiteConnection("Data Source=|DataDirectory|msgstore.db;Version=3");
        private SQLiteDataReader _sqliteReader;
        private List<TableRow_Messages> _messagesList = new List<TableRow_Messages>();
        private List<TableRow_Messages_Quotes> _messagesQuotesList = new List<TableRow_Messages_Quotes>();
        private ChatViewModel _chatViewModel = new ChatViewModel();
        private ColorPicker _chatOwnerColor = new ColorPicker();

        // GET: Chat
        public ActionResult Index()
        {
            using (_sqliteConnection)
            {
                try
                { _sqliteConnection.Open(); }
                catch (Exception e)
                { Console.WriteLine(e.Message); }

                using (var sqlCommand = _sqliteConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "SELECT * FROM messages WHERE data IS NOT NULL OR (media_mime_type IS NOT NULL AND media_enc_hash IS NOT NULL)";
                    _sqliteReader = sqlCommand.ExecuteReader();

                    //messages table
                    while (_sqliteReader.Read())
                    {
                        var values = _sqliteReader.GetValues();
                        var messageRow = new TableRow_Messages();

                        messageRow._id = Convert.ToInt32(values["_id"]);
                        {
                            int temp;
                            Int32.TryParse(values["key_from_me"], out temp);
                            messageRow.key_from_me = temp != 0;
                        }
                        messageRow.key_id = values["key_id"];
                        messageRow.Data = values["data"];
                        messageRow.timestamp = Convert.ToInt64(values["timestamp"]);
                        messageRow.media_mime_type = values["media_mime_type"];
                        messageRow.media_size = Convert.ToInt64(values["media_size"]);
                        messageRow.media_name = values["media_name"];
                        messageRow.media_caption = values["media_caption"];
                        messageRow.latitude = values["latitude"];
                        messageRow.longtitude = values["longtitude"];
                        messageRow.remote_resource = values["remote_resource"];
                        {
                            Int32 tempQuotedRowId;
                            Int32.TryParse(values["quoted_row_id"], out tempQuotedRowId);
                            messageRow.quoted_row_id = tempQuotedRowId;
                        }

                        _messagesList.Add(messageRow);
                    }
                }

                using (var sqlCommand = _sqliteConnection.CreateCommand())
                {
                    sqlCommand.CommandText = "SELECT * FROM messages_quotes WHERE data IS NOT NULL OR (media_mime_type IS NOT NULL AND media_enc_hash IS NOT NULL)";
                    _sqliteReader = sqlCommand.ExecuteReader();

                    //meesages_quotes table
                    while (_sqliteReader.Read())
                    {
                        var values = _sqliteReader.GetValues();
                        var messageRow = new TableRow_Messages_Quotes();

                        messageRow._id = Convert.ToInt32(values["_id"]);
                        {
                            int temp;
                            Int32.TryParse(values["key_from_me"], out temp);
                            messageRow.key_from_me = temp != 0;
                        }
                        messageRow.key_id = values["key_id"];
                        messageRow.Data = values["data"];
                        messageRow.timestamp = Convert.ToInt64(values["timestamp"]);
                        messageRow.media_mime_type = values["media_mime_type"];
                        messageRow.media_size = Convert.ToInt64(values["media_size"]);
                        messageRow.media_name = values["media_name"];
                        messageRow.media_caption = values["media_caption"];
                        messageRow.latitude = values["latitude"];
                        messageRow.longtitude = values["longtitude"];
                        messageRow.remote_resource = values["remote_resource"];

                        _messagesQuotesList.Add(messageRow);
                    }
                }                
            }

            for (int i = 0; i < _messagesQuotesList.Count; i++)
            {
                //currently theres a bug here, as I'm using remote resource field, some of them is empty, even if it belongs to someone else
                //so those empty fields which belongs to someone else will be named as "You" too...
                if (string.IsNullOrWhiteSpace(_messagesQuotesList[i].remote_resource))
                {
                    _messagesQuotesList[i].remote_resource = "You";
                }
            }

            //chat name coloring section
            int index = 0;
            foreach (var item in _messagesList)
            {
                if (!string.IsNullOrWhiteSpace(item.remote_resource) && !_chatOwnerColor.OwnerColorListing.ContainsKey(item.remote_resource))
                {
                    index = (index > 18) ? 2 : index;
                    _chatOwnerColor.OwnerColorListing.Add(item.remote_resource, _chatOwnerColor.CssColorList[index]);
                    index++;
                }
            }            

            ChatFormatter.FormatChat(_chatViewModel, _messagesList, _messagesQuotesList, _chatOwnerColor);

            return View(_chatViewModel);
        }

        //class to test sql validity, url syntax: localhost:xxxxx/chat/sqltest
        public string SqlTest()
        {
            //insert sql - START
            
            //insert sql - END

            var temp = "";
            //foreach (var item in _messagesQuotesList)
            //{
            //    temp += item.Id + "   " + item.Data + " | " + "<br/>";
            //}

            return temp;
        }
    }
}