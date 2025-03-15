using System.Text.Json;

namespace FoodReserve.OutletPortal.Extensions
{
    public class JsonSerializerOptionsHandler : DelegatingHandler
    {
        public JsonSerializerOptions Options { get; set; } = new JsonSerializerOptions();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content is StringContent stringContent)
            {
                var content = await stringContent.ReadAsStringAsync(cancellationToken);
                var contentType = stringContent.Headers.ContentType?.MediaType;
                
                if (contentType == "application/json")
                {
                    request.Content = new StringContent(
                        content,
                        stringContent.Headers.ContentType!.CharSet != null ? 
                            System.Text.Encoding.GetEncoding(stringContent.Headers.ContentType.CharSet) : 
                            System.Text.Encoding.UTF8,
                        contentType);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
