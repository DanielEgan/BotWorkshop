using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;


namespace DinnerBot.Dialogs
{
    [Serializable]
    public class HelloDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            //Greet the user
            await context.PostAsync(String.Format("Hey there {0}, how are you?", context.UserData.Get<String>("Name")));

            //call context.Done
            context.Done<object>(null);
        }

    }
}
