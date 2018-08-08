using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;

namespace OCRApp
{

    public class OCRAPI
    {
        // TODO SUB KEY
        const string Key = "d994912f1378493092799049c6bdd228";

        // TODO 请求地址
        const string uriBase =
            "https://eastasia.api.cognitive.microsoft.com/vision/v2.0/ocr";

        
        public static async Task<RootObject> OCRRequests(StorageFile image)
        {

            //请求
            HttpClient client = new HttpClient();

         
            client.DefaultRequestHeaders.Add(
                "Ocp-Apim-Subscription-Key", Key);

        
            string requestParameters = "language=zh-Hans&detectOrientation=true";

           
            string uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

           //转Byte[]
            byte[] byteData = await GetBytesAsync(image);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
              
                content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");

               
                response = await client.PostAsync(uri, content);
            }

        
            string contentString = await response.Content.ReadAsStringAsync();

            //JSON序列化
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(RootObject));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(result));

            return (RootObject)serializer.ReadObject(ms);



            
        }

      //StrogeFile 转 Byte[]
        public static async Task<byte[]> GetBytesAsync(StorageFile file)
        {
            byte[] fileBytes = null;
            if (file == null) return null;
            using (var stream = await file.OpenReadAsync())
            {
                fileBytes = new byte[stream.Size];
                using (var reader = new DataReader(stream))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    reader.ReadBytes(fileBytes);
                }
            }
            return fileBytes;
        }

    }

 
    //序列化封装
    [DataContract]
    public class Word
    {
        [DataMember]
        public string boundingBox { get; set; }

        [DataMember]
        public string text { get; set; }
    }

    [DataContract]
    public class Line
    {
        [DataMember]
        public string boundingBox { get; set; }

        [DataMember]
        public List<Word> words { get; set; }
    }

    [DataContract]
    public class Region
    {
        [DataMember]
        public string boundingBox { get; set; }

        [DataMember]
        public List<Line> lines { get; set; }
    }

    [DataContract]
    public class RootObject
    {
        [DataMember]
        public string language { get; set; }

        [DataMember]
        public string orientation { get; set; }

        [DataMember]
        public double textAngle { get; set; }

        [DataMember]
        public List<Region> regions { get; set; }
    }
}
