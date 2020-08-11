// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
namespace TakNotify
{
    /// <summary>
    /// The options for the <see cref="MailgunProvider"/>
    /// </summary>
    public class MailgunOptions : NotificationProviderOptions
    {
        internal static string Parameter_ApiKey = $"{MailgunConstants.DefaultName}_{nameof(Apikey)}";
        internal static string Parameter_Domain = $"{MailgunConstants.DefaultName}_{nameof(Domain)}";
        internal static string Parameter_DefaultFromAddress = $"{MailgunConstants.DefaultName}_{nameof(DefaultFromAddress)}";

        /// <summary>
        /// Instantiate the <see cref="MailgunOptions"/>
        /// </summary>
        public MailgunOptions()
        {
            Parameters.Add(Parameter_ApiKey, "");
            Parameters.Add(Parameter_Domain, "");
            Parameters.Add(Parameter_DefaultFromAddress, "");
        }

        /// <summary>
        /// The Mailgun API key
        /// </summary>
        public string Apikey
        {
            get => Parameters[Parameter_ApiKey].ToString();
            set => Parameters[Parameter_ApiKey] = value;
        }

        /// <summary>
        /// The configured domain in Mailgun
        /// </summary>
        public string Domain
        {
            get => Parameters[Parameter_Domain].ToString();
            set => Parameters[Parameter_Domain] = value;
        }

        /// <summary>
        /// The default "From Address" that will be used if the <see cref="MailgunMessage.FromAddress"/> is empty
        /// </summary>
        public string DefaultFromAddress
        {
            get => Parameters[Parameter_DefaultFromAddress].ToString();
            set => Parameters[Parameter_DefaultFromAddress] = value;
        }
    }
}
