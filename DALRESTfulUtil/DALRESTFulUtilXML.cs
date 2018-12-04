using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Xml.Serialization;
using DALRESTfulUtil.Logging;


namespace DALRESTfulUtil.HttpClientXML
{
    /**
* http://stackoverflow.com/questions/2546138/deserializing-json-data-to-c-sharp-using-json-net 
* https://jsonclassgenerator.codeplex.com/downloads/get/631627
* http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
**/

    //This Class is not used but is a kind a helper class used when do bebugging
    public class XMLHelper
    {
        public static string SerializeToString(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }
    }

    //This Class is not used but is a kind a helper class used when do bebugging
    public static class HttpExtensions
    {
        public static Task<HttpResponseMessage> PostAsXmlWithSerializerAsync<T>(this HttpClient client, Uri requestUri, T value)
        {
            return client.PostAsync(requestUri, value, new XmlMediaTypeFormatter { UseXmlSerializer = true });
        }

        public static Task<HttpResponseMessage> PutAsXmlWithSerializerAsync<T>(this HttpClient client, Uri requestUri, T value)
        {
            return client.PutAsync(requestUri, value, new XmlMediaTypeFormatter { UseXmlSerializer = true });
        }
    }


    public class APIDeleteXML<T>
    {
        public T data;
        private string url;
        private string request;
        async Task RunConsumer()
        {
            data = await deleteItem(data);
        }

        public APIDeleteXML(string rurl, string req, T item)
        {
            data = item;
            request = req;
            url = rurl;
            RunConsumer().Wait();
        }

        private async Task<T> deleteItem(T item)
        {
            T result = default(T);
            result = item;
            HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            var xmlf = new XmlMediaTypeFormatter();
            xmlf.UseXmlSerializer = true;
            var formatters = new List<MediaTypeFormatter>() { xmlf };

            using (HttpResponseMessage response = client.DeleteAsync(request).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    string resp = response.Content.ReadAsStringAsync().Result; //Only a inspection line for the raw response
                    result = await response.Content.ReadAsAsync<T>(formatters);
                }
            }
            return result;
        }
    }
    
    public class APIGetXML<T>
    {
        public T data;
        private string url;
        async Task RunConsumer()
        {
            data = await getItems();
        }

        public APIGetXML(string rurl)
        {
            url = rurl;
            RunConsumer().Wait();
        }
        private async Task<T> getItems()
        {
            T result = default(T);
            HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            //Setting up the formatter to use XML Serialization instead of Data Contract 
            //to ensure that the Edit->Paste Special-> Paste XML as Classes just work 
            //as this tool uses XML Serialization! An possible error could be like the linebelow
            //{ "Der er fejl i linje 1 tegn 135. Forventer elementet ArrayOfAuthor fra navneområdet http://schemas.datacontract.org/2004/07/DAlHttpClientJsonXML.. Element med navnet ArrayOfAuthor i navneområdet http://schemas.datacontract.org/2004/07/BookService.Models blev fundet. "}

            var xmlf = new XmlMediaTypeFormatter();
            xmlf.UseXmlSerializer = true;
            var formatters = new List<MediaTypeFormatter>() { xmlf };

            using (HttpResponseMessage response = client.GetAsync(url).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    string res = response.Content.ReadAsStringAsync().Result;//Only a inspection line for the raw response
                    result = await response.Content.ReadAsAsync<T>(formatters);
                }
                return result;
            }
        }
    }

    public class APIPutXML<T>
    {
        public T data;
        private string url;
        private string request;
        async Task RunConsumer()
        {
            data = await putItem(data);
        }

        public APIPutXML(string rurl, string req, T item)
        {
            data = item;
            request = req;
            url = rurl;
            RunConsumer().Wait();
        }
        private async Task<T> putItem(T item)
        {
            T result = default(T);
            result = item;
            HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
            client.BaseAddress = new Uri(url + request);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            var xmlf = new XmlMediaTypeFormatter() { UseXmlSerializer = true };
            var formatters = new List<MediaTypeFormatter>() {
                            new JsonMediaTypeFormatter() {UseDataContractJsonSerializer  = true },
                            xmlf };

            using (HttpResponseMessage response = client.PutAsync(client.BaseAddress, item, xmlf).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<T>(formatters);
                }
            }
            return result;
        }
    }

 

    public class APIPostXML<T>
    {
        public T data;
        private string url;
        private string request;
        async Task RunConsumer()
        {
            data = await postItem(data);
        }

        public APIPostXML(string rurl, string req, T item)
        {
            data = item;
            request = req;
            url = rurl;
            RunConsumer().Wait();
        }

        private async Task<T> postItem(T item)
        {
            T result = default(T);
            result = item;
            HttpClient client = new HttpClient(new LoggingHandler(new HttpClientHandler()));
            client.BaseAddress = new Uri(url + request);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            //String mes = XMLHelper.SerializeToString(item);
            var xmlf = new XmlMediaTypeFormatter();
            xmlf.UseXmlSerializer = true;
            var formatters = new List<MediaTypeFormatter>() { xmlf };

            using (HttpResponseMessage response = client.PostAsync(client.BaseAddress, item, xmlf).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsAsync<T>(formatters);
                }
                return result;
            }
        }
    }

  

}
