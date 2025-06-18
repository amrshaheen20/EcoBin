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



    public class WorkerResponseDto : AccountResponseDto
    {
        public int AccountId { get; set; }
        public  eWorkerJobType jobType { get; set; }
    }
}
