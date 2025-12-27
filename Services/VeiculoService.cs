using CliCarProject.Data;
using CliCarProject.Models.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace CliCarProject.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        // Injeção de Dependências: Recebe o Contexto da DB e o Serviço de Ficheiros
        public VeiculoService(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task CreateVeiculoAsync(Veiculo veiculo, List<IFormFile> imagens, string userId)
        {
            veiculo.IdVendedor = userId; // Atribuir o ID do vendedor ao veículo

            _context.Veiculos.Add(veiculo); //Adiciona o veículo ao contexto

            await _context.SaveChangesAsync(); //Guarda as alterações para obter o ID do veículo

            if (imagens != null && imagens.Count > 0)
            {
                var savedFileNames = await _fileService.SaveFilesAsync(veiculo.IdVeiculo, imagens);

                //Usamos os nomes dos ficheiros guardados para criar as entidades Imagem
                foreach (var fileName in savedFileNames)
                {
                    var imagem = new Imagem
                    {
                        IdVeiculo = veiculo.IdVeiculo,
                        Nome = fileName
                    };
                    _context.Imagems.Add(imagem);
                }
                await _context.SaveChangesAsync(); // salvar imagens
            }
        }

        public async Task DesativarVeiculoAsync(int veiculoId)
        {
            // 1.OBTEM O VEÍCULO E AS ENTIDADES ASSOCIADAS
            var veiculo = await _context.Veiculos
            .Include(v => v.Anuncios) // Inclui anúncios para desativá-los
                                      // Não precisamos das Imagens nem do FileService, pois não eliminamos nada do disco.
            .FirstOrDefaultAsync(v => v.IdVeiculo == veiculoId);

                if (veiculo == null)
                {
                    throw new KeyNotFoundException($"Veículo com ID {veiculoId} não encontrado.");
                }

                // 2. MUDANÇA DE ESTADO (Soft Delete no Veículo)
                veiculo.Disponível = false; 

                // 3. DESATIVAÇÃO EM CASCATA DOS ANÚNCIOS ASSOCIADOS (Regra de Negócio)
                if (veiculo.Anuncios != null)
                {
                    foreach (var anuncio in veiculo.Anuncios)
                    {
                        // Assumimos que a entidade Anuncio tem um campo 'Estado' (ex: "Ativo", "Indisponível", "Vendido")
                        anuncio.Estado = "Indisponível";
                        anuncio.DataAtualizacao = DateTime.Now;
                    }
                }

                // 4. PERSISTIR TODAS AS MUDANÇAS
                _context.Veiculos.Update(veiculo);
                // As alterações em 'Anuncios' também são rastreadas pelo EF Core
                await _context.SaveChangesAsync();
        }

        public Task GerirImagensAsync(int idVeiculo, IFormFile[] novasImagens, int[] removerIds)
        {
            throw new NotImplementedException();
        }

        public Task<List<Veiculo>> GetAllVeiculosAsync(string sortOrder, int page, int pageSize)
        {
            throw new NotImplementedException();
        }

        public Task<Veiculo?> GetVeiculoByIdAsync(int idVeiculo)
        {
            throw new NotImplementedException();
        }

        public Task<List<Veiculo>> GetVeiculosByVendedorAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateVeiculoAsync(Veiculo veiculo)
        {
            throw new NotImplementedException();
        }
    }
}
