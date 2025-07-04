﻿using System.Net;
using System.Text.Json.Serialization;

namespace EcoBin.API.Interfaces
{
    public interface IBaseResponse<T>
    {
        HttpStatusCode Status { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        T? Data { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        IEnumerable<object>? Errors { get; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        string? Message { get; }

        IBaseResponse<T> SetData(T data);
        IBaseResponse<T> SetErrors(IEnumerable<object> errors);
        IBaseResponse<T> SetMessage(string message);
        IBaseResponse<T> SetStatus(HttpStatusCode statusCode);
    }
}