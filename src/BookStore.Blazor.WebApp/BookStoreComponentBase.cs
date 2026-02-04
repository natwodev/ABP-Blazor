using BookStore.Localization;
using Volo.Abp.AspNetCore.Components;

namespace BookStore.Blazor.WebApp;

public abstract class BookStoreComponentBase : AbpComponentBase
{
    protected BookStoreComponentBase()
    {
        LocalizationResource = typeof(BookStoreResource);
    }
}
