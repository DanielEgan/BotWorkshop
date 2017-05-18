using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

using System.Collections.Generic;

namespace DinnerBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {

        private const string ReservationOption = "Reserve Table";
        private const string HelloOption = "Say Hello";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            PromptDialog.Choice(
                 context,
                 this.OnOptionSelected,
                 new List<string>() { ReservationOption, HelloOption },
                 String.Format("Hi, are you looking for to reserve a table or Just say hello?"), "Not a valid option", 3);


        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                //capture which option then selected
                string optionSelected = await result;
                switch (optionSelected)
                {
                    case ReservationOption:
                        break;

                    case HelloOption:
                        context.Call(new HelloDialog(), this.ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                //If too many attempts we send error to user and start all over. 
                await context.PostAsync($"Ooops! Too many attempts :( You can start again!");

                //This sets us in a waiting state, after running the prompt again. 
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }


        private async Task HelloDialogCallback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
        }
    }
}