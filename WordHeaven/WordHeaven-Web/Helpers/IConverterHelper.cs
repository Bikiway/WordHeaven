using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Models;

namespace WordHeaven_Web.Helpers
{
    public interface IConverterHelper
    {
        //livros
        Livro ToLivros(BooksViewModel model, string imageId, bool isNew);
        BooksViewModel ToBooksViewModel(Livro livros);


        //Employees converter helper
        Employees ToEmployees(EmployeesViewModel model, bool isNew);
        EmployeesViewModel ToEmployeesViewModel(Employees employees);
    }
}
