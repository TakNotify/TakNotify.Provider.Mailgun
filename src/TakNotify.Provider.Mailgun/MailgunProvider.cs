// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TakNotify
{
    /// <summary>
    /// The notification provider to send email via Mailgun
    /// </summary>
    public class MailgunProvider : NotificationProvider
    {
        private readonly HttpClient _httpClient;
        private readonly MailgunOptions _options;
        private readonly string _messagesUrl;

        /// <summary>
        /// Instantiate the <see cref="MailgunProvider"/>
        /// </summary>
        /// <param name="options">The options for this provider</param>
        /// <param name="clientFactory">The HttpClient factory</param>
        /// <param name="loggerFactory">The logger factory</param>
        public MailgunProvider(MailgunOptions options, IHttpClientFactory clientFactory, ILoggerFactory loggerFactory) : base(options, loggerFactory)
        {
            _httpClient = CreateHttpClient(options, clientFactory);
            _options = options;
            _messagesUrl = $"/v3/{options.Domain}/messages";
        }

        /// <summary>
        /// Instantiate the <see cref="MailgunProvider"/>
        /// </summary>
        /// <param name="options">The options for this provider</param>
        /// <param name="clientFactory">The HttpClient factory</param>
        /// <param name="loggerFactory">The logger factory</param>
        public MailgunProvider(IOptions<MailgunOptions> options, IHttpClientFactory clientFactory, ILoggerFactory loggerFactory) : base(options.Value, loggerFactory)
        {
            _httpClient = CreateHttpClient(options.Value, clientFactory);
            _options = options.Value;
            _messagesUrl = $"/v3/{_options.Domain}/messages";
        }

        /// <inheritdoc cref="NotificationProvider.Name"/>
        public override string Name => MailgunConstants.DefaultName;

        /// <inheritdoc cref="NotificationProvider.Send(MessageParameterCollection)"/>
        public override async Task<NotificationResult> Send(MessageParameterCollection messageParameters)
        {
            var mgMessage = new MailgunMessage(messageParameters);

            var content = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("subject", mgMessage.Subject),
                new KeyValuePair<string, string>("text", mgMessage.PlainContent),
                new KeyValuePair<string, string>("html", mgMessage.HtmlContent)
            };

            if (!string.IsNullOrEmpty(mgMessage.FromAddress))
                content.Add(new KeyValuePair<string, string>("from", mgMessage.FromAddress));
            else if (_options != null && !string.IsNullOrEmpty(_options.DefaultFromAddress))
                content.Add(new KeyValuePair<string, string>("from", _options.DefaultFromAddress));
            else
                return new NotificationResult(new List<string> { "From Address should not be empty" });

            foreach (var to in mgMessage.ToAddresses)
            {
                content.Add(new KeyValuePair<string, string>("to", to));
            }

            foreach (var cc in mgMessage.CCAddresses)
            {
                content.Add(new KeyValuePair<string, string>("cc", cc));
            }

            foreach (var bcc in mgMessage.BCCAddresses)
            {
                content.Add(new KeyValuePair<string, string>("bcc", bcc));
            }

            Logger.LogDebug(MailgunLogMessages.Sending_Start, mgMessage.Subject, mgMessage.ToAddresses);

            var response = await _httpClient.PostAsync(_messagesUrl, new FormUrlEncodedContent(content));

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Logger.LogDebug(MailgunLogMessages.Sending_End, mgMessage.Subject, mgMessage.ToAddresses);

                return new NotificationResult(true);
            }

            var respBody = await response.Content.ReadAsStringAsync();
            return new NotificationResult(new List<string> { respBody });
        }

        private HttpClient CreateHttpClient(MailgunOptions options, IHttpClientFactory clientFactory)
        {
            var client = clientFactory.CreateClient();
            client.BaseAddress = new Uri(MailgunConstants.ApiBaseUrl);

            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{options.Apikey}"));
            client.DefaultRequestHeaders.Add("Authentication", $"Basic {token}");

            return client;
        }
    }
}
