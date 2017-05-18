using AdaptiveCards;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DinnerBot.Utils
{
    public class Cards
    {
        //Create HeroCard method that takes in the data needed to construct the card, title, subtitle, image, etc.. 
        public static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            //Create a new herocard
            var heroCard = new HeroCard
            {
                //set the properties of the card
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            //return it as an attachment
            return heroCard.ToAttachment();
        }

        public static Attachment GetThumbnailCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var thumbNailCard = new ThumbnailCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return thumbNailCard.ToAttachment();
        }

        public static Attachment GetAdaptiveCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var adaptiveCard = new AdaptiveCard
            {
                BackgroundImage = "https://thumbs.dreamstime.com/z/perspective-wood-over-blurred-restaurant-bokeh-background-foods-drinks-product-display-montage-55441300.jpg",
                Body = new List<CardElement>
                {
                    new ColumnSet()
                    {
                        Columns = new List<Column>()
                        {
                            new AdaptiveCards.Column()
                            {
                                Size = "3",
                                Items = new List<AdaptiveCards.CardElement>()
                                {
                                    new TextBlock() { Text = title, Size = TextSize.Large, Weight = TextWeight.Bolder },
                                    new TextBlock() { Text = subtitle},
                                    new FactSet()
                                    {
                                        Facts = new List<AdaptiveCards.Fact>()
                                        {
                                            new AdaptiveCards.Fact() {Title = "Fact 1", Value = "Value 1" },
                                            new AdaptiveCards.Fact() {Title = "Fact 2", Value = "Value 2" }
                                        }
                                    },
                                    new ChoiceSet()
                                    {
                                        Id = "Times",
                                        Style = ChoiceInputStyle.Compact,
                                        Choices = new List<Choice>()
                                        {
                                            new Choice() { Title = "6 PM", Value = "6", IsSelected = true },
                                            new Choice() { Title = "7 PM", Value = "7" },
                                            new Choice() { Title = "8 PM", Value = "8" }
                                        }

                                    }
                                 
                                },
                            },
                            new AdaptiveCards.Column()
                            {
                                Items = new List<AdaptiveCards.CardElement>()
                                {
                                   new Image(){Url = cardImage.Url,Size = ImageSize.Stretch}
                                }
                            }
                        }
                    }

                }
            };

            /*////////////////////////////////////////////////////////////////
            Alternate way to create your cards, columns, textblocks, etc.. 
            ///////////////////////////////////////////////////////////////*/

            // ColumnSet set = new ColumnSet();
            // Column c1 = new Column()
            // {

            // };
            // Column c2 = new Column();
            // set.Columns.Add(c1);
            // set.Columns.Add(c2);

            // c1.Items.Add(new TextBlock()
            // {
            //     Text = title,
            //     Size = TextSize.Large,
            //     Weight = TextWeight.Bolder
            // });

            // c1.Items.Add(new TextBlock()
            // {
            //     Text = subtitle
            // });
            // c1.Items.Add(new FactSet()
            // {
            //     Facts = new List<AdaptiveCards.Fact>()
            //     {
            //         new AdaptiveCards.Fact() {Title = "Fact 1", Value = "Value 1" },
            //         new AdaptiveCards.Fact() {Title = "Fact 2", Value = "Value 2" }
            //     }  
            // });

            // // Add list of choices to the card.
            //c1.Items.Add(new ChoiceSet()
            // {
            //     Id = "snooze",
            //     Style = ChoiceInputStyle.Compact,
            //     Choices = new List<Choice>()
            //     {
            //         new Choice() { Title = "5 minutes", Value = "5", IsSelected = true },
            //         new Choice() { Title = "15 minutes", Value = "15" },
            //         new Choice() { Title = "30 minutes", Value = "30" }
            //     }
            // });
            // c2.Items.Add(new Image()
            // {
            //     Url = cardImage.Url,
            //     Size = ImageSize.Stretch
            // });

            // card.Body.Add(set);


            // Add text to the card.
            //card.Body.Add(new TextBlock()
            //{
            //    Text = title,
            //    Size = TextSize.Large,
            //    Weight = TextWeight.Bolder
            //});

            // Add text to the card.
            //card.Body.Add(new TextBlock()
            //{
            //    Text = subtitle
            //});
            //card.Body.Add(new Image()
            //{
            //    Url = cardImage.Url,
            //    Size = ImageSize.Medium
            //});

            adaptiveCard.Actions.Add(new SubmitAction()
            {
                    Title = "click me"
            });

            // Create the attachment.
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard
            };
            return attachment;
        }
    }

}