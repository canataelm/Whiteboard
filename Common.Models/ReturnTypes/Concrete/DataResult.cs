using Common.Models.ReturnTypes.Abstract;

namespace Common.Models.ReturnTypes.Concrete;

public class DataResult<T> : Result, IDataResult<T>
{
    public T Data { get; set; }

    public DataResult(T data, bool isSuccess, string message) : base(isSuccess, message)
    {
        Data = data;
    }
    public DataResult(T data, bool isSuccess) : base(isSuccess)
    {
        Data = data;
    }
}
