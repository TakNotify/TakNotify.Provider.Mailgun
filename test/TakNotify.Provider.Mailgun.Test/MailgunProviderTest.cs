// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Net.Http;
using TakNotify.Test;
using Xunit;

namespace TakNotify.Provider.Mailgun.Test
{
    public class MailgunProviderTest
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly Mock<ILoggerFactory> _loggerFactory;
        private readonly Mock<ILogger<Notification>> _logger;

        public MailgunProviderTest()
        {
            _httpClientFactory = new Mock<IHttpClientFactory>();
            _loggerFactory = new Mock<ILoggerFactory>();
            _logger = new Mock<ILogger<Notification>>();

            _loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(_logger.Object);
        }

        [Fact]
        public async void Send_Success()
        {
            var httpClientHandler = new FakeHttpMessageHandler(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            var httpClient = new HttpClient(httpClientHandler);
            _httpClientFactory.Setup(hcf => hcf.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var message = new MailgunMessage
            {
                FromAddress = "sender@example.com",
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var provider = new MailgunProvider(new MailgunOptions(), _httpClientFactory.Object, _loggerFactory.Object);

            var result = await provider.Send(message.ToParameters());

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);

            var startMessage = LoggerHelper.FormatLogValues(MailgunLogMessages.Sending_Start, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, startMessage);

            var endMessage = LoggerHelper.FormatLogValues(MailgunLogMessages.Sending_End, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, endMessage);
        }

        [Fact]
        public async void Send_Failed()
        {
            var httpClientHandler = new FakeHttpMessageHandler(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Error01")
            });
            var httpClient = new HttpClient(httpClientHandler);
            _httpClientFactory.Setup(hcf => hcf.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var message = new MailgunMessage
            {
                FromAddress = "sender@example.com",
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var provider = new MailgunProvider(new MailgunOptions(), _httpClientFactory.Object, _loggerFactory.Object);

            var result = await provider.Send(message.ToParameters());

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Error01", result.Errors[0]);

            var startMessage = LoggerHelper.FormatLogValues(MailgunLogMessages.Sending_Start, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, startMessage);

            var endMessage = LoggerHelper.FormatLogValues(MailgunLogMessages.Sending_End, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, endMessage, Times.Never());
        }

        [Fact]
        public async void Send_WithDefaultFromAddress_Success()
        {
            var httpClientHandler = new FakeHttpMessageHandler(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            var httpClient = new HttpClient(httpClientHandler);
            _httpClientFactory.Setup(hcf => hcf.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var message = new MailgunMessage
            {
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var provider = new MailgunProvider(
                new MailgunOptions() { DefaultFromAddress = "sender@example.com" }, 
                _httpClientFactory.Object, 
                _loggerFactory.Object);

            var result = await provider.Send(message.ToParameters());

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async void Send_WithoutFromAddress_ReturnError()
        {
            var httpClientHandler = new FakeHttpMessageHandler(new HttpResponseMessage(System.Net.HttpStatusCode.OK));
            var httpClient = new HttpClient(httpClientHandler);
            _httpClientFactory.Setup(hcf => hcf.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var message = new MailgunMessage
            {
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var provider = new MailgunProvider(new MailgunOptions(), _httpClientFactory.Object, _loggerFactory.Object);

            var result = await provider.Send(message.ToParameters());

            Assert.False(result.IsSuccess);
            Assert.Equal("From Address should not be empty", result.Errors[0]);
        }
    }
}
