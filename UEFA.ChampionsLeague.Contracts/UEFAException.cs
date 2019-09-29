using System;
using UEFA.ChampionsLeague.Contracts.Enums;

namespace UEFA.ChampionsLeague.Contracts
{
    public class UEFAException : Exception
    {
        public UEFAExceptionType Type { get; set; }

        public UEFAException(string message, UEFAExceptionType type = UEFAExceptionType.None) : base(message)
        {
        }
    }
}
