using WordHeaven_Web.Data.Entity;
using WordHeaven_Web.Models.Books;
using WordHeaven_Web.Models.Employee;

namespace WordHeaven_Web.Helpers
{
    public class ConverterHelper : IConverterHelper
    {
        //Books
        public Livro ToLivros(BooksViewModel model, string imageId, bool isNew)
        {
            return new Livro
            {
                Id = isNew ? 0 : model.Id,
                Titulo = model.Titulo,
                Autor = model.Autor,
                Editora = model.Editora,
                AnoDePublicacao = model.AnoDePublicacao,
                Idioma = model.Idioma,
                NumeroPaginas = model.NumeroPaginas,
                Resumo = model.Resumo,
                Tematica = model.Tematica,
                Desconto = model.Desconto,
                Valor = model.Valor,
                user = model.user,
            };
        }
        public BooksViewModel ToBooksViewModel(Livro livros)
        {
            return new BooksViewModel
            {
                Id = livros.Id,
                Titulo = livros.Titulo,
                Autor = livros.Autor,
                Editora = livros.Editora,
                AnoDePublicacao = livros.AnoDePublicacao,
                Idioma = livros.Idioma,
                NumeroPaginas = livros.NumeroPaginas,
                Resumo= livros.Resumo,
                Tematica= livros.Tematica,
                Desconto= livros.Desconto,
                Valor= livros.Valor,
                user = livros.user,
            };
        }




        //Employees
        public Employees ToEmployees(EmployeesViewModel model, bool isNew)
        {
            return new EmployeesViewModel
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                PhoneNumber = model.PhoneNumber,
                JobTitle = model.JobTitle,
                UserName = model.UserName,
                storeId = model.storeId,
                stores = model.stores,
                user = model.user,
            };
        }

        public EmployeesViewModel ToEmployeesViewModel(Employees employees)
        {
            return new EmployeesViewModel
            {
                Id = employees.Id,
                FirstName = employees.FirstName,
                LastName = employees.LastName,
                Age = employees.Age,
                PhoneNumber = employees.PhoneNumber,
                JobTitle = employees.JobTitle,
                UserName = employees.UserName,
                storeId= employees.storeId,
                stores = employees.stores,
                user = employees.user,
            };
        }

       
    }
}
