using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using DinnerBot.Models;
using DinnerBot.Forms;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;


using System.Collections.Generic;

namespace DinnerBot.Dialogs
{
    [Serializable]
    [LuisModel("89e03939-b575-4f66-9b6a-2cebbc70962a", "4f83c77f1ccf44df9c1ee72e4e0ce9ec")]
    public class RootDialog : LuisDialog<object>
    {

        private const string ReservationOption = "Reserve Table";
        private const string HelloOption = "Say Hello";
        private const string SearchRestaurantsOption = "Search Restaurants";

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("ReserveATable")]
        public async Task ReserveATable(IDialogContext context, LuisResult result)
        {
            try
            {
                await context.PostAsync("Great, lets book a table for you. You will need to provide a few details.");
                var form = new FormDialog<Reservation>(
                new Reservation(context.UserData.Get<String>("Name")),
                ReservationForm.BuildForm,
                FormOptions.PromptInStart,
                null);

                context.Call(form, this.ReservationFormComplete);
            }
            catch (Exception)
            {
                await context.PostAsync("Something really bad happened. You can try again later meanwhile I'll check what went wrong.");
                context.Wait(MessageReceived);
            }
        }

        [LuisIntent("SayHello")]
        public async Task SayHello(IDialogContext context, LuisResult result)
        {
            context.Call(new HelloDialog(), this.ResumeAfterOptionDialog);
        }
        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Insert Help Dialog here");
            context.Wait(MessageReceived);
        }


        private async Task ReservationFormComplete(IDialogContext context, IAwaitable<Reservation> result)
        {
            try
            {
                var reservation = await result;
                await context.PostAsync("Thanks for the using Dinner Bot.");
                //use a card for showing their data
                var resultMessage = context.MakeMessage();
                //resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                resultMessage.Attachments = new List<Attachment>();
                string ThankYouMessage;

                if (reservation.SpecialOccasion == Reservation.SpecialOccasionOptions.none)
                {
                    ThankYouMessage = reservation.Name + ", thank you for joining us for dinner, we look forward to having you and your guests.";
                }
                else
                {
                    ThankYouMessage = reservation.Name + ", thank you for joining us for dinner, we look forward to having you and your guests for the " + reservation.SpecialOccasion;
                }
                ThumbnailCard thumbnailCard = new ThumbnailCard()
                {

                    Title = String.Format("Dinner Reservations on {0}", reservation.ReservationDate.ToString("MM/dd/yyyy")),
                    Subtitle = String.Format("at {1} for {0} people", reservation.NumberOfDinners, reservation.ReservationTime.ToString("hh:mm")),
                    Text = ThankYouMessage,
                    Images = new List<CardImage>()
        {
            new CardImage() { Url = "https://upload.wikimedia.org/wikipedia/en/e/ee/Unknown-person.gif" }
        },
                };

                resultMessage.Attachments.Add(thumbnailCard.ToAttachment());
                await context.PostAsync(resultMessage);
                await context.PostAsync(String.Format(""));
            }
            catch (FormCanceledException)
            {
                await context.PostAsync("You canceled the transaction, ok. ");
            }
            catch (Exception ex)
            {
                var exDetail = ex;
                await context.PostAsync("Something really bad happened. You can try again later meanwhile I'll check what went wrong.");
            }
            finally
            {
                context.Wait(MessageReceivedAsync);
            }
        }


        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            //check to see if we already have username stored
            //If not, we will ask for it. 
            string userName = String.Empty;
            var message = await result;
            if (!context.UserData.TryGetValue<string>("Name", out userName))
            {
                context.Call(new UserInfoDialog(), ResumeAfterUserInfoDialog);
            }
            else
            {
                PromptUser(context);
            }
        }

        private async Task ResumeAfterUserInfoDialog(IDialogContext context, IAwaitable<object> result)
        {
            PromptUser(context);
        }


        private void PromptUser(IDialogContext context)
        {
            PromptDialog.Choice(
            context,
            this.OnOptionSelected,
            // Present two (2) options to user
            new List<string>() { ReservationOption, HelloOption, SearchRestaurantsOption },
            String.Format("Hi {0}, are you looking for to reserve a table or Just say hello?", context.UserData.Get<String>("Name")), "Not a valid option", 3);
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

                        var form = new FormDialog<Reservation>(
                        new Reservation(context.UserData.Get<String>("Name")),
                        ReservationForm.BuildForm,
                        FormOptions.PromptInStart,
                        null);

                        context.Call(form, this.ReservationFormComplete);
                        break;  

                    case HelloOption:
                        context.Call(new HelloDialog(), this.ResumeAfterUserHelloDialog);
                        break;

                    case SearchRestaurantsOption:
                        context.Call(new SearchRestaurantDialog(), this.ResumeAfterUserHelloDialog);
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

        private async Task ResumeAfterUserHelloDialog(IDialogContext context, IAwaitable<object> result)
        {
            //we want it to go right to the prompting of reservation or hello
            var message = await result;
            PromptUser(context);
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
            var message = await result;
            context.Wait(MessageReceivedAsync);
        }
    }
}