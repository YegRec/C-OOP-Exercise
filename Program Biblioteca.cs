using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Xml;

namespace ConsoleApp1
{
    internal class Program
    {
        private class GeneradorID
        {
            private static int IDusuario { get; set; }
            private static int IDLibro { get; set; }

            public static string GenIDUsuario(string nombre)
            {
                IDusuario++;
                return Biblioteca.Iniciales() + "US" + nombre.Substring(0, 1) + IDusuario;
            }

            public static string GenIDLibro(string titulo, string autor)
            {
                IDLibro++;
                return Biblioteca.Iniciales() + autor.Substring(0, 1) + titulo.Substring(0, 1) + IDLibro; ;
            }
           

        }
        public class Biblioteca
        {
            private List<Usuario> Usuarios = new List<Usuario>();
            private List<Libro> Libros = new List<Libro>();
            private static string Nombre { get; set; }
            public static string Iniciales()
            {
                return Nombre.Substring(0, 1);
            }

            public Biblioteca(string nombre)
            {
                Nombre = (!string.IsNullOrEmpty(nombre)) ? nombre : throw new ArgumentException("Nombre de biblioteca Nulo o incorrecto");
            }

            public void AgregarLibro(string titulo, string autor)
            {
                if (!string.IsNullOrEmpty(titulo) && !string.IsNullOrEmpty(autor))
                {
                    if (!Libros.Exists(p => p.Titulo == titulo))
                    {
                        Libro NuevoLibro = new Libro(autor, titulo);
                        Libros.Add(NuevoLibro);
                    }
                    else
                    {
                        Console.WriteLine($"\nEl libro {titulo} ya se encuentra registrado\n");
                    }
                }
                else
                {
                    throw new ArgumentException("Verificar: Titulo o autor son nulos o incorrectos");
                }
            }

            public void EliminarLibro(string titulo)
            {
                if (!string.IsNullOrEmpty(titulo))
                {
                    Libro BuscaLibro = Libros.Find(p => p.Titulo == titulo);
                    if (BuscaLibro != null && BuscaLibro.Disponible == true)
                    {
                        Libros.Remove(BuscaLibro);
                        Console.WriteLine($"El libro {titulo} fue removido con exito");
                    }
                    else if (BuscaLibro != null && BuscaLibro.Disponible == false)
                    {
                        Console.WriteLine($"\nError: {titulo} no puede ser eliminado. Se encuentra prestado\n");
                    }
                    else
                    {
                        Console.WriteLine($"\nEl libro {titulo} no existe\n");
                    }
                }
                else
                {
                    throw new ArgumentException("Nombre del libro nulo o invalido");
                }
            }

            public void RegistrarUsuario(string nombre)
            {
                if (!string.IsNullOrEmpty(nombre))
                {
                    Usuario verificacion = Usuarios.Find(p => p.Nombre == nombre);
                    if (verificacion != null)
                    {
                        Console.WriteLine($"\nEl usuario {nombre} ya se encuentra registrado\n");
                    }
                    else
                    {
                        Usuario NuevoUsuario = new Usuario(nombre);
                        Usuarios.Add(NuevoUsuario);
                    }
                }
                else
                {
                    throw new ArgumentException("Nombre de usuario nulo o invalido");
                }
            }

            public void PrestarLibro(string titulo, string nombre)
            {
                if (!string.IsNullOrEmpty(titulo) && !string.IsNullOrEmpty(nombre))
                {
                    Libro Prestar = Libros.Find(p => p.Titulo == titulo);
                    if (Prestar != null)
                    {
                        if (Prestar.Disponible == true)
                        {
                            Usuario Usu = Usuarios.Find(p => p.Nombre == nombre);
                            if (Usu != null)
                            {
                                if (Usu.LibrosPrestados.Count < 3)
                                {
                                    Usu.LibrosPrestados.Add(Prestar);
                                    Prestar.Disponible = false;
                                    Console.WriteLine($"El libro {titulo} fue prestado a {nombre} con exito");

                                }
                                else
                                {
                                    Console.WriteLine($"\nEl usuario {nombre} excedio el limite de libros prestados\n");
                                }



                            }
                            else
                            {
                                Console.WriteLine($"\nEl usuario {nombre} no existe.\n");
                            }


                        }
                        else
                        {
                            Console.WriteLine($"\nEl libro {titulo} se encuentra prestado\n");
                        }


                    }
                    else
                    {
                        Console.WriteLine($"\nEl libro {titulo} no existe.\n");
                    }


                }
                else
                {
                    throw new ArgumentException("Titulo o Autor invalidos");
                }



            }

            public void DevolverLibro(string titulo, string nombre)
            {
                if (!string.IsNullOrEmpty(titulo) && !string.IsNullOrEmpty(nombre))
                {
                    Libro LibroDevuelto = Libros.Find(p => p.Titulo == titulo);
                    if (LibroDevuelto != null)
                    {
                        if (LibroDevuelto.Disponible == false)
                        {
                            Usuario Usu = Usuarios.Find(p => p.Nombre ==  nombre);
                            if (Usu != null)
                            {
                                if (Usu.LibrosPrestados.Contains(LibroDevuelto))
                                {
                                    Usu.LibrosPrestados.Remove(LibroDevuelto);
                                    LibroDevuelto.Disponible = true;
                                    Console.WriteLine($"El usuario {nombre} ha devuelto el libro {titulo} con exito");


                                }
                                else
                                {
                                    Console.WriteLine($"\nEl usuario {nombre} no tiene el libro {titulo} prestado");
                                }


                            }
                            else
                            {
                                Console.WriteLine($"\nEl usuario {nombre} no existe\n");
                            }

                        }
                        else
                        {
                            Console.WriteLine($"\nEl libro {titulo} no se encuentra prestado\n");
                        }

                    }
                    else
                    {
                        Console.WriteLine($"\nEl libro {titulo} no existe.\n");
                    }

                }
                else
                {
                    throw new ArgumentException("Nombre o titulo invalidos");
                }
            }

            public void MostrarLibros()
            {
                if (Libros.Count > 0)
                {
                    foreach (Libro Libro in Libros)
                    {
                        Console.WriteLine($"\nTitulo: {Libro.Titulo}\n" +
                            $"ID: {Libro.ID}\n" +
                            $"Autor: {Libro.Autor}\n" +
                            $"Disponible: {Libro.Disponible}\n");
                    }
                }
                else
                {
                    Console.WriteLine($"No hay ningun libro registrado.");
                }
            }

            public void LibrosPrestados(string nombre)
            {
                if (!string.IsNullOrEmpty(nombre))
                {
                    Usuario Usu = Usuarios.Find(p => p.Nombre == nombre);

                    if (Usu != null)
                    {
                        if (Usu.LibrosPrestados.Count > 0)
                        {
                            Console.WriteLine($"El usuario {nombre} tiene:\n");
                            foreach (Libro Libro in Usu.LibrosPrestados)
                            {
                                Console.WriteLine($"\nTitulo: {Libro.Titulo}\n" +
                                    $"ID: {Libro.ID}\n" +
                                    $"Autor: {Libro.Autor}\n");

                            }

                        }
                        else
                        {
                            Console.WriteLine($"El usuario {nombre} no tiene libros prestados");
                        }


                    }
                    else
                    {
                        Console.WriteLine($"\nEl usuario {nombre} no existe\n");
                    }
                }
                else
                {
                    throw new ArgumentException("Nombre Invalido o Nulo");
                }

            }
        }

        private class Usuario
        {
            public List<Libro> LibrosPrestados = new List<Libro>();

            public string Nombre { get; private set; }
            public string ID { get; private set; }

            public Usuario(string nombre)
            {
                Nombre = nombre;
                ID = GeneradorID.GenIDUsuario(nombre);
                Console.WriteLine($"Usuario {nombre} registrado existosamente");
            }

        }

        private class Libro
        {
            public string Autor { get; private set; }
            public string Titulo {  get; private set; }
            public string ID { get; private set; }

            public bool Disponible = true;

            public Libro(string autor,  string titulo)
            {
                Autor = autor;
                Titulo = titulo;
                ID = GeneradorID.GenIDLibro(titulo, autor);
                Console.WriteLine($"El libro {titulo} fue agregado con exito");
            }
           
            

        }
        static void Main(string[] args)
        {
            Biblioteca biblioteca = new Biblioteca("Cristal");

            biblioteca.AgregarLibro("Daila Negra", "Jose Alfredo");
            biblioteca.AgregarLibro("Diamante de Sangre", "Alfa Romeo");
            biblioteca.AgregarLibro("Charlie Charlie", "Kendrick Lamar");
            biblioteca.AgregarLibro("The Giver", "Erick Bengosme");



            biblioteca.RegistrarUsuario("Jorge");

            biblioteca.PrestarLibro("Daila Negra", "Jorge");
            biblioteca.PrestarLibro("Charlie Charlie", "Jorge");
            biblioteca.PrestarLibro("The Giver", "Jorge");

            biblioteca.DevolverLibro("The Giver", "Jorge");

            biblioteca.MostrarLibros();

            biblioteca.LibrosPrestados("Jorge");




        }
    }
}

