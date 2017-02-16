using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace DinnerBot.Dialogs
{
    [Serializable]
    public class UserInfoDialog : IDialog<IMessageActivity>
    {
        public async Task StartAsync(IDialogContext context)
        {
            
            //Greet the user
            await context.PostAsync("Before we begin, we would like to know who we are talking to?");
            //We ask here but dont capture it here, we do that in the MessageRecieved Async
            await context.PostAsync("What is your name?");
            //We set a value telling us that we need to get the name out of userdata
            context.UserData.SetValue<bool>("GetName", true);
            //call context.Wait and set the callback method
            context.Wait(MessageReceivedAsync);

        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            //variable to hold message coming in
            var message = await argument;
            //variable for userName
            var userName = String.Empty;
            //variable to hold whether or not we need to get name
            var getName = false;
            //see if name exists
            context.UserData.TryGetValue<string>("Name", out userName);
            //if GetName exists we assign it to the getName variable and replace false
            context.UserData.TryGetValue<bool>("GetName", out getName);
            //If we need to get name, we go in here.
            if (getName)
            {
                //we get the username we stored above. and set getname to false
                userName = message.Text;
                context.UserData.SetValue<string>("Name", userName);
                context.UserData.SetValue<bool>("GetName", false);
            }
            //call context.done to exit this dialog and go back to the root dialog
            context.Done(message);
        }

    }
}