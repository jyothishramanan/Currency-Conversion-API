using AutoMapper;
using Currency_Conversion_API.ViewModel;
using Currency_Conversion_Business.Models;

namespace Currency_Conversion_Business.Helper
{
    public class CustomMapper:Profile
    {
        public CustomMapper() 
        {
           CreateMap<HistoryModel, History>();
        }
    }
}
