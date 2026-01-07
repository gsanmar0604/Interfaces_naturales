using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces_naturales.Model
{
    public class SpeechHistoryItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public SpeechType Type { get; set; }
    }

    public enum SpeechType
    {
        SpeechToText,
        TextToSpeech
    }
}
