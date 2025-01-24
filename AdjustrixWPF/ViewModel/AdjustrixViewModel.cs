using AdjustrixWPF.Model;

namespace AdjustrixWPF.ViewModel
{
    /// <summary>
    /// Common base class for ViewModels. 
    /// Intended to have all common props (i.e. for theme, language) 
    /// </summary>
    public class AdjustrixViewModel : ViewModelBase
    {
		private LanguageViewModel languageViewModel = LanguageViewModel.Singleton;
		private ErrorMessageGenerator messageGenerator = new(LanguageViewModel.Singleton);
		public LanguageViewModel LanguageViewModel
		{
			get { return languageViewModel; }
			set { languageViewModel = value; }
		}

		public ErrorMessageGenerator ErrorMessageGenerator
		{
			get
			{
				return messageGenerator;
			}
		}

	}
}
