using EcoBin.API.Enums;
using EcoBin.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.Dtos
{
    public class WorkerRequestDto : RegisterRequestDto
    {
        [IsValid(typeof(RequiredAttribute))]
        public eWorkerJobType? jobType { get; set; }
    }

    public class WorkerResponseDto : BaseResponseDto
    {
        public  AccountResponseDto Account { get; set; } = default!;
        public  eWorkerJobType jobType { get; set; }
    }
}
