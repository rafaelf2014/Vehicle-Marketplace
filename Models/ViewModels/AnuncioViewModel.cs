//Eu sei o que podes estar a pensar: Para que serve a viewModel? Isso deveria ser para MVVM!

//Na verdade , a viewModel aqui serve para estruturar os dados que serão enviados para a view, garantindo que apenas as informações necessárias sejam expostas. Isso ajuda a manter a separação de preocupações e facilita a manutenção do código.
//Além disso, a viewModel pode ser usada para formatar ou transformar os dados conforme necessário antes de serem apresentados na interface do usuário.
//Por isso, mesmo em um padrão MVC, o uso de viewModels pode ser benéfico para melhorar a clareza e a organização do código.


using CliCarProject.Models.Classes;

namespace CliCarProject.Models.ViewModels
{
    public class AnuncioViewModel
    {

        public int IdSelectedVeiculo { get; set; }
        public int IdSelectedLocalizacao { get; set; }

        public string Title { get; set; } = null!; 
        public string? Description { get; set; }
        public decimal Price { get; set; }

        //Dados para as dropdowns

        public List<Veiculo> Veiculos { get; set; } = null!;
        public List<Localizacao> Localizacoes { get; set; } = null!;
    }
}
