using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CleanCode14.Model.ArgsException;

namespace CleanCode14.Model;


public class ArgsException : Exception
{
    private char errorArgumentId = '\0';
    private string errorParameter = null;
    private ErrorCode errorCode = ErrorCode.OK;
    public ArgsException()
    {
    }
    public ArgsException(string message):base(message)
    {
    }
    public ArgsException(ErrorCode errorCode)
    {
        this.errorCode = errorCode;
    }
    public ArgsException(ErrorCode errorCode, string errorParameter)
    {
        this.errorCode = errorCode;
        this.errorParameter = errorParameter;
    }
    public ArgsException(ErrorCode errorCode, char errorArgumentId, string errorParameter)
    {
        this.errorCode = errorCode;
        this.errorParameter = errorParameter;
        this.errorArgumentId = errorArgumentId;
    }
    public char getErrorArgumentId()
    {
        return errorArgumentId;
    }
    public void setErrorArgumentId(char errorArgumentId)
    {
        this.errorArgumentId = errorArgumentId;
    }
    public string getErrorParameter()
    {
        return errorParameter;
    }
    public void setErrorParameter(string errorParameter)
    {
        this.errorParameter = errorParameter;
    }
    public ErrorCode getErrorCode()
    {
        return errorCode;
    }
    public void setErrorCode(ErrorCode errorCode)
    {
        this.errorCode = errorCode;
    }
    public string errorMessage()
    {
        switch (errorCode)
        {
            case ErrorCode.OK:
                return "TILT: Should not get here.";
            case ErrorCode.UNEXPECTED_ARGUMENT:
                return string.Format("Argument -{0} unexpected.", errorArgumentId);
            case ErrorCode.MISSING_STRING:
                return string.Format("Could not find string parameter for -{0}.", errorArgumentId);
            case ErrorCode.INVALID_INTEGER:
                return string.Format("Argument -%c expects an integer but was '{0}'.", errorArgumentId,
                        errorParameter);
            case ErrorCode.MISSING_INTEGER:
                return string.Format("Could not find integer parameter for -{0}.", errorArgumentId);
            case ErrorCode.INVALID_DOUBLE:
                return string.Format("Argument -%c expects a double but was '{0}'.", errorArgumentId, errorParameter);
            case ErrorCode.MISSING_DOUBLE:
                return string.Format("Could not find double parameter for -{0}.", errorArgumentId);
            case ErrorCode.INVALID_ARGUMENT_NAME:
                return string.Format("'{0}' is not a valid argument name.", errorArgumentId);
            case ErrorCode.INVALID_ARGUMENT_FORMAT:
                return string.Format("'{0}' is not a valid argument format.", errorParameter);
        }
        return "";
    }
    public enum ErrorCode
    {
        OK,
        INVALID_ARGUMENT_FORMAT,
        UNEXPECTED_ARGUMENT,
        INVALID_ARGUMENT_NAME,
        MISSING_STRING,
        MISSING_INTEGER,
        INVALID_INTEGER,
        MISSING_DOUBLE,
        INVALID_DOUBLE
    }
}
