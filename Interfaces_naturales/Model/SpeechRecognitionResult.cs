using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces_naturales.Model
{
    public class SpeechRecognitionResult
    {
        public string RecognizedText { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
