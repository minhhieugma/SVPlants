using System;
namespace Application.Exceptions;

public class MyApplicationException : Exception
{
    public dynamic Payload { get; set; }

    public MyApplicationException(string? message, Exception? innerException) : base(message, innerException)
    {

    }
}