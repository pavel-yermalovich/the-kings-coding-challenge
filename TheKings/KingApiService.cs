using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TheKings
{
    public class KingApiService : IKingApiService
    {
        private readonly string _apiUrl;

        public KingApiService(string apiUrl)
        {
            _apiUrl = apiUrl;
        }

        public IEnumerable<King> GetKings()
        {
            var kings = new List<ApiKing>();

            using (var client = new WebClient())
            {
                var content = client.DownloadString(_apiUrl);

                var serializer = new DataContractJsonSerializer(typeof(List<ApiKing>));
                using (var memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(content)))
                {
                    kings = (List<ApiKing>)serializer.ReadObject(memoryStream);
                }
            }

            var result = kings.Select(dto => new King(dto));
            return result;
        }
    }
}
