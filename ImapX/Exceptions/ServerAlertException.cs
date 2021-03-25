using System;

namespace ImapX.Exceptions
{
    public class ServerAlertException : Exception
    {
        public ServerAlertException() { }
        public ServerAlertException(string message) : base(message) { }
    }
}
