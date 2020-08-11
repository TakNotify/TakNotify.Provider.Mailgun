// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TakNotify.Provider.Mailgun.Test
{
    public class FakeHttpMessageHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage _expectedResponse;

        public FakeHttpMessageHandler(HttpResponseMessage expectedResponse)
        {
            _expectedResponse = expectedResponse;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(_expectedResponse);
        }
    }
}
