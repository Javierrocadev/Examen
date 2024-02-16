using Examen.Models;

namespace Examen.Repositories
{
    public interface IRepositoryComics
    {
        List<Comic> GetComics();

        void InsertComic(int idComic, string nombre, string imagen, string descripcion);

        void InsertComicLambda(int idComic, string nombre, string imagen, string descripcion);

        void DeleteComic(int idComic);

        Comic FindComicByid(int idComic);

        List<Comic> GetComicByNombre(string nombre);

        List<Comic> GetComicsNombreSelect(string nombre);

        List<string> GetNombreComics();
    }
}
