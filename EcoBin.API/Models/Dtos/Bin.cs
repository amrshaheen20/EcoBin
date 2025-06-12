using EcoBin.API.Enums;
using EcoBin.API.Validators;
using System.ComponentModel.DataAnnotations;

namespace EcoBin.API.Models.Dtos
{
    public class BinRequestDto
    {
        [IsValid(typeof(RequiredAttribute))]
        public string? Name { get; set; }
        [IsValid(typeof(RequiredAttribute))]
        public string? Location { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public int? MaxCapacity { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public int? CurrentCapacity { get; set; }

        [IsValid(typeof(RequiredAttribute))]
        public bool? IsLidOpen { get; set; } = false;

        [IsValid(typeof(RequiredAttribute))]
        public bool? IsMaintenanceMode { get; set; } = false;
    }


    public class BinResponseDto : BaseResponseDto
    {
        public required string Name { get; set; }
        public required string Location { get; set; }
        public required int MaxCapacity { get; set; }
        public required int CurrentCapacity { get; set; }
        public required bool IsLidOpen { get; set; }
        public required bool IsMaintenanceMode { get; set; }

        public eBinStatus Status
        {
            get
            {
                if (CurrentCapacity < 0)
                    return eBinStatus.Unknown;
                if (CurrentCapacity == 0)
                    return eBinStatus.Empty;
                if (CurrentCapacity > 0 && CurrentCapacity < MaxCapacity)
                    return eBinStatus.Partial;
                if (CurrentCapacity == MaxCapacity)
                    return eBinStatus.Full;
                if (CurrentCapacity > MaxCapacity)
                    return eBinStatus.Overfilled;

                return eBinStatus.Unknown;
            }
        }

    }

    public class LidStateRequestDto
    {
        [Required]
        public bool IsOpen { get; set; }
    }

    public class CurrentCapacityRequestDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Current capacity must be a non-negative integer.")]
        public int CurrentCapacity
        {
            get; set;
        }
    }

    public class BinTokenResponseDto
    {
        public required int BinId { get; set; }
        public required string Token { get; set; }
    }

    public class BinStatusRequestDto : CurrentCapacityRequestDto
    {
        [Required]
        public bool IsLidOpen { get; set; }
        [Required]
        public bool IsMaintenanceMode { get; set; }

        [Required(ErrorMessage = "Token is required.")]
        public string Token { get; set; } =string.Empty;
    }
}

