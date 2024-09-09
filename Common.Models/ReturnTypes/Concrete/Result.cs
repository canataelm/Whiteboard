using Common.Models.ReturnTypes.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ReturnTypes.Concrete;

public class Result : IResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

    public Result(bool isSuccess, string message) : this(isSuccess)
    {
        Message = message;
    }
    public Result(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }
}
