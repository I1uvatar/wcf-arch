using System.ServiceModel;
using BookService.DataContracts.Messages;

namespace BookService.ServiceContracts
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBookService" in both code and config file together.
    [ServiceContract]
    public interface IBookService
    {
        [OperationContract]
        GetAllBooksResponse GetAllBooks();
        
        // TODO: Add your service operations here
    }

}
