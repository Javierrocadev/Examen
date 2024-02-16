using Examen.Models;
using Examen.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Examen.Controllers
{
    public class ComicsController : Controller
    {
        private IRepositoryComics repo;

        public ComicsController(IRepositoryComics repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            List<Comic> comics= this.repo.GetComics();
            return View(comics);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Comic comic)
        {
            this.repo.InsertComic(comic.IdComic, comic.Nombre, comic.Imagen, comic.Descripcion);
            return RedirectToAction("Index");
        }

        public IActionResult CreateLambda()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateLambda(Comic comic)
        {
            this.repo.InsertComicLambda(comic.IdComic, comic.Nombre, comic.Imagen, comic.Descripcion);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int idcomic)
        {
            this.repo.DeleteComic(idcomic);
            return RedirectToAction("Index");
        }
        //filtro
        public IActionResult ComicsNombre()
        {
            List<Comic> comics = this.repo.GetComics();
            return View(comics);
        }

        [HttpPost]
        public IActionResult ComicsNombre(string Nombre)
        {
            List<Comic> comics = this.repo.GetComicByNombre(Nombre);
            if(comics == null)
            {
                ViewData["MENSAJE"] = "No hay ningún comic con este nombre";
                return View();
            }
            else
            {
                return View(comics);
            }
        }

        //eliminar con detalles
        public IActionResult DeleteDetails(int idcomic)
        {
            Comic comic = this.repo.FindComicByid(idcomic);
            return View(comic);
        }
       
        //filtrado con select
        public IActionResult ComicsSelect()
        {
            ViewData["NOMBRE"] = this.repo.GetNombreComics();
            return View();
        }
        [HttpPost]
        public IActionResult ComicsSelect(string nombre)
        {
            ViewData["NOMBRE"] = this.repo.GetNombreComics();
            List<Comic> comics = this.repo.GetComicsNombreSelect(nombre);
            return View(comics);
        }

    }
}
