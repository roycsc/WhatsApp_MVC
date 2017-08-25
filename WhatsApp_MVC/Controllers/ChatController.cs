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
        private List<Messages> _messagesList = new List<Messages>();

        // GET: Chat
        public ActionResult Index()
        {
            using (_sqliteConnection)
            {
                try
                { _sqliteConnection.Open(); }
                catch (Exception e)
                { Console.WriteLine(e.Message); }

                var sqlCommand = _sqliteConnection.CreateCommand();

                sqlCommand.CommandText = "SELECT * FROM messages WHERE data IS NOT NULL OR media_enc_hash IS NOT NULL";

                _sqliteReader = sqlCommand.ExecuteReader();

                while (_sqliteReader.Read())
                {
                    var values = _sqliteReader.GetValues();
                    var messageRow = new Messages();

                    messageRow.Id = Convert.ToInt32(values["_id"]);
                    messageRow.key_remote_jid = values["key_remote_jid"];
                    messageRow.key_from_me = Convert.ToBoolean(values["key_from_me"]);
                    messageRow.Data = values["data"];
                    messageRow.timestamp = Convert.ToInt64(values["timestamp"]);
                    messageRow.mediam_mime_type = values["mediam_mime_type"];
                    messageRow.media_size = Convert.ToInt64(values["media_size"]);
                    messageRow.media_name = values["media_name"];
                    messageRow.media_caption = values["media_caption"];
                    messageRow.latitude = values["latitude"];
                    messageRow.longtitude = values["longtitude"];
                    messageRow.quoted_row_id = Convert.ToInt32(values["quoted_row_id"]);

                    _messagesList.Add(messageRow);
                }
            }

            return View();
        }

        //class to test sql validity, url syntax: localhost:xxxxx/chat/sqltest
        public string sqlTest()
        {
            //insert sql - START
            using (_sqliteConnection)
            {
                try
                { _sqliteConnection.Open(); }
                catch (Exception e)
                { Console.WriteLine(e.Message); }

                var sqlCommand = _sqliteConnection.CreateCommand();

                sqlCommand.CommandText = "SELECT * FROM messages WHERE data IS NOT NULL OR media_enc_hash IS NOT NULL";

                _sqliteReader = sqlCommand.ExecuteReader();

                while (_sqliteReader.Read())
                {
                    var values = _sqliteReader.GetValues();
                    var messageRow = new Messages();

                    messageRow.Id = Convert.ToInt32(values["_id"]);
                    messageRow.key_remote_jid = values["key_remote_jid"];
                    messageRow.key_from_me = Convert.ToBoolean(values["key_from_me"]);
                    messageRow.Data = values["data"];
                    messageRow.timestamp = Convert.ToInt64(values["timestamp"]);
                    messageRow.mediam_mime_type = values["mediam_mime_type"];
                    messageRow.media_size = Convert.ToInt64(values["media_size"]);
                    messageRow.media_name = values["media_name"];
                    messageRow.media_caption = values["media_caption"];
                    messageRow.latitude = values["latitude"];
                    messageRow.longtitude = values["longtitude"];
                    messageRow.quoted_row_id = Convert.ToInt32(values["quoted_row_id"]);
                    messageRow.media_enc_hash = values["media_ench_hash"];

                    _messagesList.Add(messageRow);
                }
            }
            //insert sql - END

            var temp = "";
            foreach (var item in _messagesList)
            {
                temp += item.Id + "   " + item.Data + "   " + item.media_enc_hash + "<br/>";
            }

            return temp;
        }
    }
}