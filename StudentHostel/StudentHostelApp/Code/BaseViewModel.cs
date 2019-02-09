using System.ComponentModel;

namespace StudentHostelApp.Code
{
    /// <summary>
    /// Предоставляет базовый интерфейс ViewModel
    /// </summary>
    public class BaseViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string property)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
