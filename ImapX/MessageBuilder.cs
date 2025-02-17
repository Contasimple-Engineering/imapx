﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using ImapX.Enums;

namespace ImapX
{
    internal class MessageBuilder
    {
        
        /// <summary>
        /// Creates a new <code>ImapX.Message</code> from EML
        /// </summary>
        /// <param name="eml">The eml data</param>
        public static Message FromEml(string eml)
        {
            var msg = new Message();
            var state = MessageFetchState.None;

            using (var reader = new StringReader(eml))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (string.IsNullOrEmpty(line))
                    {
                        if (state == MessageFetchState.None)
                            continue;

                        if (state == MessageFetchState.Headers)
                            state = MessageFetchState.Body;
                    }
                    else if(state == MessageFetchState.None)
                        state = MessageFetchState.Headers;



                    switch (state)
                    {
                        case MessageFetchState.Headers:
                            msg.TryProcessHeader(line);
                            break;
                        case MessageFetchState.Body:
                            line.ToString();
                            break;
                    }

                }
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts a ImapX.Message to EML
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string ToEml(Message message)
        {
            var sb = new StringBuilder();

            foreach (var header in message.Headers)
                sb.AppendLine(string.Format("{0}: {1}", header.Key, header.Value));

            sb.AppendLine(); // separate header from body through an empty line

            var boundary = message.BodyParts.Length > 1
                ? (message.ContentType == null || string.IsNullOrEmpty(message.ContentType.Boundary)
                    ? (Guid.NewGuid().ToString("N"))
                    : message.ContentType.Boundary)
                : null;

            if (boundary == null)
            {
                message.BodyParts.First().AppendEml(ref sb, false);
                return sb.ToString();
            }

            sb.AppendLine("This is a multipart message in MIME format");
            sb.AppendLine();

            foreach (var part in message.BodyParts)
            {
                sb.AppendLine("--" + boundary);
                part.AppendEml(ref sb, true);
                sb.AppendLine();
            }

            sb.AppendLine("--" + boundary + "--");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
