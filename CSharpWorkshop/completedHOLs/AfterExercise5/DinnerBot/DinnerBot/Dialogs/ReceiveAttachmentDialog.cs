using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.ProjectOxford.Face;
using System.IO;
using System.Diagnostics;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using System.Text;

[Serializable]
internal class ReceiveAttachmentDialog : IDialog<object>
{
    private string _faceKey;
    private string _emotionKey;
    private string _visionKey;

    public ReceiveAttachmentDialog(string faceKey, string emotionKey, string visionKey)
    {
        _faceKey = faceKey;
        _emotionKey = emotionKey;
        _visionKey = visionKey;
    }

    public async Task StartAsync(IDialogContext context)
    {
        context.Wait(this.MessageReceivedAsync);
    }

    public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;

        if (message.Attachments != null && message.Attachments.Any())
        {
            var attachment = message.Attachments.First();
            using (HttpClient httpClient = new HttpClient())
            {
                // Skype & MS Teams attachment URLs are secured by a JwtToken, so we need to pass the token from our bot.
                if ((message.ChannelId.Equals("skype", StringComparison.InvariantCultureIgnoreCase) || message.ChannelId.Equals("msteams", StringComparison.InvariantCultureIgnoreCase))
                    && new Uri(attachment.ContentUrl).Host.EndsWith("skype.com"))
                {
                    var token = await new MicrosoftAppCredentials().GetTokenAsync();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var responseMessage = await httpClient.GetAsync(attachment.ContentUrl);

                var contentLengthBytes = responseMessage.Content.Headers.ContentLength;
                var stream = await responseMessage.Content.ReadAsStreamAsync();

                var reply = context.MakeMessage();

                reply.Attachments.Add(new Attachment()
                {
                    ContentUrl = attachment.ContentUrl,
                    ContentType = "image/png",
                    Name = "5test.png"
                });

                await context.PostAsync(reply);
                //var faces = await DetectEmotions(GetImageAsMemoryStream(stream));
                //foreach (var face in faces)
                //{
                //    context.PostAsync($"{face.Scores.}");
                //}

                var visionResults = await DetectObjects(GetImageAsMemoryStream(stream));

                await context.PostAsync($"This image seems like: {string.Join(",", visionResults.Description.Tags)}");


                await context.PostAsync($"Attachment of {attachment.ContentType} type and size of {contentLengthBytes} bytes received.");
            }
        }
        else
        {
            await context.PostAsync("Hi there! I'm a bot created to show you how I can receive message attachments, but no attachment was sent to me. Please, try again sending a new message including an attachment.");
        }

        context.Wait(this.MessageReceivedAsync);
    }


    private async Task<AnalysisResult> DetectObjects(Stream imageStream)
    {
        var visionServiceClient = new VisionServiceClient(_visionKey);

        try
        {
            var visionResult = await visionServiceClient.DescribeAsync(imageStream);
            return visionResult;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            throw;
        }

    }


    private async Task<Emotion[]> DetectEmotions(Stream imageStream)
    {
        var emotionServiceClient = new EmotionServiceClient(_emotionKey);

        try
        {
            Emotion[] emotionResult;

            emotionResult = await emotionServiceClient.RecognizeAsync(imageStream);
            return emotionResult;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            throw;
        }

    }


    private async Task<Microsoft.ProjectOxford.Face.Contract.Face[]> DetectFaces(Stream imageStream)
    {
        var faceServiceClient = new FaceServiceClient(_faceKey);

        try
        {
            var faces = await faceServiceClient.DetectAsync(imageStream, false, true, new FaceAttributeType[] { FaceAttributeType.Emotion, FaceAttributeType.Gender, FaceAttributeType.HeadPose, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.FacialHair, FaceAttributeType.Glasses });
            return faces;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            throw;
        }

    }

    static MemoryStream GetImageAsMemoryStream(Stream myImage)
    {
        BinaryReader binaryReader = new BinaryReader(myImage);
        var bytes = binaryReader.ReadBytes((int)myImage.Length);
        return new MemoryStream(bytes);
    }


}
