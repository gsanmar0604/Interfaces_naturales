using Interfaces_naturales.ViewModel;

namespace Interfaces_naturales.View;

public partial class TextToSpeechPage : ContentPage
{
    public TextToSpeechPage(TextToSpeechViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}