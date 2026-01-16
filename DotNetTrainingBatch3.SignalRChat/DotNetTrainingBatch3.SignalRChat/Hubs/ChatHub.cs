using Microsoft.AspNetCore.SignalR;

namespace DotNetTrainingBatch3.SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        // Client က message ပို့လိုက်တဲ့အခါ Server က လက်ခံရယူမယ့် method
        public async Task ServerReceiveMessage(string user, string message)
        {
            // ချိတ်ဆက်ထားတဲ့ client အားလုံးဆီ message ကို broadcast လုပ်ပါတယ်
            await Clients.All.SendAsync("ClientReceiveMessage", user, message);
        }

    }
}
