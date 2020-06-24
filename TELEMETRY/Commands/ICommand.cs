using System.Collections.Generic;

namespace TELEMETRY.Commands
{
    public interface ICommand
    {
        void Execute(Dictionary<string, string> arguments);
    }
}