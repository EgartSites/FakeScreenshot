using System.Text.RegularExpressions;
using TdLib;

namespace egartbot.Modules
{
    public class FakeScreenshot : ModuleBase
    {
        readonly string commandPattern = "^[.]\\s*screenshot$";
        public FakeScreenshot()
        {
            Subscribe<TdApi.Update.UpdateNewMessage>(Process, this);
        }

        public async Task Process(TdApi.Update.UpdateNewMessage updateNewMessage)
        {
            if (updateNewMessage.Message.IsOutgoing && updateNewMessage.Message.Content is TdApi.MessageContent.MessageText messageText)
            {
                var chat = await ExecuteAsync(new TdApi.GetChat
                {
                    ChatId = updateNewMessage.Message.ChatId
                });

                var text = messageText.Text.Text;

                Match match = Regex.Match(text, commandPattern);

                if (match.Success)
                {
                    await ExecuteAsync(new TdApi.DeleteMessages
                    {
                        ChatId = updateNewMessage.Message.ChatId,
                        MessageIds = new long[] { updateNewMessage.Message.Id },
                        Revoke = true
                    });

                    if (chat.Type is TdApi.ChatType.ChatTypePrivate || chat.Type is TdApi.ChatType.ChatTypeSecret)
                    {
                        await ExecuteAsync(new TdApi.SendChatScreenshotTakenNotification
                        {
                            ChatId = updateNewMessage.Message.ChatId
                        });
                    }
                }
            }
        }
    }
}