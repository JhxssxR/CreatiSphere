using System;
using System.Collections.ObjectModel;

namespace CreatiSphere.Services
{
    public class ChatMessage
    {
        public string SenderName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsFromCustomer { get; set; }
        public bool IsFromCreator => !IsFromCustomer;
        
        // For Customer Portal binding
        public Microsoft.Maui.Graphics.Color BackgroundColor => IsFromCustomer ? Microsoft.Maui.Graphics.Color.FromArgb("#0D9488") : Microsoft.Maui.Graphics.Color.FromArgb("#FFFFFF");
        public Microsoft.Maui.Graphics.Color TextColor => IsFromCustomer ? Microsoft.Maui.Graphics.Colors.White : Microsoft.Maui.Graphics.Color.FromArgb("#334155");
        public Microsoft.Maui.Controls.LayoutOptions Alignment => IsFromCustomer ? Microsoft.Maui.Controls.LayoutOptions.End : Microsoft.Maui.Controls.LayoutOptions.Start;

        // For Creator Portal binding
        public Microsoft.Maui.Graphics.Color CreatorBackgroundColor => !IsFromCustomer ? Microsoft.Maui.Graphics.Color.FromArgb("#0D9488") : Microsoft.Maui.Graphics.Color.FromArgb("#FFFFFF");
        public Microsoft.Maui.Graphics.Color CreatorTextColor => !IsFromCustomer ? Microsoft.Maui.Graphics.Colors.White : Microsoft.Maui.Graphics.Color.FromArgb("#334155");
        public Microsoft.Maui.Controls.LayoutOptions CreatorAlignment => !IsFromCustomer ? Microsoft.Maui.Controls.LayoutOptions.End : Microsoft.Maui.Controls.LayoutOptions.Start;
    }

    public static class MockMessageService
    {
        public static ObservableCollection<ChatMessage> ChatHistory { get; set; } = new ObservableCollection<ChatMessage>
        {
            new ChatMessage { SenderName = "Creator", Text = "Hello! Thanks for your order. Let me know if you have any questions.", Timestamp = DateTime.Now.AddMinutes(-30), IsFromCustomer = false }
        };

        public static bool HasNewMessageForCreator { get; set; } = false;
        public static bool HasNewMessageForCustomer { get; set; } = false;

        public static void AddMessage(string sender, string text, bool fromCustomer)
        {
            ChatHistory.Add(new ChatMessage
            {
                SenderName = sender,
                Text = text,
                Timestamp = DateTime.Now,
                IsFromCustomer = fromCustomer
            });

            if (fromCustomer) HasNewMessageForCreator = true;
            else HasNewMessageForCustomer = true;
        }
    }
}
