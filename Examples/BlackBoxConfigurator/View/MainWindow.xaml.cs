using BlackBoxConfigurator.ViewModel;
using System.Windows;

namespace BlackBoxConfigurator.View
{
    public partial class MainWindow : Window
    {
        internal MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Utilities.AddDefautTextBoxEventHandlers(this);
        }
    }
}