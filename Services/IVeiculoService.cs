using CliCarProject.Models.Classes;

namespace CliCarProject.Services
{
    public interface IVeiculoService
    {
        //Método para criar veículo
        Task CreateVeiculoAsync(Veiculo veiculo, List<IFormFile> imagens, string userId);

        //Método para obter veículo por vendedor
        Task<List<Veiculo>> GetVeiculosByVendedorAsync(string userId);

        //Método para obter veículo por ID
        Task<Veiculo?> GetVeiculoByIdAsync(int idVeiculo);

        //Método para obter todos os veículos para página de veiculos
        Task<List<Veiculo>> GetAllVeiculosAsync(string sortOrder, int page, int pageSize);

        //Método para atualizar veículo
        Task UpdateVeiculoAsync(Veiculo veiculo);

        //Método para gerir imagens de veículo
        Task GerirImagensAsync(int idVeiculo, IFormFile[] novasImagens, int[] removerIds);

        //Método para deletar veículo
        Task DesativarVeiculoAsync(int veiculoId);
    }
}
