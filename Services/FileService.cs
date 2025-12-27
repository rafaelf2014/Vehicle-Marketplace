using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using CliCarProject.Services;

namespace CliCarProject.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private const string UploadsFolder = "uploads/veiculos";

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        // Helper interno para obter o caminho completo no disco
        private string GetVehicleDirectory(int veiculoId)
        {
            // Combina o caminho base (wwwroot) com a pasta de uploads e o ID do veículo
            return Path.Combine(_environment.WebRootPath, UploadsFolder, veiculoId.ToString());
        }

        //Implementação do SaveFilesAsync
        public async Task<List<string>> SaveFilesAsync(int veiculoId, List<IFormFile> files)
        {
            var savedFileNames = new List<string>();
            var vehicleDir = GetVehicleDirectory(veiculoId);
            // Garantir que o diretório existe
            if (!Directory.Exists(vehicleDir))
            {
                Directory.CreateDirectory(vehicleDir);
            }
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var filePath = Path.Combine(vehicleDir, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    savedFileNames.Add(file.FileName);
                }
            }
            return savedFileNames;
        }

        //Implementação do DeleteFile
        public void DeleteFile(int veiculoId, string filename)
        {
            var vehicleDir = GetVehicleDirectory(veiculoId);
            var filePath = Path.Combine(vehicleDir, filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        //Implementação do GetFilePath
        public string GetFilePath(int veiculoId, string filename)
        {
            var vehicleDir = GetVehicleDirectory(veiculoId);
            return Path.Combine(vehicleDir, filename);
        }
    }
}
