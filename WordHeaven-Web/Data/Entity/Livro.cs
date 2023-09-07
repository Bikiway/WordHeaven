namespace WordHeaven_Web.Data.Entity
{
    public class Livro : IEntity
    {
        public int Id { get; set; }

        public string Titulo { get; set; }

        public string Autor { get; set; }

        public string Editora { get; set; }

        public string AnoDePublicacao { get; set; }

        public string Resumo { get; set; }

        public string Idioma { get; set; }

        public int NumeroPaginas { get; set; }

        public string Tematica { get; set; }

        public bool Desconto { get; set; }

        public double Valor { get; set; }

        public User user { get; set; }  

    }
}
