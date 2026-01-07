using Interfaces_naturales.ViewModel;

namespace Interfaces_naturales.View;

public partial class SpeechToTextPage : ContentPage
{
    public SpeechToTextPage(SpeechToTextViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}