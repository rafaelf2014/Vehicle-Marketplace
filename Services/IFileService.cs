using Microsoft.AspNetCore.Http;

namespace CliCarProject.Services
{
    public interface IFileService
    {
        //Método para guardar uma lista de ficheiros no sistema de ficheiros
        //Devolve uma lista com o nome dos ficheiros guardados
        Task<List<string>> SaveFilesAsync(int veiculoId, List<IFormFile> files);

        //Método para eliminar uma lista de ficheiros do sistema de ficheiros
        void DeleteFile(int veiculoId, string filename);
        
        //Método para obter o caminho completo de um ficheiro
        string GetFilePath(int veiculoId, string filename);
    }
}
