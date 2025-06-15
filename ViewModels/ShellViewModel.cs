using Caliburn.Micro;


namespace DBF.ViewModels
{
    public class ShellViewModel : Conductor<Screen>.Collection.OneActive, IConductActiveItem
    {
        public ShellViewModel()
        {
            //var db = new DBFContext();
      
        }

        #region Show Screens
        public async void OpenControlView()        
        {
            var screen = IoC.Get<ControlViewModel>();

            await ActivateItemAsync(screen);
            await screen.ShowProjector();
        }
        #endregion
    }
}