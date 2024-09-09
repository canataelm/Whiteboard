namespace Common.Models.ReturnTypes.Abstract;

public interface IDataResult<T> : IResult
{
    T Data { get; set; }
}
