using System.Collections.Generic;
using BookService.DataContracts.Messages;
using BookService.DataContracts.Model;
using BookService.ServiceContracts;

namespace BookService.ServiceImplementation
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BookService" in both code and config file together.
    public class BookService : IBookService
    {
       public GetAllBooksResponse GetAllBooks()
        {
            {
                return new GetAllBooksResponse()
                {
                    Books = new List<Book>()
                {
                    new Book()
                    {
                        Author = "Lev Nikolajevič Tolstoj",
                        BookID = 333,
                        Price = 33.1m
                    }
                }
                };
            }
        }
    }
}
