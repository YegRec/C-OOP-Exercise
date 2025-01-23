using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        //Lista de todos los productos disponibles para compra.
        //List of every product aviable. to buy.

        //Esta clase puede mostrar todos los productos disponibles y eliminar.
        //This class can show every product aviable and delete anyone.
        public static class ListaProductos
        {
            public static List<Producto> ListaDeProductos = new List<Producto>(); 

            public static void MostrarTodos()
            {
                if (ListaDeProductos.Count == 0)
                {
                    throw new Exception("No existen produtos");
                }
                Console.WriteLine("\nLista de Productos:");
                int count = 1;
                foreach (Producto c in ListaDeProductos)
                {
                    Console.WriteLine($"({count})" +
                        $"\nNombre: {c.Nombre}" +
                        $"\nDescripcion: {c.Descripcion}" +
                        $"\nPrecio: {c.Precio}");
                    count++;
                }
            }

            public static void EliminarProducto(string nombre)
            {
                if (string.IsNullOrEmpty(nombre))
                {
                    throw new ArgumentException("Nombre de producto a eliminar nulo o invalido");
                }

                Producto Eliminar = ListaDeProductos.Find(p => p.Nombre == nombre);

                if (Eliminar != null)
                {
                    ListaDeProductos.Remove(Eliminar);
                    Console.WriteLine($"Producto {nombre} eliminado con exito.");
                }
                else
                {
                    Console.WriteLine($"No es posible eliminar {nombre} porque no existe");
                }
            }
        }

        //Clase cliente tiene un carrito de compras. Puede agregar, eliminar productos en cantidades especificadas
        //The Cliente/Client class has a shoping car. Can add & remove products with cantities.  
        public class Cliente
        {
            private string Nombre { get;  set; }
            private string Correo { get;  set; }

            private CarritoDeCompras Carrito = new CarritoDeCompras();

            public Cliente(string nombre, string correo)
            {
                Nombre = (!string.IsNullOrEmpty(nombre)) ? nombre : throw new ArgumentException("Nombre no puede estar vacio o nulo");
                Correo = (!string.IsNullOrEmpty(correo) && correo.Contains("@")) ? correo : throw new ArgumentException("Correo nulo o invalido");
                Console.WriteLine($"\nCliente {nombre} agregado con exito");
            }

            public void MostrarInformacion()
            {
                Console.WriteLine($"" +
                    $"\nNombre del cliente: {Nombre}" +
                    $"\nCorreo electronico: {Correo}");
                Carrito.MostrarInformacion();
            }

            public void AgregarAlCarrito(string nombre, int cantidad)
            {
                Carrito.AgregarProducto(nombre, cantidad);
            }

            public void EliminarDelCarrito(string nombre, int cantidad)
            {
                Carrito.EliminarProducto(nombre, cantidad);
            }

        }

        //El carrito de compras maneja un diccionario de productos para alojar los productos y usa el valor para
        //almanecar la cantidad de productos que el cliente agrega o elimina. A su vez actualiza el monto del carrito
        //por cada movimiento realizado.

        //The shoping car (CarritoDeCompras) class, has a dictionary that contains all the client's products and use the
        //Value to save the amount of items that a client has. Also uptade the Total price of the car with every movement.
        public class CarritoDeCompras
        {
            private Dictionary<Producto, int> Productos = new Dictionary<Producto, int>();
            private decimal Total = 0;

            public void AgregarProducto(string nombre, int cantidad)
            {
                Producto producto = ListaProductos.ListaDeProductos.Find(p =>  p.Nombre == nombre);
                if (producto == null )
                {
                    throw new ArgumentException($"El producto {nombre} no puede ser agregado porque no existe.");
                }
                if (cantidad <= 0)
                {
                    throw new ArgumentNullException("Cantidad de producto debe ser mayor a 0");
                }

                if (Productos.ContainsKey(producto))
                {
                    Productos[producto] += cantidad;
                }
                else
                {
                    Productos[producto] = cantidad;
                }
                Total += (producto.Precio * cantidad);
                Console.WriteLine($"+{cantidad} {nombre} agregado al carrito");

            }

            public void EliminarProducto(string nombre, int cantidad)
            {

                Producto producto = ListaProductos.ListaDeProductos.Find(p => p.Nombre == nombre);
                if (producto == null)
                {
                    throw new ArgumentException($"El producto {nombre} no existe.");
                }
                if (!Productos.ContainsKey(producto))
                {
                    throw new Exception($"El producto {producto.Nombre} no esta en el carrito");
                }

                if (cantidad <= 0)
                {
                    throw new ArgumentNullException("Cantidad de producto debe ser mayor a 0");
                }

                if ((Productos[producto] - cantidad) <= 0)
                {
                    throw new ArgumentException($"No tienes suficientes {nombre} en el carrito para remover");
                }

                Productos[producto] -= cantidad;
                Console.WriteLine($"Se han removido {cantidad} {nombre} del carrito");
                Total -= (producto.Precio * cantidad);


            }

            public void MostrarInformacion()
            {
                int count = 1;
                Console.WriteLine("\nProductos en el carrito:");
                foreach (var c in Productos)
                {
                    Console.WriteLine($"{count}. {c.Key.Nombre} - {c.Key.Descripcion} - {c.Key.Precio:C} - cant: {c.Value}");
                    
                }
                Console.WriteLine($"\nTotal del carrito: {Total:C}\n");
            }
        }
        //Clase producto. tiene un precio, nombre y descripcion. La clase puede crear productos y a su vez son agregados a la lista.
        //The producto class has a price, name and description. The class can create products and at the same time add them to the global list.
        public class Producto
        {
            public string Nombre { get; protected set; }
            public string Descripcion { get; protected set; }
            public decimal Precio { get; protected set; }

            public static void CrearProducto(string nombre, string descripcion, decimal precio)
            {
                if (ListaProductos.ListaDeProductos.Exists(p => p.Nombre == nombre))
                {
                    throw new Exception($"El producto {nombre} ya existe");
                }
                Producto NuevoProducto = new Producto(nombre, descripcion, precio);
                ListaProductos.ListaDeProductos.Add(NuevoProducto);
                Console.WriteLine($"Producto {nombre} creado con exito.");
            }

            private Producto(string nombre, string descripcion, decimal precio)
            {
                Nombre = (!string.IsNullOrEmpty(nombre)) ? nombre : throw new ArgumentException("Nombre de producto nulo o invalido");
                Descripcion = (!string.IsNullOrEmpty(descripcion)) ? descripcion : throw new ArgumentException("Descripcion de producto nulo o invalida");
                Precio = (!string.IsNullOrEmpty(precio.ToString()) && precio > 0) ? precio : throw new ArgumentException("Precio nulo o es menor o igual a 0");
            }

            public void MostrarInformacion()
            {
                Console.WriteLine($"" +
                    $"Nombre: {Nombre}" +
                    $"Descripcion: {Descripcion}" +
                    $"Precio: {Precio}");
            }




        }
        static void Main(string[] args)
        {
            //Creacion de productos:
            //Creating products:
            Producto.CrearProducto("Manzana", "Rojas y jugosas", 120);
            Producto.CrearProducto("Telefono", "Ultimo modelo 8GB", 800);
            Producto.CrearProducto("Zapatillas", "Hechas con cuero", 250);
            Producto.CrearProducto("Galleta", "Crujiente con crema", 10);


            //Mostrar productos creados:
            //Show created products:
            ListaProductos.MostrarTodos();

            //Elimina algun producto que no esta disponible para que el cliente no pueda agregarlo al carrito
            //Remove some product that is no aviable so that the client can't add it to the shoping car.
            ListaProductos.EliminarProducto("Galleta");

            Cliente cliente1 = new Cliente("Jose", "JoseAlfredo@gmail.com");

            //Agregando productos al carrito.
            //Adding products to the shoping car.
            cliente1.AgregarAlCarrito("Manzana", 10);
            cliente1.AgregarAlCarrito("Telefono", 2);
            cliente1.AgregarAlCarrito("Zapatillas", 1);

            cliente1.EliminarDelCarrito("Telefono", 1);


            //Muestra informacion del cliente y su carrito mas el total.
            //Show the client's information and the shoping car's total price.
            cliente1.MostrarInformacion();

        }
    }
}
