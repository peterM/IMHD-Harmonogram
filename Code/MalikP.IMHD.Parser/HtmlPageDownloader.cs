using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MalikP.IMHD.Parser
{
    public static class HtmlPageDownloader
    {
        public static string DownloadHtmlPage(string url)
        {
            string response = null;

            var request = WebRequest.Create(url);
            request.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
            var webResponse = request.GetResponse();
            using (var stream = webResponse.GetResponseStream())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                response = Encoding.UTF8.GetString(ms.ToArray());
            }

            return response;
        }

    }
}
