﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatsApp_MVC.Models
{
    public class TableRow_Messages_Quotes
    {
        public int _id { get; set; }
        public bool key_from_me { get; set; }
        public string key_id { get; set; }
        public string Data { get; set; }
        public long timestamp { get; set; }
        public string media_mime_type { get; set; }
        public long media_size { get; set; }
        public string media_name { get; set; }
        public string media_caption { get; set; }
        public string latitude { get; set; }
        public string longtitude { get; set; }
        public string remote_resource { get; set; }
    }
}