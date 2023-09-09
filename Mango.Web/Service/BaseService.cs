using Mango.Web.Models;
using Mango.Web.Models.DTO;
using Mango.Web.Service.IService;

namespace Mango.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseDTO> SendAsync(RequestDTO requestDTO)
        {
            throw new NotImplementedException();
        }
    }
}
