using System;

namespace DShop.Common.Types
{
    public class DShopException : Exception
    {
        public string Code { get; }

        public DShopException()
        {
        }

        public DShopException(string code)
        {
            Code = code;
        }

        public DShopException(string message, params object[] args) 
            : this(string.Empty, message, args)
        {
        }

        public DShopException(string code, string message, params object[] args) 
            : this(null, code, message, args)
        {
        }

        public DShopException(Exception innerException, string message, params object[] args)
            : this(innerException, string.Empty, message, args)
        {
        }

        public DShopException(Exception innerException, string code, string message, params object[] args)
            : base(string.Format(message, args), innerException)
        {
            Code = code;
        }        
    }
}