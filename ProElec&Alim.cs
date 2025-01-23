using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleApp
{
    internal class Program
    {
        public interface IMostrable
        {
            void MostrarInformacion();
        } 

        public abstract class Producto : IMostrable
        {
            public string Nombre { get; protected set; }

            public string Descripcion { get; protected set; }
            public decimal Precio { get; protected set; }

            public virtual void MostrarInformacion()
            {
                Console.WriteLine($"Nombre del producto: {Nombre}" +
                    $"\nPrecio del producto: {Precio}" +
                    $"\nDescripcion: {Descripcion}");
            }

            public virtual void Constructor(string nombre, decimal precio, string descripcion)
            {
                Nombre = (!string.IsNullOrEmpty(nombre)) ? nombre : throw new ArgumentException("Nombre nulo o invalido");
                Precio = (!string.IsNullOrEmpty(precio.ToString()) && precio > 0) ? precio : throw new ArgumentException("Precio nulo o invalido");
                Descripcion = (!string.IsNullOrEmpty(descripcion)) ? descripcion : throw new ArgumentException("Descripcion nula o invalida");
            }
        }

        public class ProductoElectronico : Producto
        {
            public int Garantia { get; protected set; }

            public ProductoElectronico(string nombre, decimal precio,  int garantia, string descripcion)
            {
                base.Constructor(nombre, precio, descripcion);
                Garantia = (!string.IsNullOrEmpty(garantia.ToString()) && garantia >= 0) ? garantia : throw new FormatException("Garantia nula o invalida)"); 
            }

            public override void MostrarInformacion()
            {
                base.MostrarInformacion();
                Console.WriteLine($"Meses de garantia: {Garantia}\n");
            }

        }

        public class ProductoAlimenticio : Producto
        {
            public DateTime FechaDeExpiracion { get; protected set; }

            public ProductoAlimenticio(string nombre, decimal precio,  string fechaDeExpiracion, string descripcion)
            {
                base.Constructor(nombre, precio, descripcion);
                FechaDeExpiracion = (!DateTime.TryParseExact(fechaDeExpiracion, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime c) && c > DateTime.Now) ? FechaDeExpiracion : throw new FormatException("Fecha de vencimiento incorrecta o invalida"); 
            }

            public override void MostrarInformacion()
            {
                base.MostrarInformacion();
                Console.WriteLine($"Fecha de expiracion: {FechaDeExpiracion}\n");
            }
        }

        public static class  Interfaz
        {
            public static string ReadResult;
            public static int Seleccion;
            public static bool Salir = true;

            public static void Input(string input)
            {
                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int seleccion))
                {
                    Seleccion = seleccion;
                }
                else if (input.ToLower().Trim() == "salir")
                {
                    Salir = false;
                }
                else
                {
                    throw new FormatException("Seleccion nula o invalida");
                }



            }
            public static void Bienvenida()
            {
                Console.Clear();
                Console.WriteLine("Selecciona una opcion: (1 - 2)\n" +
                    "1. Crear producto.\n" +
                    "2. Ver lista de productos.\n" +
                    "[Salir] para salir\n");
                ReadResult = Console.ReadLine();
                Input(ReadResult);
            }

            public static void CrearProducto()
            {
                Console.Clear();
                Console.WriteLine("Selecciona el tipo de producto que deceas agregar: (1 - 2)\n" +
                    "1. Producto Alimenticio.\n" +
                    "2. Producto Electronico\n" +
                    "[Salir] para salir");
                ReadResult = Console.ReadLine();
                Input(ReadResult);
            }

            public static ProductoAlimenticio ProdAlimenticio()
            {
                Console.Clear();
                Console.WriteLine("Ingresa un nombre");
                string nombre = Console.ReadLine();
                Console.WriteLine("Ingresa un precio");
                decimal precio = Convert.ToDecimal(Console.ReadLine());
                Console.WriteLine("Ingresa la fecha de expiracion (dd/MM/yyyy)");
                string fecha = Console.ReadLine();
                Console.WriteLine("Ingresa una descripcion");
                string descripcion = Console.ReadLine();

                ProductoAlimenticio NuevoProducto = new ProductoAlimenticio(nombre, precio, fecha, descripcion);
                return NuevoProducto;
            }

            public static ProductoElectronico ProdEletronico()
            {
                Console.Clear();
                Console.WriteLine("Ingresa un nombre");
                string nombre = Console.ReadLine();
                Console.WriteLine("Ingresa un precio");
                decimal precio = Convert.ToDecimal(Console.ReadLine());
                Console.WriteLine("Ingresa la garantia en meses");
                int garantia = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Ingresa la descripcion");
                string descripcion = Console.ReadLine();

                ProductoElectronico NuevoProducto = new ProductoElectronico(nombre, precio, garantia, descripcion);
                return NuevoProducto;
            }

            public static void MostrarLista(List<Producto> productos)
            {
                {
                    Console.Clear();
                    Console.WriteLine("¿Qué productos deseas mostrar? (1 - 3)\n" +
                                      "1. Productos Alimenticios\n" +
                                      "2. Productos Electrónicos\n" +
                                      "3. Todos los productos\n" +
                                      "[Salir] para salir");
                    string opcion = Console.ReadLine();

                    if (opcion.ToLower().Trim() == "salir")
                    {
                        return;
                    }

                    switch (opcion)
                    {
                        case "1": // Mostrar solo productos alimenticios
                            var productosAlimenticios = productos.OfType<ProductoAlimenticio>().ToList();
                            if (productosAlimenticios.Count > 0)
                            {
                                foreach (var producto in productosAlimenticios)
                                {
                                    producto.MostrarInformacion();
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay productos alimenticios en la lista.");
                            }
                            break;

                        case "2": // Mostrar solo productos electrónicos
                            var productosElectronicos = productos.OfType<ProductoElectronico>().ToList();
                            if (productosElectronicos.Count > 0)
                            {
                                foreach (var producto in productosElectronicos)
                                {
                                    producto.MostrarInformacion();
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay productos electrónicos en la lista.");
                            }
                            break;

                        case "3": // Mostrar todos los productos
                            if (productos.Count > 0)
                            {
                                foreach (Producto producto in productos)
                                {
                                    producto.MostrarInformacion();
                                }
                            }
                            else
                            {
                                Console.WriteLine("No hay ningún producto en la lista.");
                            }
                            break;

                        default:
                            Console.WriteLine("Opción inválida. Intenta nuevamente.");
                            break;
                    }

                    Console.WriteLine("Presiona cualquier tecla para continuar...");
                    Console.ReadKey();
                }
            }
        }

        static void Main(string[] args)
        {
            List<Producto> productos = new List<Producto>();

            void guardar()
            {
                string rutaArchivo = Path.Combine(Path.GetTempPath(), "productos.json");
                string json = JsonSerializer.Serialize(productos, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(rutaArchivo, json);
            }

            while (Interfaz.Salir)
            {
                Interfaz.Bienvenida();

                switch (Interfaz.Seleccion)
                {
                    case 1:
                        {
                            Interfaz.CrearProducto();
                            switch (Interfaz.Seleccion)
                            {
                                case 1:
                                    {
                                        productos.Add(Interfaz.ProdAlimenticio());
                                        guardar();
                                        
                                    }
                                    break;
                                case 2:
                                    {
                                        productos.Add(Interfaz.ProdEletronico());
                                        guardar();
                                    }
                                    break;
                            }
                        }
                        break;
                    case 2:
                        {
                            Interfaz.MostrarLista(productos);
                        }
                        break;
                }
            }




            

        }
    }
}