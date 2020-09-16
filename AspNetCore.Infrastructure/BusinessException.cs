using System;

namespace AspNetCore.Infrastructure
{
    public sealed class BusinessException : Exception
    {
        public BusinessExceptions Code { get; }

        public BusinessException(BusinessExceptions code, string message)
            : base(message)
        {
            Code = code;
        }
    }

    public enum BusinessExceptions
    {
        ProductAlreadyExists = 0,
        ProductDoesNotExist = 1
    }
}
