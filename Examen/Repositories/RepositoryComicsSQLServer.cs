using Examen.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Data;
using System.Data.SqlClient;

#region
//create procedure SP_DELETE_PERSONAJE
//(@idcomic int)
//as
//	delete from COMICS where IDCOMIC=@idcomic
//go
//
//create procedure SP_INSERT_COMIC_1
//(@idcomic int, @nombre varchar(255), @imagen varchar(255), @descripcion varchar(255))
//as
//	insert into COMICS values(@idcomic, @nombre, @imagen, @descripcion)
//go
#endregion
namespace Examen.Repositories
{
    public class RepositoryComicsSQLServer: IRepositoryComics
    {
        private DataTable tablaComics;
        private SqlConnection cn;
        private SqlCommand com;
        private SqlDataReader reader;

        public RepositoryComicsSQLServer()
        {
            string connectionString = @"Data Source=LOCALHOST\SQLEXPRESS;Initial Catalog=COMICS;User ID=javier;Password=MCSD2023";
            this.cn = new SqlConnection(connectionString);
            this.com = new SqlCommand();
            this.com.Connection = this.cn;
            this.tablaComics = new DataTable();
            string sql = "select * from COMICS";
            SqlDataAdapter ad = new SqlDataAdapter(sql, this.cn);
            ad.Fill(this.tablaComics);
        }

        public void DeleteComic(int idComic)
        {
            this.com.Parameters.AddWithValue("@idcomic", idComic);
            this.com.CommandType= CommandType.StoredProcedure;
            this.com.CommandText = "SP_DELETE_COMIC";
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public Comic FindComicByid(int idComic)
        {
            string sql = "select * from COMICS where IDCOMIC = @idcomic";
            this.com.Parameters.AddWithValue("@idcomic", idComic);
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            this.cn.Open();
            this.reader = this.com.ExecuteReader();
            this.reader.Read();

            Comic comic = new Comic();
            comic.IdComic = int.Parse(this.reader["IDCOMIC"].ToString());
            comic.Nombre = this.reader["NOMBRE"].ToString();
            comic.Imagen = this.reader["IMAGEN"].ToString();
            comic.Descripcion = this.reader["DESCRIPCION"].ToString();

            this.reader.Close();
            this.com.Parameters.Clear();
            this.cn.Close();
            return comic;
        }

        public List<Comic> GetComicByNombre(string nombre)
        {
            var consulta = from datos in this.tablaComics.AsEnumerable() where datos.Field<string>("NOMBRE") == nombre select datos;
            if (consulta.Count() == 0)
            {
                return null;
            }
            else
            {
                List<Comic> comics = new List<Comic>();
                foreach (var row in consulta)
                {
                    Comic co = new Comic
                    {
                        IdComic = row.Field<int>("IDCOMIC"),
                        Nombre = row.Field<String>("NOMBRE"),
                        Imagen = row.Field<string>("IMAGEN"),
                        Descripcion = row.Field<string>("DESCRIPCION")
                    };
                    comics.Add(co);
                }
                return comics;
            }
        }

        public List<Comic> GetComics()
        {
            var consulta = from datos in this.tablaComics.AsEnumerable() select datos;
            List<Comic> comics = new List<Comic>();
            foreach (var row in consulta)
            {
                Comic co = new Comic
                {
                    IdComic = row.Field<int>("IDCOMIC"),
                    Nombre = row.Field<String>("NOMBRE"),
                    Imagen = row.Field<string>("IMAGEN"),
                    Descripcion = row.Field<string>("DESCRIPCION")
                };
                comics.Add(co);
            }
            return comics;
        }

        public List<Comic> GetComicsNombreSelect(string nombre)
        {
            var consulta = from datos in this.tablaComics.AsEnumerable() where datos.Field<string>("NOMBRE") == nombre select datos;
            List<Comic> comics = new List<Comic>();
            foreach (var row in consulta)
            {
                Comic co = new Comic
                {
                    IdComic = row.Field<int>("IDCOMIC"),
                    Nombre = row.Field<String>("NOMBRE"),
                    Imagen = row.Field<string>("IMAGEN"),
                    Descripcion = row.Field<string>("DESCRIPCION")
                };
                comics.Add(co);
            }
            return comics;
        }

        public List<string> GetNombreComics()
        {
            var consulta = (from datos in this.tablaComics.AsEnumerable() select datos.Field<string>("NOMBRE")).Distinct();
            List<string> nomselect = new List<string>();
            foreach (var nom in consulta)
            {
                nomselect.Add(nom);
            }
            return nomselect;
        }

        public void InsertComic(int idComic, string nombre, string imagen, string descripcion)
        {
            // NORMAL SIN PROCEDURE
            //string sql = "insert into COMICS values (@idcomic, @nombre, @imagen, @descripcion)";
            //this.com.Parameters.AddWithValue("@idcomic", idComic);
            //this.com.Parameters.AddWithValue("@nombre", nombre);
            //this.com.Parameters.AddWithValue("@imagen", imagen);
            //this.com.Parameters.AddWithValue("@descripcion", descripcion);

            //this.com.CommandType = CommandType.Text;
            //this.com.CommandText = sql;
            //this.cn.Open();
            //int af = this.com.ExecuteNonQuery();
            //this.cn.Close();
            //this.com.Parameters.Clear();


            //CON LAMBDA
            var consulta = from datos in this.tablaComics.AsEnumerable() select datos;
            int maxId = consulta.Max(row => row.Field<int>("IDCOMIC"));
            int nuevoIdComic = maxId + 1;
            //PROCEDIMIENTO ALMACENADO
            this.com.Parameters.AddWithValue("@idcomic", nuevoIdComic);
            this.com.Parameters.AddWithValue("@nombre", nombre);
            this.com.Parameters.AddWithValue("@imagen", imagen);
            this.com.Parameters.AddWithValue("@descripcion", descripcion);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_INSERT_COMIC_1";
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();

           



        }

        public void InsertComicLambda(int idComic, string nombre, string imagen, string descripcion)
        {
            //CON LAMBDA
            var consulta = from datos in this.tablaComics.AsEnumerable() select datos;
            int maxId = consulta.Max(row => row.Field<int>("IDCOMIC"));
            int nuevoIdComic = maxId + 1;
            //PROCEDIMIENTO ALMACENADO
            this.com.Parameters.AddWithValue("@idcomic", nuevoIdComic);
            this.com.Parameters.AddWithValue("@nombre", nombre);
            this.com.Parameters.AddWithValue("@imagen", imagen);
            this.com.Parameters.AddWithValue("@descripcion", descripcion);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "SP_INSERT_COMIC_1";
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
    }
}
