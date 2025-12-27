using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationExtension;
internal sealed partial class ToastCommand(string message, MessageState state = MessageState.Info) : InvokableCommand
{
    public override ICommandResult Invoke()
    {
        var t = new ToastStatusMessage(new StatusMessage()
        {
            Message = message,
            State = state,
        });
        t.Show();

        return CommandResult.KeepOpen();
    }
}
