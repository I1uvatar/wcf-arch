using System;
using System.Collections.Generic;
using AppPlatform.Core.EnterpriseLibrary.Diagnostics;
using AppPlatform.Core.EnterpriseLibrary.Services;
using BookService.DataContracts.Messages;
using BookService.DataContracts.Model;
using BookService.ServiceContracts;

namespace WcfArch.Client.BookConsole
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var books = GetAllBooksFromService();
            var bookPrintOutFormat = "Tile: {0}, Author: {1}, Price: {2}";
            foreach (var book in books)
            {
                Console.WriteLine(bookPrintOutFormat, book.Name, book.Author, book.Price);
            }
            Console.WriteLine("===========================================");
            Console.ReadLine();
        }

        private static List<Book> GetAllBooksFromService()
        {
            var proxy = new ServiceProxy<IBookService>();
            GetAllBooksResponse response = null;

            proxy.ExecuteAndRelease(() => response = proxy.Contract.GetAllBooks());
            if (Safe.IsNull(() => response.Books))
            {
                return new List<Book>();
            }

            return response.Books;
        }
    }
}
      

       
           
        