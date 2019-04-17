using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Mocks.Tests.Common
{
    public class ExtendedExceptionAttribute : ExpectedExceptionBaseAttribute
    {
        public Type _expectedExceptionType { get; set; }

        public ExtendedExceptionAttribute(Type _expectedExceptionType, string _expectedErrorMessage) : base(_expectedErrorMessage)
        {
            this._expectedExceptionType = _expectedExceptionType;
        }

        protected override void Verify(Exception exception)
        {
            if (exception.Message != this.NoExceptionMessage || !exception.GetType().Equals(_expectedExceptionType))
                throw new Exception("Exception not expected");
        }
    }
}
