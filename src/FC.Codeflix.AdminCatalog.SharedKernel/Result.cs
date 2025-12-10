using System.Diagnostics.CodeAnalysis;

namespace FC.Codeflix.AdminCatalog.SharedKernel;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);
    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result<T> Failure<T>(string error) => new(default!, false, error);

    protected static Result<T> Create<T>(T? value) 
        => value is not null ? Success(value) : Failure<T>("Result has no value");
}

public class Result<T> : Result
{
    private readonly T? _value;

    internal Result(T value, bool isSuccess, string error)
        : base(isSuccess, error)
    {
        _value = value;
    }
    
    [NotNull]
    public T Value => _value! ?? throw new InvalidOperationException("Result has no value");
    
    public static implicit operator Result<T>(T? value) => Create(value);
}