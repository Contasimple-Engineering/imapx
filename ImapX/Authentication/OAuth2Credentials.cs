﻿using System;
using System.IO;
using System.Text;
using ImapX.Constants;

namespace ImapX.Authentication
{
    /// <summary>
    /// Credentials used for OAuth2 authentication
    /// </summary>
    public class OAuth2Credentials : ImapCredentials
    {
        public OAuth2Credentials(string login, string authToken, string vendor = null)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(authToken))
                throw new ArgumentException("Login and auth token cannot be empty");

            TwoWayProcessing = true;

            Login = login;
            AuthToken = authToken;
            Vendor = vendor;
        }

        /// <summary>
        ///     The login name
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        ///     The auth token
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// The vendor
        /// </summary>
        public string Vendor { get; set; }

        public override string ToCommand(Capability capabilities)
        {
            if (!IsSupported(capabilities))
                throw new NotSupportedException("The selected authentication mechanism is not supported");

            return string.Format(ImapCommands.Authenticate + " {1}", "XOAUTH2",
                PrepareOAuthCredentials(Login, AuthToken, Vendor));
        }

        public override bool IsSupported(Capability capabilities)
        {
            return capabilities.XoAuth2;
        }

        private string PrepareOAuthCredentials(string login, string token, string vendor = null)
        {
            byte[] userData = Encoding.UTF8.GetBytes("user=" + login);
            byte[] tokenLabelData = Encoding.UTF8.GetBytes("auth=Bearer ");
            byte[] vendorData = string.IsNullOrEmpty(vendor) ? null : Encoding.UTF8.GetBytes("vendor=" + vendor + "\n");
            byte[] tokenData = Encoding.UTF8.GetBytes(token + (vendorData == null ? "\n" : ""));

            using (var stream = new MemoryStream())
            {
                stream.Write(userData, 0, userData.Length);
                stream.WriteByte(1);
                stream.Write(tokenLabelData, 0, tokenLabelData.Length);
                stream.Write(tokenData, 0, tokenData.Length);
                stream.WriteByte(1);
                if (vendorData != null)
                {
                    stream.Write(vendorData, 0, vendorData.Length);
                    stream.WriteByte(1);
                }
                stream.WriteByte(1);
                return Convert.ToBase64String(stream.ToArray());
            }
        }


        public override void ProcessCommandResult(string data)
        {
        }

        public override byte[] AppendCommandData(string serverResponse)
        {
            return Encoding.UTF8.GetBytes(Environment.NewLine);
        }
    }

}