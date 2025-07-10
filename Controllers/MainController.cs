using PlantManagement.Views;

namespace PlantManagement.Controllers
{
    public class MainController
    {
        public void OpenLoginWindow()
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }

        public void OpenRegisterWindow()
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
        }
    }
}