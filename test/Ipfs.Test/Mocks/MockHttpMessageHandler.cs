using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ipfs.Test.Mocks
{    
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _toReturn;
        private readonly TimeSpan _delayBeforeResponse;

        public HttpRequestMessage LastRequest { get; private set; }

        public MockHttpMessageHandler(HttpResponseMessage toReturn, TimeSpan delayBeforeResponse = new TimeSpan())
        {
            _toReturn = toReturn;
            _delayBeforeResponse = delayBeforeResponse;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            await Task.Delay(_delayBeforeResponse, cancellationToken);            
            return await Task.FromResult(_toReturn);
        }
    }
}
