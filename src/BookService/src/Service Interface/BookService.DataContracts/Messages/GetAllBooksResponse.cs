using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BookService.DataContracts.Constants;
using BookService.DataContracts.Model;

namespace BookService.DataContracts.Messages
{
    [DataContract(Namespace = Namespace.May2015Message, Name = "GetAllBooksResponse")]
    public class GetAllBooksResponse
    {
        [DataMember(Order = 0, Name = "Books", IsRequired = true)]
        public List<Book> Books { get; set; }

    }
}
