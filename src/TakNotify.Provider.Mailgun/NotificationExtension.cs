// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using System.Threading.Tasks;

namespace TakNotify
{
    /// <summary>
    /// The extension for <see cref="INotification"/> to send email with Mailgun provider
    /// </summary>
    public static class NotificationExtension
    {
        /// <summary>
        /// Send message with <see cref="MailgunProvider"/>
        /// </summary>
        /// <param name="notification">The notification object</param>
        /// <param name="message">The wrapper of the message that will be sent</param>
        /// <returns></returns>
        public static Task<NotificationResult> SendEmailWithMailgun(this INotification notification, MailgunMessage message)
        {
            return notification.Send(MailgunConstants.DefaultName, message.ToParameters());
        }
    }
}
