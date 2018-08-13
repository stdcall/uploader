using System;
using System.Globalization;

namespace helloworld.Helpers
{

    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }

    }
}