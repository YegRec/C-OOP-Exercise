using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.IO.Pipelines;
using System.Text.Json.Serialization;

namespace ConsoleApp1
{
    internal class Program
    {
        public interface IPrestable
        {
            void Prestar();
            void Devolver();
        }

        public abstract class MaterialBiblioteca : IPrestable
        {
            [JsonInclude]
            public string ID { get; private set; }
            [JsonInclude]
            public string Titulo { get; private set; }
            [JsonInclude]
            public string Autor {  get; private set; }
            [JsonInclude]
            public int AnioPublicacion { get; private set; }
            [JsonInclude]
            public bool Status { get; private set; }

            public virtual void Constructor(string titulo,  string autor, int publicacion, string tipo)
            {
                //Constructor ID.
                ID = "B-" + tipo + titulo.Substring(0,1).ToUpper() + autor.Substring(0,1).ToUpper() + publicacion.ToString().Substring(2);
                Titulo = titulo;
                Autor = autor;      
                AnioPublicacion = publicacion;
                Status = true;
            }

            protected MaterialBiblioteca() { }

            public void Devolver()
            {
                Status = true;              
            }

            public void Prestar()
            {
                Status = false;
            }

            public bool VerStatus()
            {
                return Status;
            }

            public virtual void MostrarDetalles()
            {
                Console.WriteLine($"\nDetalles del material:\n" +
                    $"Titulo: {Titulo}\n" +
                    $"Autor: {Autor}\n" +
                    $"Publicacion: {AnioPublicacion}\n" +
                    $"ID: {ID}\n" +
                    $"Disponible: {Status}\n");
            }

        }

        public class Libro : MaterialBiblioteca
        {
            [JsonInclude]
            public int NumeroDePaginas { get; set; }



            public Libro(string titulo, string autor, int publicacion, int numPaginas)
            {
                base.Constructor(titulo, autor, publicacion, "L");
                NumeroDePaginas = numPaginas;
            }

            public Libro() { } // Constructor para el JSon

            public override void MostrarDetalles()
            {
                base.MostrarDetalles();
                Console.WriteLine($"Numero de paginas: {NumeroDePaginas}\n");
            }

        }

        public class Revista : MaterialBiblioteca
        {
            [JsonInclude]
            public int NumeroEdicion { get; set; }

            public Revista(string titulo, string autor, int publicacion, int edicion)
            {
                base.Constructor(titulo, autor, publicacion, "R");
                NumeroEdicion = edicion;
            }

            public Revista() { } // Constructor Json.

            public override void MostrarDetalles()
            {
                base.MostrarDetalles();
                Console.WriteLine($"Numero de edicion: {NumeroEdicion}\n");
            }
        }

        public class MaterialDiginal : MaterialBiblioteca
        {
            [JsonInclude]
            public string Formato { get; set; }

            public MaterialDiginal(string titulo, string autor, int publicacion, string formato)
            {
                base.Constructor(titulo, autor, publicacion, "MD");
            }
            public MaterialDiginal() { } // Constructor Json

            public override void MostrarDetalles()
            {
                base.MostrarDetalles();
                Console.WriteLine($"Formato: {Formato}\n");
            }
        }
        //Clase que gestiona la lista de Materiales
        public static class GestionLibreria
        {
            private static List<MaterialBiblioteca> Materiales = new List<MaterialBiblioteca>();

            public static void AgregarMaterial(MaterialBiblioteca material)
            {
                Materiales.Add(material);
            }

            public static void MostrarMateriales()
            {
                foreach (var materiales in Materiales)
                {
                    materiales.MostrarDetalles();
                }

            }

            public static int CantidadMateriales()
            {
                return Materiales.Count;
            }

            public static MaterialBiblioteca BuscarMaterial(string material)
            {
                MaterialBiblioteca testID = Materiales.Find(p => p.ID.ToUpper() == material.ToUpper());
                if (testID != null)
                {
                    return testID;
                }
                MaterialBiblioteca testAutor = Materiales.Find(p => p.Autor.ToLower() == material.ToLower());
                if (testAutor != null)
                {
                    return testAutor;
                }
                MaterialBiblioteca testTitulo = Materiales.Find(p => p.Titulo.ToLower() == material.ToLower());
                return testTitulo;
            }

            public static void PrestarMaterial(MaterialBiblioteca material)
            {
                material.Prestar();

            }

            public static void DevolverMaterial(MaterialBiblioteca material)
            {
                material.Devolver();
            }

            public static void Guardar()
            {
                string rutaArchivo = Path.Combine(Path.GetTempPath(), "Materiales.json");
                string json = JsonSerializer.Serialize(Materiales, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
                File.WriteAllText(rutaArchivo, json);
                Console.WriteLine("Se guardaron los archivos con exito.");
                Console.WriteLine($"Contenido del archivo:\n{json}");
            }

            public static void Cargar()
            {
                string rutaArchivo = Path.Combine(Path.GetTempPath(), "Materiales.json");
                if (File.Exists(rutaArchivo))
                {
                    string json = File.ReadAllText(rutaArchivo);
                    var objetos = JsonSerializer.Deserialize<List<JsonElement>>(json);

                    Materiales = objetos.Select(obj =>
                    {
                        string id = obj.GetProperty("ID").GetString();
                        char tipo = id[2]; // Extraer el carácter del tipo

                        // Validamos el tipo y deserializamos correctamente
                        if (tipo == 'L')
                            return (MaterialBiblioteca)JsonSerializer.Deserialize<Libro>(obj.GetRawText());
                        else if (tipo == 'R')
                            return (MaterialBiblioteca)JsonSerializer.Deserialize<Revista>(obj.GetRawText());
                        else if (tipo == 'M')
                            return (MaterialBiblioteca)JsonSerializer.Deserialize<MaterialDiginal>(obj.GetRawText());
                        else
                            throw new NotSupportedException($"Tipo no soportado en el ID: {id}");
                    }).ToList();

                    Console.WriteLine("Datos cargados con éxito.");
                }
                else
                {
                    Console.WriteLine("No se encontró el archivo.");
                }
            }




        }
        //Todo lo relacionado con interfaz, esta en esta clase.
        public static class InterfazUsuario
        {
            public static void Wait()
            {
                Console.WriteLine("Presiona cualquier tecla para continuar");
                Console.ReadKey();
            }
            private static void InvSelection()
            {
                Console.WriteLine("Seleccion invalida, intenta de nuevo");
            }

            //Metodo que retorna un string y recibe el tipo de mensaje que se decea en caso de error.
            private static string ReadString(string mensaje)
            {
                string readResult;
                do
                {
                    readResult = Console.ReadLine();
                    if (string.IsNullOrEmpty(readResult))
                    {
                        Console.WriteLine(mensaje);
                    }

                } while (readResult.Length == 0);

                return readResult;
            }


            //Este metodo retorna un string y recibe un string que simboliza el valor maximo que puede retornar el metodo
            private static int ReadInt(int c)
            {
                int result = 0;
                do
                {
                    string readResult = Console.ReadLine();
                    if (string.IsNullOrEmpty(readResult) || !int.TryParse(readResult, out result) || (Convert.ToInt32(readResult) > c) || (Convert.ToInt32(readResult) < 0))
                    {
                        InvSelection();
                    }
                } while (result <= 0 || result > c);
                return result;
            }

            private static int ReadInt(int c, string mensaje)
            {
                int result = 0;
                do
                {
                    string readResult = Console.ReadLine();
                    if (string.IsNullOrEmpty(readResult) || !int.TryParse(readResult, out result) || (Convert.ToInt32(readResult) > c) || (Convert.ToInt32(readResult) < 0))
                    {
                        Console.WriteLine(mensaje);
                    }
                } while (result <= 0 || result > c);
                return result;

            }

            public static int MenuPrincipal()
            {
                Console.Clear();
                Console.WriteLine($"Bienvenido, escoge una opcion:\n" +
                    $"1. Agregar nuevo material.\n" +
                    $"2. Buscar material.\n" +
                    $"3. Mostrar todos los materiales.\n" +
                    $"4. Prestar o devolver material.\n" +
                    $"5. Guardar lista en JSON.\n" +
                    $"6. Cargar lista desde JSON.");
                return ReadInt(6);
            }

            public static int MenuMaterial()
            {
                Console.Clear();
                Console.WriteLine("Selecciona el tipo de material:\n" +
                    "1. Libro.\n" +
                    "2. Revista.\n" +
                    "3. Material Digital.");
                return ReadInt(3);
            }

            public static void CreacionMaterial(int seleccion)
            {
                int numPaginas;
                int edicion;

                Console.Clear();
                Console.WriteLine("Ingresa el Titulo del Material:");
                string titulo = ReadString("Titulo incorrecto o nulo, intenta de nuevo");

                Console.Clear();
                Console.WriteLine("Ingresa el autor del Material:");
                string autor = ReadString("Autor incorrecto o nulo, intenta de nuevo");

                Console.Clear();
                Console.WriteLine("Ingresa el anio de publicacion");
                int anio = ReadInt(DateTime.Now.Year, "Anio invalido o nulo, intenta de nuevo");

                switch (seleccion)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("Ingresa el numero de paginas del libro");
                        numPaginas = ReadInt(int.MaxValue, "Numero incorrecto o nulo, intenta de nuevo");
                        Libro NuevoLibro = new Libro(titulo, autor, anio, numPaginas);
                        Console.WriteLine("Material agregado con exito");
                        GestionLibreria.AgregarMaterial(NuevoLibro);
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("Ingresa la edicion de la revista");
                        edicion = ReadInt(int.MaxValue, "Edicion incorrecta o nula, intenta de nuevo");
                        Revista NuevaRevista = new Revista(titulo, autor, anio, edicion);
                        Console.WriteLine("Material agregado con exito");
                        GestionLibreria.AgregarMaterial(NuevaRevista);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Ingresa el formato: PDF, ePUB, etc");
                        string formato = ReadString("Autor invalido o nulo, intenta de nuevo");
                        MaterialDiginal NuevoMD = new MaterialDiginal(titulo, autor, anio, formato);
                        Console.WriteLine("Material agregado con exito");
                        GestionLibreria.AgregarMaterial(NuevoMD);
                        break;
                }
            }

            public static void MenuBuscarMaterial()
            {
                Console.Clear();
                Console.WriteLine("Ingresa el Titulo, ID o Autor del material");
                string busqueda = ReadString("Titulo, ID o Autor invalido, intenta de nuevo");

                MaterialBiblioteca Material = GestionLibreria.BuscarMaterial(busqueda);

                if (Material != null)
                {
                    Material.MostrarDetalles();
                    return;
                }
                Console.WriteLine($"El material {busqueda} no fue encontrado.");
            }

            public static void MostrarMateriales()
            {
                Console.Clear();
                if (GestionLibreria.CantidadMateriales() > 0)
                {
                    GestionLibreria.MostrarMateriales();
                    return;
                }
                Console.WriteLine("No se encontro ningun material.");
            }

            public static int MenuPoDMaterial()
            {
                Console.Clear();
                Console.WriteLine("Selecciona una opcion:\n" +
                    "1. Devolver material.\n" +
                    "2. Tomar prestado un material.");
                return ReadInt(2);   
            }

            public static void DevolverMaterial()
            {
                Console.Clear();
                Console.WriteLine("Ingresa el material que deceas devolver (Titulo, ID, o Autor)");
                string material = ReadString("Material invalido, intenta de nuevo");

                MaterialBiblioteca Material = GestionLibreria.BuscarMaterial(material);

                if (Material != null)
                {
                    if (Material.VerStatus() == false)
                    {
                        Material.Devolver();
                        Console.WriteLine($"El material {Material.Titulo} fue devuelto con exito");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("El material no se puede devolver porque no esta prestado.");
                        return;
                    }
                }

                Console.WriteLine($"El material no existe.");
            }

            public static void PrestarMaterial()
            {
                Console.Clear();
                Console.WriteLine("Ingresa el material que deceas prestar (Titulo, ID, o Autor)");
                string material = ReadString("Material invalido, intenta de nuevo");

                MaterialBiblioteca Material = GestionLibreria.BuscarMaterial(material);

                if (Material != null)
                {
                    if (Material.VerStatus() == true)
                    {
                        Material.Prestar();
                        Console.WriteLine($"El material {Material.Titulo} fue prestado con exito");
                        return;
                    }
                    else
                    {
                        Console.WriteLine("El material no se puede prestar porque no esta disponible.");
                        return;
                    }
                }
                Console.WriteLine($"El material no existe.");
            }
        }
        static void Main(string[] args)
        {

            while (true)
            {
                switch (InterfazUsuario.MenuPrincipal())
                {
                    case 1:
                        {
                            InterfazUsuario.CreacionMaterial(InterfazUsuario.MenuMaterial());
                            InterfazUsuario.Wait();
                            break;
                        }
                    case 2:
                        {
                            InterfazUsuario.MenuBuscarMaterial();
                            InterfazUsuario.Wait();
                            break;
                        }
                    case 3:
                        {
                            InterfazUsuario.MostrarMateriales();
                            InterfazUsuario.Wait();
                            break;
                        }
                    case 4:
                        {
                            switch(InterfazUsuario.MenuPoDMaterial())
                            {
                                case 1:
                                    {
                                        InterfazUsuario.DevolverMaterial();
                                        InterfazUsuario.Wait();
                                        break;
                                    }
                                case 2:
                                    {
                                        InterfazUsuario.PrestarMaterial();
                                        InterfazUsuario.Wait();
                                        break;
                                    }
                            }
                            break;
                        }
                    case 5:
                        GestionLibreria.Guardar();
                        InterfazUsuario.Wait();
                        break;
                    case 6:
                        GestionLibreria.Cargar();
                        InterfazUsuario.Wait();
                        break;
                }
                               
            }

        }

    }
}
