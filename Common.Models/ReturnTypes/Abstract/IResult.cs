namespace Common.Models.ReturnTypes.Abstract;

public interface IResult
{
    bool IsSuccess { get; set; }
    string Message { get; set; }
}
