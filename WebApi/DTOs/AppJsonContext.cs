using System.Text.Json.Serialization;

namespace WebApi.DTOs;

[JsonSerializable(typeof(TransactionRequestDto))]
[JsonSerializable(typeof(TransactionResponseDto))]
public partial class AppJsonContext : JsonSerializerContext
{
}