using Examen.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Oracle.ManagedDataAccess.Client;
using System.Data;

#region
//create or replace procedure sp_delete_comic
//(p_idcomic COMICS.IDCOMIC%TYPE)
//as
//begin
//delete from COMICS where IDCOMIC=p_idcomic;
//commit;
//end;
//
//create or replace procedure SP_INSERT_COMIC
//(p_idcomic COMICS.IDCOMIC%TYPE , p_nombre COMICS.NOMBRE%TYPE, p_imagen COMICS.IMAGEN%TYPE , p_descripcion COMICS.DESCRIPCION%TYPE)
//as
//begin
//	insert into COMICS values(p_idcomic, p_nombre, p_imagen, p_descripcion);
//commit;
//end;
#endregion
namespace Examen.Repositories
{
    public class RepositoryComicsOracle : IRepositoryComics
    {
        private DataTable tablaComics;
        private OracleConnection cn;
        private OracleCommand com;
        private OracleDataReader reader;

        public RepositoryComicsOracle()
        {
            string connectionString = @"Data Source=LOCALHOST:1521/XE; Persist Security Info=True; User Id=SYSTEM; Password=oracle";
            this.cn = new OracleConnection(connectionString);
            this.com = new OracleCommand();
            this.com.Connection = this.cn;
            string sql = "select * from COMICS";
            OracleDataAdapter ad = new OracleDataAdapter(sql, this.cn);
            this.tablaComics = new DataTable();
            ad.Fill(this.tablaComics);
        }

        public void DeleteComic(int idComic)
        {
            OracleParameter pamIdComic = new OracleParameter(":p_idcomic", idComic);
            this.com.Parameters.Add(pamIdComic);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "sp_delete_comic";
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public Comic FindComicByid(int idComic)
        {
            string sql = "select * from COMICS where IDCOMIC=:idcomic";
            OracleCommand cmd = new OracleCommand(sql, cn);
            cmd.Parameters.Add(new OracleParameter("idcomic", OracleDbType.Int32)).Value = idComic;
            cn.Open();
            OracleDataReader reader = cmd.ExecuteReader();

            Comic comic = null;
            if(reader.Read()) 
            {
                comic = new Comic();
                comic.IdComic = Convert.ToInt32(reader["IDCOMIC"]);
                comic.Nombre = reader["NOMBRE"].ToString();
                comic.Imagen = reader["IMAGEN"].ToString();
                comic.Descripcion = reader["DESCRIPCION"].ToString();
            }
            reader.Close();
            cn.Close();
            return comic;

        }

        public List<Comic> GetComicByNombre(string nombre)
        {
            var consulta = from datos in this.tablaComics.AsEnumerable() where datos.Field<string>("NOMBRE") == nombre select datos;
            if(consulta.Count() == 0)
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
            //INSERT NORMAL
            //string sql = "insert into COMICS values (:idcomic, :nombre, :imagen, :descripcion)";

            //OracleParameter pamIdComic = new OracleParameter(":idcomic", idComic);
            //this.com.Parameters.Add(pamIdComic);
            //OracleParameter pamNombre = new OracleParameter(":nombre", nombre);
            //this.com.Parameters.Add(pamNombre);
            //OracleParameter pamImagen = new OracleParameter(":imagen", imagen);
            //this.com.Parameters.Add(pamImagen);
            //OracleParameter pamDescripcion = new OracleParameter(":descripcion", descripcion);
            //this.com.Parameters.Add(pamDescripcion);

            //this.com.CommandType = CommandType.Text;
            //this.com.CommandText = sql;
            //this.cn.Open();
            //int af = this.com.ExecuteNonQuery();
            //this.cn.Close();
            //this.com.Parameters.Clear();

            //PROCEDIMIENTO ALMACENADO
            //con lambda
            var consulta = from datos in this.tablaComics.AsEnumerable() select datos;
            int maxId = consulta.Max(row => row.Field<int>("IDCOMIC"));
            int nuevoIdComic = maxId + 1;

            //PROCEDIMIENTO ALMACENADO
            OracleParameter pamIdComic = new OracleParameter(":p_idcomic", nuevoIdComic);
            this.com.Parameters.Add(pamIdComic);
            OracleParameter pamNombre = new OracleParameter(":p_nombre", nombre);
            this.com.Parameters.Add(pamNombre);
            OracleParameter pamImagen = new OracleParameter(":p_imagen", imagen);
            this.com.Parameters.Add(pamImagen);
            OracleParameter pamDescripcion = new OracleParameter(":p_descripcion", descripcion);
            this.com.Parameters.Add(pamDescripcion);

            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "sp_insert_comic";
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }

        public void InsertComicLambda(int idComic, string nombre, string imagen, string descripcion)
        {
            //con lambda
            var consulta = from datos in this.tablaComics.AsEnumerable() select datos;
            int maxId = consulta.Max(row => row.Field<int>("IDCOMIC"));
            int nuevoIdComic = maxId + 1;

            //PROCEDIMIENTO ALMACENADO
            OracleParameter pamIdComic = new OracleParameter(":p_idcomic", nuevoIdComic);
            this.com.Parameters.Add(pamIdComic);
            OracleParameter pamNombre = new OracleParameter(":p_nombre", nombre);
            this.com.Parameters.Add(pamNombre);
            OracleParameter pamImagen = new OracleParameter(":p_imagen", imagen);
            this.com.Parameters.Add(pamImagen);
            OracleParameter pamDescripcion = new OracleParameter(":p_descripcion", descripcion);
            this.com.Parameters.Add(pamDescripcion);

            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = "sp_insert_comic";
            this.cn.Open();
            int af = this.com.ExecuteNonQuery();
            this.cn.Close();
            this.com.Parameters.Clear();
        }
    }
}
