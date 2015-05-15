using frauddetect.common.core.convertor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace frauddetect.common.core.web
{
    public sealed class WebManager
    {
        public Tout POSTMethod<Tin, Tout>(string url, Tin input)
        {
            HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = "POST";
            webRequest.ContentType = "application/json;";
            webRequest.Timeout = 1000 * 60;

            string data = new Convertor().SerializeToJSON(input, typeof(Tin));
            byte[] reqData = Encoding.UTF8.GetBytes(data);

            webRequest.ContentLength = reqData.Length;

            Stream stream = webRequest.GetRequestStream();
            stream.Write(reqData, 0, reqData.Length);

            string responseString = null;

            using (HttpWebResponse webResponse = webRequest.GetResponse() as HttpWebResponse)
            {
                if (webResponse.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        responseString = streamReader.ReadToEnd();
                    }
                }
            }

            return new Convertor().DeserializeFromJSON<Tout>(responseString);
        }
    }
}
