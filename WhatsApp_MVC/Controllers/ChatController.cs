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
        private ChatViewModel chatViewModel = new ChatViewModel();

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
                    sqlCommand.CommandText = "SELECT * FROM messages WHERE data IS NOT NULL OR media_mime_type IS NOT NULL";
                    _sqliteReader = sqlCommand.ExecuteReader();

                    //messages table
                    while (_sqliteReader.Read())
                    {
                        var values = _sqliteReader.GetValues();
                        var messageRow = new TableRow_Messages();

                        messageRow.Id = Convert.ToInt32(values["_id"]);
                        messageRow.key_remote_jid = values["key_remote_jid"];
                        {
                            int tempKeyFromMe;
                            Int32.TryParse(values["key_from_me"], out tempKeyFromMe);
                            messageRow.key_from_me = tempKeyFromMe != 0;
                        }
                        messageRow.key_id = values["key_id"];
                        messageRow.Data = values["data"];
                        messageRow.timestamp = Convert.ToInt64(values["timestamp"]);
                        messageRow.mediam_mime_type = values["mediam_mime_type"];
                        messageRow.media_size = Convert.ToInt64(values["media_size"]);
                        messageRow.media_name = values["media_name"];
                        messageRow.media_caption = values["media_caption"];
                        messageRow.latitude = values["latitude"];
                        messageRow.longtitude = values["longtitude"];
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
                    sqlCommand.CommandText = "SELECT * FROM messages_quotes WHERE data IS NOT NULL OR media_mime_type IS NOT NULL";
                    _sqliteReader = sqlCommand.ExecuteReader();

                    //meesages_quotes table
                    while (_sqliteReader.Read())
                    {
                        var values = _sqliteReader.GetValues();
                        var messageRow = new TableRow_Messages_Quotes();

                        messageRow.Id = Convert.ToInt32(values["_id"]);
                        messageRow.key_remote_jid = values["key_remote_jid"];
                        {
                            int tempKeyFromMe;
                            Int32.TryParse(values["key_from_me"], out tempKeyFromMe);
                            messageRow.key_from_me = tempKeyFromMe != 0;
                        }
                        messageRow.key_id = values["key_id"];
                        messageRow.Data = values["data"];
                        messageRow.timestamp = Convert.ToInt64(values["timestamp"]);
                        messageRow.mediam_mime_type = values["mediam_mime_type"];
                        messageRow.media_size = Convert.ToInt64(values["media_size"]);
                        messageRow.media_name = values["media_name"];
                        messageRow.media_caption = values["media_caption"];
                        messageRow.latitude = values["latitude"];
                        messageRow.longtitude = values["longtitude"];

                        _messagesQuotesList.Add(messageRow);
                    }
                }                
            }
            
            ChatFormatter.FormatChat(chatViewModel, _messagesList, _messagesQuotesList);

            return View(chatViewModel);
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