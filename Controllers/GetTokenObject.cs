﻿namespace CodeConverterTool.Controllers
{
    public class GetTokenObject
    {
        public string device_code { get; set; }
        public string user_code { get; set; }
        public string verification_uri { get; set; }
        public int expires_in { get; set; }
        public int interval { get; set; }
        public string verification_uri_complete { get; set; }
    }
}
