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
        public enum Categorias
        {
            Electronica = 1,
            Ropa = 2,
            Alimentos = 3,
            Juguetes = 4,
            Hogar = 5,
        }


        public class Producto
        {
            /// <summary>
            /// Clase producto maneja el objeto producto y sus caracteristicas y/o parametros.
            /// usa la clase ID: para asignar un ID unico al producto incluyendo la primera letra de su categoria
            /// las 2 primeras letras de su nombre y el primer numero de su precio mas la letra 'P' seguido de un '-'
            /// todo al final agregado en mayusculas.
            /// 
            /// 
            /// El metodo newStock es usado para asignar/modificar el stock actual del producto.
            /// </summary>
            public string ID { get; private set; }
            public string Nombre { get; private set; }
            public Categorias Categoria { get; private set; }
            public double Precio { get; private set; }
            public int Stock { get; private set; }

            public Producto(string nombre, Categorias categoria, double precio, int stock)
            {
                ID = "P-" + categoria.ToString().Substring(0, 1) + nombre.Substring(0, 2).ToUpper() + Math.Floor(precio).ToString().Substring(0, 1);
                Nombre = nombre;
                Categoria = categoria;
                Precio = precio;
                Stock = stock;
            }

            public void newStock(int stock)
            {
                Stock = stock;
            }

            public void MostrarDetalles()
            {
                Console.WriteLine($"Nombre: {Nombre}\n" +
                    $"Categoria: {Categoria}\n" +
                    $"Precio: {Precio}\n" +
                    $"ID: {ID}\n" +
                    $"Stock: {Stock}\n");
            }
        }


        public class Tienda
        {
            /// <summary>
            /// Clase tienda maneja la lista de productos de la tienda, caracteristicas como:
            /// 
            /// - Agregar producto
            /// - Eliminar producto
            /// - Buscar producto
            /// - Modificar parametros del producto
            /// 
            /// </summary>
            private List<Producto> Productos { get; set; } = new List<Producto>();

            public void AgregarProducto(Producto producto)
            {
                if (Productos.Exists(p => p.ID == producto.ID || p.Nombre == producto.Nombre))
                {
                    throw new InvalidOperationException("Error, el producto ya existe");
                }
                Productos.Add(producto);
            }

            public void BuscarProductoPorNombre(string producto)
            {
                Producto BuscaProducto = Productos.Find(p => p.Nombre == producto);

                if (BuscaProducto == null)
                {
                    Console.WriteLine($"El producto {producto} no existe.");
                }
                else
                {
                    Console.Clear();
                    BuscaProducto.MostrarDetalles();
                }
            }


            public Producto BuscarProductoPorID(string ID)
            {
                Producto BuscaID = Productos.Find(p => p.ID == ID);
                if (BuscaID == null)
                {
                    throw new ArgumentException($"El producto con ID {ID} no existe.");
                }
                return BuscaID;
            }

            //Este metodo en especifico crea una lista para almecenar los productos que coinciden con la
            //categoria seleccionada, luego dependiendo la cantidad de productos encontrados ejecuta una accion o otra
            //Si no encuentra ningun producto de la categoria ejecuta un mensaje.
            //Si encuentra 1 o mas imprimira los detalles de cada producto encontrado.
            public void BuscarProductoPorCategoria(Categorias categoria)
            {
                var ListaProducto = new List<Producto>();

                foreach (var producto in Productos)
                {
                    if (producto.Categoria == categoria)
                    {
                        ListaProducto.Add(producto);
                    }
                }

                if (ListaProducto.Count == 0)
                {
                    Console.WriteLine($"No se encontraron productos en la categoria {categoria}");
                    return;
                }

                Console.WriteLine($"Se encontraron {ListaProducto.Count} productos en la categoria {categoria}");
                foreach (var Prod in ListaProducto)
                {
                    Prod.MostrarDetalles();
                }

            }

            public void MostrarTodosLosProductos()
            {
                if (Productos.Count > 0)
                {
                    foreach (var producto in Productos)
                    {
                        producto.MostrarDetalles();
                    }
                }
                else
                {
                    Console.WriteLine("Error: No existen productos.");
                }
            }

            public void ActualizarStock(string id, int stock)
            {
                BuscarProductoPorID(id).newStock(stock);
            }

            public void EliminarProducto(string id)
            {
                Productos.Remove(BuscarProductoPorID(id));
            }

        }

        public static class Validaciones
        {
            /// <summary>
            /// Clase validaciones contiene metodos que verifican los inputs que ingresa el usuario y verifica
            /// que sean correctos o funcionales para el programa.
            /// 
            /// 
            /// Inputnum:
            /// 
            /// Verifica que el input ingresado por el usuario sea un numero.
            /// Tiene 3 overloads:
            /// 
            /// string input: hace la verificacion y retorna un int.
            /// string input y overload: hace la verificacion y retorna un double.
            /// string input y int maxvalue: hace la verificacion y ademas verifica que el numero
            /// ingresado por el usuario no sea mayor al valor asignado a maxvalue.
            /// </summary>
            /// 

            public static int Inputnum(string input)
            {
                if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int value))
                {
                    throw new ArgumentException("El valor ingresado es nulo o invalido");
                }

                return int.Parse(input);
            }

            public static double Inputnum(string input, string overloadmethod)
            {
                if (string.IsNullOrEmpty(input) || !double.TryParse(input, out double value) || double.Parse(input) <= 0)
                {
                    throw new ArgumentException("El valor ingresado es nulo o invalido");
                }

                return double.Parse(input);
            }

            public static int Inputnum(string input, int maxvalue)
            {
                if (string.IsNullOrEmpty(input) || !int.TryParse(input, out int value) || int.Parse(input) > maxvalue || int.Parse(input) <= 0)
                {
                    throw new ArgumentException("El valor ingresado es nulo o invalido");
                }

                return int.Parse(input);
            }

            public static string InputString(string input)
            {
                if (string.IsNullOrEmpty(input) || input.Length == 0)
                {
                    throw new ArgumentException("El valor ingresado es nulo o invalido");
                }
                return input;
            }

            public static void Continuar()
            {
                Console.WriteLine("Preciona cualquier tecla para continuar");
                Console.ReadLine();
            }

        }

        /// <summary>
        /// Clase interfazUSuario maneja todo lo relacionado con interfaces y interacciones con el usuario.
        /// 
        /// Funciones o metodos:
        /// 
        /// MenuPrincipal: Maneja el menu principal y la seleccion del usuario.
        /// MenuCrearProducto: Maneja la interfaz y las interacciones del usuario al crear un producto.
        /// MenuBuscarPorCategoria: Implementa la interfaz del usuario al buscar un producto por categoria.
        /// MenuBuscarPorNombre: Implementa la interfaz del usuario al buscar un producto por su nombre.
        /// MenuActualizarStock: Maneja la interfaz de usuario y las interacciones para actualizar el stock de un producto.
        /// MenuEliminarProducto: Interfaz para eliminar un producto de la tienda.
        /// </summary>
        public static class InterfazUsuario
        {
            public static void MenuPrincipal()
            {
                Console.Clear();
                Console.WriteLine("Selecciona una opcion:\n" +
                    "1. Agregar Producto\n" +
                    "2. Buscar producto por nombre\n" +
                    "3. Buscar productos por categoria\n" +
                    "4. Mostrar todos los productos\n" +
                    "5. Actualizar Stock\n" +
                    "6. Eliminar producto\n" +
                    "7. Salir.\n");
            }

            public static void MenuCrearProducto(Tienda tienda)
            {
                Console.Clear();
                Console.WriteLine("Ingresa el nombre del producto");
                string nombre = Validaciones.InputString(Console.ReadLine());

                if (nombre.Length <= 1)
                {
                    throw new ArgumentException("Error: nombre debe ser mayor a 2 letras.");
                }

                Console.Clear();
                Console.WriteLine("Ingresa la categoria del producto:\n");
                foreach (Categorias cat in Enum.GetValues(typeof(Categorias)))
                {
                    Console.WriteLine($"{(int)cat}. {cat}");
                }

                int seleccionCategoria = Validaciones.Inputnum(Console.ReadLine(), 5);
                Categorias categorias = (Categorias)seleccionCategoria;

                Console.Clear();
                Console.WriteLine("Ingresa el precio del producto");
                double precio = Validaciones.Inputnum(Console.ReadLine(), " ");

                Console.Clear();
                Console.WriteLine("Ingresa el stock del producto");
                int stock = Validaciones.Inputnum(Console.ReadLine());
                if (stock <= 0)
                {
                    throw new ArgumentException("El stock debe ser un numero entero positivo mayor a 0");
                }

                Producto NuevoProducto = new Producto(nombre, categorias, precio, stock);
                tienda.AgregarProducto(NuevoProducto);

                Console.Clear();
                Console.WriteLine("Nuevo producto agregado:\n");
                NuevoProducto.MostrarDetalles();
            }

            public static void MenuBuscarPorCategoria(Tienda tienda)
            {
                Console.Clear();
                Console.WriteLine("Selecciona la categoria que quieres buscar");
                foreach (Categorias cat in Enum.GetValues(typeof(Categorias)))
                {
                    Console.WriteLine($"{(int)cat} {cat}");
                }

                int seleccion = Validaciones.Inputnum(Console.ReadLine(), 5);
                Console.Clear();
                tienda.BuscarProductoPorCategoria((Categorias)seleccion);
            }

            public static void MenuBuscarPorNombre(Tienda tienda)
            {
                Console.Clear();
                Console.WriteLine("Ingresa el nombre del producto");
                tienda.BuscarProductoPorNombre(Validaciones.InputString(Console.ReadLine()));
            }

            public static void MenuMostrarProductos(Tienda tienda)
            {
                Console.Clear();
                tienda.MostrarTodosLosProductos();
            }

            public static void MenuActualizarStock(Tienda tienda)
            {
                Console.Clear();
                Console.WriteLine("Ingresa el ID del producto que deceas actualizar");
                string id = Validaciones.InputString(Console.ReadLine());
                tienda.BuscarProductoPorID(id).MostrarDetalles();

                Console.WriteLine("\nIngresa el nuevo stock del producto");
                tienda.ActualizarStock(id, Validaciones.Inputnum(Console.ReadLine()));
                Console.WriteLine("Stock actualizado con exito!");
            }

            public static void MenuEliminarProducto(Tienda tienda)
            {
                Console.Clear();
                Console.WriteLine("Ingresa el ID del producto que deceas eliminar");
                string id = Validaciones.InputString(Console.ReadLine());
                Console.Clear();
                tienda.BuscarProductoPorID(id).MostrarDetalles();
                Console.WriteLine("Seguro deceas eliminar el producto? Y / N");

                string seleccion = Validaciones.InputString(Console.ReadLine());

                if (seleccion.ToLower().Trim() == "y")
                {
                    Console.Clear();
                    tienda.EliminarProducto(id);
                    Console.WriteLine("Producto eliminado con exito");
                }
                else
                {
                    return;
                }

            }

        }

        static void Main(string[] args)
        {
            /// Preferi usar la clase tienda de esta forma para asi poder tener diferentes instancias
            /// de la misma. Por motivos de testeo y pruebas considero que es mejor opcion ya que permite trabajar, editar
            /// o configurar cualquier metodo sin necesidad de comprometer una instancia de Tienda principal en caso de haberla.

            Tienda tienda1 = new Tienda();


            //Utilizo un loop while para estar constantemente en el menu principal hasta que el usuario
            //seleccione la opcion salir.
            while (true)
            {
                InterfazUsuario.MenuPrincipal();


                try
                {
                    int option = Validaciones.Inputnum(Console.ReadLine(), 7);
                    switch (option)
                    {
                        case 1:
                            InterfazUsuario.MenuCrearProducto(tienda1);
                            Validaciones.Continuar();
                            break;
                        case 2:
                            InterfazUsuario.MenuBuscarPorNombre(tienda1);
                            Validaciones.Continuar();
                            break;
                        case 3:
                            InterfazUsuario.MenuBuscarPorCategoria(tienda1);
                            Validaciones.Continuar();
                            break;
                        case 4:
                            InterfazUsuario.MenuMostrarProductos(tienda1);
                            Validaciones.Continuar();
                            break;
                        case 5:
                            InterfazUsuario.MenuActualizarStock(tienda1);
                            Validaciones.Continuar();
                            break;
                        case 6:
                            InterfazUsuario.MenuEliminarProducto(tienda1);
                            Validaciones.Continuar();
                            break;
                    }

                    if (option == 7)
                    {
                        Console.WriteLine("Saliendo...");
                        Validaciones.Continuar();
                        break;
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"{ex.Message}");
                    Validaciones.Continuar();
                }
                catch (InvalidOperationException ez)
                {
                    Console.WriteLine($"{ez.Message}");
                    Validaciones.Continuar();
                }
                catch (Exception ev)
                {
                    Console.WriteLine($"Error inesperado: {ev.Message}");
                }

            }

        }

    }
}
