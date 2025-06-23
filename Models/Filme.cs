using System.ComponentModel.DataAnnotations;

namespace FilmesAPI.Models {
    public class Filme {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Genero { get; set; }
        public int Ano { get; set; }
        public bool EhSerie { get; set; }

        public string Diretor { get; set; }
        public int DuracaoMinutos { get; set; }
        [DataType(DataType.Date)]
        public DateTime DataLancamento { get; set; }

        public string Sinopse { get; set; }
        public decimal Nota { get; set; }
        public string? ImagemUrl { get; set; }
    }
}
