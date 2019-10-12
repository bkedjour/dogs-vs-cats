using System.IO;
using System.Threading.Tasks;
using DogsVsCats.Models;

namespace DogsVsCats.Contracts
{
    public interface IImageAnalyser
    {
        Task<FighterType?> GetFighterTypeAsync(Stream fighterImage);
    }
}
