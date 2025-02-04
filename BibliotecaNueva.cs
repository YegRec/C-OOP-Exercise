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
using System.Security.Principal;
using System.Text.Json.Serialization.Metadata;
using System.Runtime.InteropServices;


namespace ConsoleApp1
{
    internal class Program
    {
        public enum Categorias
        {
            Entretenimiento = 1,
            Informativo = 2,
            Educativo = 3,
        }
        [JsonPolymorphic]
        [JsonDerivedType(typeof(Libro), typeDiscriminator: "Libro")]
        [JsonDerivedType(typeof(Revista), typeDiscriminator: "Revista")]
        [JsonDerivedType(typeof(MaterialDigital), typeDiscriminator: "MaterialDigital")]
        public abstract class MaterialBiblioteca
        {
            [JsonInclude]
            public string ID { get; protected set; }
            [JsonInclude]
            public string Titulo { get; protected set; }
            [JsonInclude]
            public string Autor { get; protected set; }
            [JsonInclude]
            public int AnioPublicacion { get; protected set; }
            [JsonInclude]
            public Categorias Categoria { get; protected set; }
            [JsonInclude]
            public bool Status { get; protected set; }

            public void Prestar()
            {
                Status = false;
            }

            public void Devolver()
            {
                Status = true;
            }

            public virtual void MostrarDetalles()
            {
                Console.WriteLine($"\nTitulo: {Titulo}\n" +
                    $"Autor: {Autor}\n" +
                    $"Publicacion: {AnioPublicacion}\n" +
                    $"ID: {ID}\n" +
                    $"Categoria: {Categoria}\n" +
                    $"Status: {Status}");
            }

            //Este metodo se encarga de construir el ID de cada material basado en su titulo, autor y anio de publicacion.
            //primero verifica si el titulo tiene mas de una palabra, luego agrega el primer caracter de cada palabra al id.
            //luego hace lo mismo con el autor y finalmente agrega el anio de publicacion al final del ID.
            //IMPORTANTE: esta es solo una parte del ID, luego lo demas dependera del tipo de material y sera
            //agregado o finalizado en el constructor de su respectiva clase.
            protected string GenID(string titulo, string autor, int anioPublicacion)
            {
                string[] Ptitulo = Titulo.Split(' ');
                string inicialesTitulo = (Ptitulo.Length > 1) ? string.Concat(Ptitulo.Take(2).Select(p => p.Substring(0, 1).ToUpper())) : titulo.Substring(0, Math.Min(3, titulo.Length)).ToUpper();

                string[] Pautor = autor.Split(' ');
                string inicialesautor = (Pautor.Length > 1) ? string.Concat(Pautor.Select(p => p.Substring(0, 1).ToUpper())) : autor.Substring(0, Math.Min(3, autor.Length)).ToUpper();

                return inicialesTitulo + "-" + inicialesautor + "-" + anioPublicacion.ToString();
            }


            //Modifica los detalles de una clase recibiendo el numero de seleccion y el nuevo valor.
            //las verificaciones del valor se haran posterior a la asignacion o invocacion de este metodo.
            public virtual void ModificarDetalles(int seleccion, string opcion)
            {
                switch(seleccion)
                {
                    case 1:
                        Titulo = opcion;
                        break;
                    case 2:
                        Autor = opcion;
                        break;
                    case 3:
                        AnioPublicacion = int.Parse(opcion);
                        break;
                    case 4:
                        Categoria = (Categorias)int.Parse(opcion);
                        break;
                }
            }
        }

        public class Libro : MaterialBiblioteca
        {
            [JsonInclude]
            public int NumPaginas { get; private set; }

            public Libro(Categorias categoria, string titulo, string autor, int publicacion, int numpaginas)
            {
                Titulo = titulo;
                Autor = autor;
                NumPaginas = numpaginas;
                AnioPublicacion = publicacion;
                ID = "L"+ "-" + GenID(titulo, autor, publicacion);
                Categoria = categoria;
                Status = true;
            }

            public override void ModificarDetalles(int seleccion, string opcion)
            {
                base.ModificarDetalles(seleccion, opcion);
                ID = "L" + "-" + GenID(Titulo, Autor, AnioPublicacion);
            }
            public override void MostrarDetalles()
            {
                base.MostrarDetalles();
                Console.WriteLine($"Numero de paginas: {NumPaginas}");

            }

            public Libro() { }
        }

        public class Revista : MaterialBiblioteca
        {
            [JsonInclude]
            public int Edicion { get; private set; }

            public Revista(Categorias categoria, string titulo, string autor, int publicacion, int edicion)
            {
                Titulo = titulo;
                Autor = autor;
                Edicion = edicion;
                AnioPublicacion = publicacion;
                ID = "R" + "-" + GenID(titulo, autor, publicacion);
                Categoria = categoria;
                Status = true;
            }
            public override void ModificarDetalles(int seleccion, string opcion)
            {
                base.ModificarDetalles(seleccion, opcion);
                ID = "R" + "-" + GenID(Titulo, Autor, AnioPublicacion);
            }
            public override void MostrarDetalles()
            {
                base.MostrarDetalles();
                Console.WriteLine($"Edicion: {Edicion}");

            }

            public Revista() { }


        }

        public class MaterialDigital : MaterialBiblioteca
        {
            [JsonInclude]
            public string Formato { get; private set; }

            public MaterialDigital(Categorias categoria, string titulo, string autor, int publicacion, string formato)
            {
                Titulo = titulo;
                Autor = autor;
                Formato = formato;
                AnioPublicacion = publicacion;
                ID = "MD" + "-" + GenID(titulo, autor, publicacion);
                Categoria = categoria;
                Status = true;
            }

            public override void ModificarDetalles(int seleccion, string opcion)
            {
                base.ModificarDetalles(seleccion, opcion);
                ID = "L" + "-" + GenID(Titulo, Autor, AnioPublicacion);
            }
            public override void MostrarDetalles()
            {
                base.MostrarDetalles();
                Console.WriteLine($"Formato: {Formato}");

            }

            public MaterialDigital() { }
        }

        public class Biblioteca
        {
            public List<MaterialBiblioteca> Materiales { get; private set; } = new List<MaterialBiblioteca>();

            public bool AgregarMaterial(MaterialBiblioteca material)
            {
                if (Materiales.Any(p => p.ID == material.ID))
                {
                    Console.WriteLine($"El material {material.Titulo} no puede ser agregado porque ya existe");
                    return false;
                }

                Materiales.Add(material);
                return true;
            }

            public MaterialBiblioteca BuscarMaterialPorTitulo(string titulo)
            {
                return Materiales.FirstOrDefault(m => m.Titulo.Equals(titulo, StringComparison.OrdinalIgnoreCase));
            }

            public List<MaterialBiblioteca> BuscarMaterialPorAutor(string autor)
            {
                return Materiales.Where(m => m.Autor.Equals(autor, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            public List<MaterialBiblioteca> BuscarMaterialPorCategoria(Categorias categoria)
            {
                return Materiales.Where(m => m.Categoria.Equals(categoria)).ToList();
            }

            public void MostrarTodosLosMateriales()
            {
                if (!Materiales.Any())
                {
                    Console.WriteLine("No hay materiales en la biblioteca");
                    return;
                }

                Materiales.ForEach(m => m.MostrarDetalles());
            }

            public void PrestarMaterial(string id)
            {
                MaterialBiblioteca MPrestar = Materiales.Find(p => p.ID == id);

                if (MPrestar == null)
                {
                    Console.WriteLine($"El material con el ID: {id} no existe.");
                    return;
                }

                if (MPrestar.Status == false)
                {
                    Console.WriteLine($"Error: El material {MPrestar.ID} ya se encuentra prestado.");
                    return;
                }

                MPrestar.Prestar();
                Console.WriteLine($"El material {id} fue prestado con exito.");
            }

            public void DevolverMaterial(string id)
            {
                MaterialBiblioteca MDevolver = Materiales.Find(p => p.ID == id);

                if (MDevolver == null)
                {
                    Console.WriteLine($"El material con el ID: {id} no existe.");
                    return;
                }

                if (!MDevolver.Status == true)
                {
                    Console.WriteLine($"Error: El material {id} no se encuentra prestado.");
                    return;
                }

                MDevolver.Devolver();
                Console.WriteLine($"El material {id} fue devuelto con exito.");
            }

            public List<MaterialBiblioteca> MaterialesPrestados()
            {
                return Materiales.Where(p => p.Status == false).ToList();
            }

            public List<MaterialBiblioteca> MaterialesDisponibles()
            {
                return Materiales.Where(p => p.Status == true).ToList();
            }

            public void GuardarLista()
            {
                string rutaArchivo = Path.Combine(Path.GetTempPath(), "Biblioteca2.json");

                var opciones = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    IncludeFields = true,
                    PropertyNameCaseInsensitive = true,
                    TypeInfoResolver = new DefaultJsonTypeInfoResolver()

                };
                string json = JsonSerializer.Serialize(Materiales, opciones);
                File.WriteAllText(rutaArchivo, json);

                Console.WriteLine("El archivo fue guardado con exito...");
            }

            public void CargarLista()
            {
                string rutaArchivo = Path.Combine(Path.GetTempPath(), "Biblioteca2.json");

                if (File.Exists(rutaArchivo))
                {
                    var opciones = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                    };
                    string json = File.ReadAllText(rutaArchivo);
                    Materiales = JsonSerializer.Deserialize<List<MaterialBiblioteca>>(json, opciones);
                    Console.WriteLine("Archivo cargado con exito...");
                }
                else
                {
                    Console.WriteLine("Error: No se encontro ningun archivo");
                }
            }

        }

        public static class Validaciones
        {
            public static void Esperar()
            {
                Console.WriteLine("\nPreciona cualquier tecla para continuar");
                Console.ReadKey();
            }

            public static int ValidarInt(string num, int maxvalue)
            {
                if (string.IsNullOrEmpty(num) || !int.TryParse(num, out int value) || int.Parse(num) > maxvalue || int.Parse(num) <= 0)
                {
                    throw new ArgumentNullException("El valor ingresado es invalido o nulo");
                }

                return int.Parse(num);
            }

            public static string ValidarString(string str)
            {
                if (string.IsNullOrEmpty(str))
                {
                    throw new ArgumentNullException("El valor ingresado es nulo o invalido.");
                }
                return str;
            }


        }

        public static class InterfazdeUsuario
        {
            public static void MenuPrincipal()
            {
                Console.Clear();
                Console.WriteLine("Selecciona una opcion:\n" +
                    "1. Agregar nuevo material\n" +
                    "2. Buscar material por titulo\n" +
                    "3. Buscar Materiales por autor\n" +
                    "4. Buscar Materiales por categoria\n" +
                    "5. Mostrar todos los materiales\n" +
                    "6. Modificar detalles de un material\n" +
                    "7. Prestar material\n" +
                    "8. Devolver material\n" +
                    "9. Guardar / Cargar en Json\n" +
                    "10. Salir.\n");
            }

            public static void AgregarMaterial(Biblioteca biblioteca)
            {
                Console.Clear();
                Console.WriteLine("Ingresa el tipo de material que deceas agregar:\n" +
                    "1. Libro.\n" +
                    "2. Revista\n" +
                    "3. Material Digital.\n");
                int seleccion = Validaciones.ValidarInt(Console.ReadLine(), 3);

                Console.Clear();
                Console.WriteLine("Ingresa el titulo del material");
                string titulo = Validaciones.ValidarString(Console.ReadLine());

                Console.Clear();
                Console.WriteLine("Ingresa el autor del material");
                string autor = Validaciones.ValidarString(Console.ReadLine());

                Console.Clear();
                Console.WriteLine("Ingresa el anio de publicacion del material");
                int anio = Validaciones.ValidarInt(Console.ReadLine(), DateTime.Now.Year);

                Console.Clear();
                Console.WriteLine("Selecciona la categoria del material");
                foreach (Categorias cat in Enum.GetValues(typeof(Categorias)))
                {
                    Console.WriteLine($"{(int)cat} {cat}");
                }
                int SeleCategoria = Validaciones.ValidarInt(Console.ReadLine(), 3);
                Console.Clear();

                switch(seleccion)
                {
                    case 1:
                        Console.WriteLine("Ingresa el numero de paginas del libro");
                        int numPaginas = Validaciones.ValidarInt(Console.ReadLine(), int.MaxValue);
                        Libro NuevoLibro = new Libro((Categorias)SeleCategoria, titulo, autor, anio, numPaginas);
                        biblioteca.AgregarMaterial(NuevoLibro);
                        break;
                    case 2:
                        Console.WriteLine("Ingresa la edicion de la revista");
                        int edicion = Validaciones.ValidarInt(Console.ReadLine(), int.MaxValue);
                        Revista NuevaRevista = new Revista((Categorias)SeleCategoria, titulo, autor, anio, edicion);
                        biblioteca.AgregarMaterial(NuevaRevista);
                        break;
                    case 3:
                        Console.WriteLine("Ingresa el formato del material digital");
                        string formato = Validaciones.ValidarString(Console.ReadLine());
                        MaterialDigital NuevoMaterial = new MaterialDigital((Categorias)SeleCategoria, titulo, autor, anio, formato);
                        biblioteca.AgregarMaterial(NuevoMaterial);
                        break;
                }

                Console.WriteLine("Material agregado con exito.");
            }

            public static void BuscarMaterialPorTitulo(Biblioteca biblioteca)
            {
                Console.Clear();
                Console.WriteLine("Por favor ingresa el titulo del material");
                string titulo = Validaciones.ValidarString(Console.ReadLine());
                var temp = biblioteca.BuscarMaterialPorTitulo(titulo);

                if (temp != null)
                {
                    Console.WriteLine("Material encontrado:");
                    temp.MostrarDetalles();
                    return;
                }

                throw new ArgumentNullException($"Error: El material de titulo {titulo} no existe");

            }

            public static void BuscarMaterialesPorAutor(Biblioteca biblioteca)
            {
                Console.Clear();
                Console.WriteLine("Ingresa el nombre del autor que deceas buscar");
                string autor = Validaciones.ValidarString(Console.ReadLine());

                var temp = biblioteca.BuscarMaterialPorAutor(autor);

                if (temp != null && temp.Count > 0)
                {
                    Console.WriteLine($"Se encontraron {temp.Count} resultados para {autor}:");
                    temp.ForEach(p => p.MostrarDetalles());
                    return;
                }

                throw new ArgumentNullException($"Error: No se encontro ningun material del autor {autor}");
            }

            public static void BuscarMaterialesPorCategoria(Biblioteca biblioteca)
            {
                Console.Clear();
                Console.WriteLine("Por favor selecciona la categoria:");
                foreach (Categorias cat in Enum.GetValues(typeof(Categorias)))
                {
                    Console.WriteLine($"{(int)cat} {cat}");
                }

                int seleccion = Validaciones.ValidarInt(Console.ReadLine(), 3);
                Console.Clear();

                Console.WriteLine("Materiales encontrados: \n");
                var MaterialesCategoria = biblioteca.BuscarMaterialPorCategoria((Categorias)seleccion);

                if (MaterialesCategoria != null && MaterialesCategoria.Count > 0)
                {
                    MaterialesCategoria.ForEach(p => p.MostrarDetalles());
                    return;
                }

                throw new ArgumentNullException($"Error: No se encontro ningun material de la categoria {(Categorias)seleccion}");
            }

            public static void MostrarTodosLosMateriales(Biblioteca biblioteca)
            {
                Console.Clear();
                if (biblioteca.Materiales.Count > 0)
                {
                    biblioteca.Materiales.ForEach(p => p.MostrarDetalles());
                    return;
                }

                throw new Exception($"Error: No existen materiales en la biblioteca");
            }

            public static void PrestarMaterial(Biblioteca biblioteca)
            {
                Console.Clear();
                Console.WriteLine("Ingresa el ID del naterial que deceas prestar");
                biblioteca.PrestarMaterial(Validaciones.ValidarString(Console.ReadLine()));

            }

            public static void DevolverMaterial(Biblioteca biblioteca)
            {
                Console.Clear();
                Console.WriteLine("Ingresa el ID del material que deceas devolver");
                biblioteca.DevolverMaterial(Validaciones.ValidarString(Console.ReadLine()));
            }

            public static void MenuModificarDetalles(Biblioteca biblioteca)
            {
                Console.Clear();
                Console.WriteLine("Ingresa el ID del material que deceas modificar");
                string id = Validaciones.ValidarString(Console.ReadLine());

                MaterialBiblioteca material = biblioteca.Materiales.Find(p => p.ID == id);

                if (material == null)
                {
                    throw new ArgumentNullException($"Error: el material con el ID {id} no existe.");
                }

                Console.Clear();
                Console.WriteLine("Selecciona la opcion que deceas modificar:\n" +
                    "1. Titulo\n" +
                    "2. Autor\n" +
                    "3. Anio de publicacion\n" +
                    "4. Categoria\n");
                int seleccion = Validaciones.ValidarInt(Console.ReadLine(), 4);

                Console.Clear();
                switch (seleccion)
                {
                    case 1:
                        Console.WriteLine("Ingresa el nuevo titulo del material");
                        string opcion1 = Validaciones.ValidarString(Console.ReadLine());
                        material.ModificarDetalles(seleccion, opcion1);
                        break;
                    case 2:
                        Console.WriteLine("Ingresa el nuevo autor del material");
                        string opcion2 = Validaciones.ValidarString(Console.ReadLine());
                        material.ModificarDetalles(seleccion, opcion2);
                        break;
                    case 3:
                        Console.WriteLine("Ingresa el nuevo anio de publicacion del material");
                        string opcion3 = Validaciones.ValidarInt(Console.ReadLine(), DateTime.Now.Year).ToString();
                        material.ModificarDetalles(seleccion, opcion3);
                        break;
                    case 4:
                        Console.WriteLine("Selecciona la nueva categoria ");
                        foreach (Categorias var in Enum.GetValues(typeof(Categorias)))
                        {
                            Console.WriteLine($"{(int)var} {(var)}");
                        }
                        string opcion4 = Validaciones.ValidarInt(Console.ReadLine(), 3).ToString();
                        material.ModificarDetalles(seleccion, opcion4);
                        break;
                }

                Console.Clear();
                Console.WriteLine("Material modificado con exito.");
            }

            public static void GuardarOCargar(Biblioteca biblioteca)
            {
                Console.Clear();
                Console.WriteLine("Selecciona una accion:\n" +
                    "1. Guardar\n" +
                    "2. Cargar.");

                int seleccion = Validaciones.ValidarInt(Console.ReadLine(), 2);

                if (seleccion == 1)
                {
                    biblioteca.GuardarLista();
                    return;
                }
                else
                {
                    biblioteca.CargarLista();
                    return;
                }

            }


        }


        static void Main(string[] args)
        {
            Biblioteca biblioteca1 = new Biblioteca();

            while (true)
            {
                Console.Clear();
                InterfazdeUsuario.MenuPrincipal();

                try
                {
                    int seleccion = Validaciones.ValidarInt(Console.ReadLine(), 10);
                    switch (seleccion)
                    {
                        case 1: //Agregar Material
                            InterfazdeUsuario.AgregarMaterial(biblioteca1);
                            break;
                        case 2: //Buscar Material por titulo.
                            InterfazdeUsuario.BuscarMaterialPorTitulo(biblioteca1);
                            break;
                        case 3: //Buscar Materiales por autor.
                            InterfazdeUsuario.BuscarMaterialesPorAutor(biblioteca1);
                            break;
                        case 4: //Buscar Materiales por categoria.
                            InterfazdeUsuario.BuscarMaterialesPorCategoria(biblioteca1);
                            break;
                        case 5: // Mostrar todos los materiales
                            InterfazdeUsuario.MostrarTodosLosMateriales(biblioteca1);
                            break;
                        case 6: // Modificar detalles de un material.
                            InterfazdeUsuario.MenuModificarDetalles(biblioteca1);
                            break;
                        case 7: // Prestar Material
                            InterfazdeUsuario.PrestarMaterial(biblioteca1);
                            break;
                        case 8: //Devolver Material.
                            InterfazdeUsuario.DevolverMaterial(biblioteca1);
                            break;
                        case 9: //Guardar / Cargar.
                            InterfazdeUsuario.GuardarOCargar(biblioteca1);
                            break;
                        case 10:
                            break;
                    }

                    Validaciones.Esperar();
                    if (seleccion == 10)
                    {
                        break;
                    }


                }
                catch (ArgumentNullException ez)
                {
                    Console.WriteLine(ez.Message);
                    Validaciones.Esperar();
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    Validaciones.Esperar();
                }
                catch (Exception f)
                {
                    Console.WriteLine(f.Message);
                    Validaciones.Esperar();
                }







            }




        }

    }
}
