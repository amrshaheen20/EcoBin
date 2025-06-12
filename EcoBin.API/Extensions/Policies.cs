using EcoBin.API.Enums;

namespace EcoBin.API.Extensions
{
    public class Policies
    {
        public const string Manger = nameof(Manger);
        public const string Worker = nameof(Worker);
        public const string AllUsers = nameof(AllUsers);
        public const string BinSystem = nameof(BinSystem);

        public static readonly Dictionary<string, string[]> PolicyRolesMap = new()
        {
            { Manger,     new[] { nameof(eRole.Manger) } },
            { Worker,   new[] { nameof(eRole.Worker) } },

            { AllUsers,  new[]
                {
                    nameof(eRole.Manger),
                    nameof(eRole.Worker),
                }
            }
        };
    }
}
