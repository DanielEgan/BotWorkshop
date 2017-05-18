using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net.Http;
using DinnerBot.Models;

namespace DinnerBot.Dialogs
{
    [Serializable]
    public class SearchRestaurantDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            var responseMessage = "Please enter a zipcode";
            PromptDialog.Number(context, AfterChosenAsync, responseMessage, "Sorry! that was not a number. Please enter a zip code.",2);
        }

        private async Task AfterChosenAsync(IDialogContext context, IAwaitable<long> result)
        {
            //Zip Code from user
            var message = await result;

            //Create Message
            var reply = context.MakeMessage();
            //Set reply type to Carousel
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            //Make the call to the OpenTable API
            using (var client = new HttpClient())
            {
                try
                {
                   string url = "https://opentable.herokuapp.com/api/restaurants?zip=" + message;
                   HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        //retrieve response
                        var json = await response.Content.ReadAsStringAsync();
                        //create a object from the json
                        var des = (RootObject)Newtonsoft.Json.JsonConvert.DeserializeObject(json, typeof(RootObject));

                        //Create a list of cards to use for the data coming back. These are of type Attachment
                        List<Attachment> cards = new List<Attachment>();

                        //Loop through the results and turn them into cards
                        //Note:  I limit them to 10 because Skype has a limit of 10. If you send more none will show. 
                        foreach (var info in des.restaurants.Take(10))
                        {
                            //This dataset has images with it but they dont come back to the emultor so we are using
                            //a default opentable image
                            var image = "http://media.opentable.com/about/images/logos/ogimage.jpg"; //info.image_url;
                            //Call our card util to return the type of card we want. 
                            Attachment card = Utils.Cards.GetAdaptiveCard(
                                info.name,
                                info.address,
                                info.city,
                                new CardImage(url: image),
                                new CardAction(ActionTypes.OpenUrl, "Learn more", value: info.reserve_url)
                                );
                            cards.Add(card);
                        }
                        //when done add the cards to the reply
                        reply.Attachments = cards;
                        //post the reply (The cards in a carousel)
                        await context.PostAsync(reply);
                        //exit dialog
                        context.Done<object>(null);
                    }
                }
                catch (Exception ex)
                {

                    string myerror = ex.ToString();
                }

            }

            context.Done<object>(null);
        }
    }
}