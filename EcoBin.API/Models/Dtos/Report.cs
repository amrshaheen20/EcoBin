using EcoBin.API.Models.DbSet;
using EcoBin.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.Dtos
{
    public class ReportRequestDto
    {
        [IsValid(typeof(RequiredAttribute))]
        [StringLength(255)]
        public string? Title { get; set; }
        [IsValid(typeof(RequiredAttribute))]
        [StringLength(500)]
        public string? Message { get; set; }
    }
    public class ReportResponseDto:BaseResponseDto
    {
        public required string Title { get; set; }
        public required string Message { get; set; }
        public required WorkerResponseDto Worker { get; set; } = default!;
    }
}
