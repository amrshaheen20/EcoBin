using EcoBin.API.Models.DbSet;
using EcoBin.API.Validators;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.Dtos
{
    public class WorkerTaskRequestDto
    {
        [IsValid(typeof(RequiredAttribute), typeof(IsExists<Worker>))]
        public int? WorkerId { get; set; }

        [IsValid(typeof(RequiredAttribute),typeof(IsExists<TranchBin>))]
        public int? TranchBinId { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [StringLength(maximumLength: 255, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 255 characters.")]
        public string? Title { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        [StringLength(maximumLength: 500, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 500 characters.")]
        public string? Description { get; set; }
        [IsValid(typeof(RequiredAttribute))]
        public DateTime? DueDate { get; set; }
        
        [DefaultValue(false)]
        public bool? IsCompleted { get; set; } = false;
    }

    public class WorkerTaskResponseDto : BaseResponseDto
    {
        public WorkerResponseDto Worker { get; set; } = default!;
        public BinResponseDto TranchBin { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
