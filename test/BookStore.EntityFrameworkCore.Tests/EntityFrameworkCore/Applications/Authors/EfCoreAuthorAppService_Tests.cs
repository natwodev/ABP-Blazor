using BookStore.Authors;
using BookStore.EntityFrameworkCore;
using Xunit;

namespace BookStore.EntityFrameworkCore.Applications.Authors;

[Collection(BookStoreTestConsts.CollectionDefinitionName)]
public class EfCoreAuthorAppService_Tests : AuthorAppService_Tests<BookStoreEntityFrameworkCoreTestModule>
{

}
