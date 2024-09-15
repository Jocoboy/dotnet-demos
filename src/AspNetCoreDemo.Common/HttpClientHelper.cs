using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreDemo.Common
{
    public class HttpClientHelper
    {
        private static readonly object LockObj = new object();
        private static HttpClient _httpClient = null;

        public HttpClientHelper()
        {
            GetInstance();
        }

        public static HttpClient GetInstance()
        {

            if (_httpClient == null)
            {
                lock (LockObj)
                {
                    if (_httpClient == null)
                    {
                        _httpClient = new HttpClient();
                    }
                }
            }
            return _httpClient;
        }

        public string Get(string url)
        {
            try
            {
                var response =  _httpClient.GetStringAsync(url);
                return response.Result;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public async Task<string> GetAsync(string url)
        {
            try
            {
                var res = await _httpClient.GetStringAsync(url);
                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string Post(string url, string postDataStr, string contentType = "application/json")
        {
            try
            {
                HttpContent content = new StringContent(postDataStr);
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                Task<HttpResponseMessage> response = _httpClient.PostAsync(url, content);
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    string res = response.Result.Content.ReadAsStringAsync().Result;
                    return res;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<string> PostAsync(string url, string postDataStr, string contentType = "application/json")
        {
            try
            {
                HttpContent content = new StringContent(postDataStr);
                content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string res = response.Content.ReadAsStringAsync().Result;
                    return res;
                }
                else
                {
                    return null;
                }
            
            }
            catch (Exception)
            {
                return null;
            }
        }

    }
}
