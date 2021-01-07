using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Maslomat
{
    public static class Global
    {
        public static string ApiUrl { get; } = "https://lab1-iot-jankowiak-2.azurewebsites.net/users";

        public static HttpClientHandler HttpClientHandler { get; } = new HttpClientHandler();
    }
}
