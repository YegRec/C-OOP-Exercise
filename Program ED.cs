using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        public interface IDispositivo
        {
            void Encender();

            void Apagar();

            void MostrarInformacion();

        }

        abstract public class DispositivoELectronico : IDispositivo
        {
            public string Marca {  get; protected set; }
            public string Modelo {  get; protected set; }
            public decimal Precio { get; protected set; }

            public virtual void ConsDispositivo(string marca, string modelo, decimal precio)
            {
                Marca = (!string.IsNullOrEmpty(marca)) ? marca : throw new ArgumentNullException("Marca nula o invalida");
                Modelo = (!string.IsNullOrEmpty(modelo)) ? modelo : throw new ArgumentNullException("Modelo nulo o invalido");
                Precio = (!string.IsNullOrEmpty(precio.ToString()) || precio < 0) ? precio : throw new ArgumentNullException("Precio nulo o invalido");
            }
            public virtual void Encender()
            {
                Console.WriteLine("Encendiendo dispositivo...");
            }

            public virtual void Apagar()
            {
                Console.WriteLine("Apagando dispositivo...");
            }

            public virtual void MostrarInformacion()
            {
                Console.WriteLine($"\nInformacion del dispositivo:\n" +
                    $"Marca: {Marca}\n" +
                    $"Modelo: {Modelo}\n" +
                    $"Precio: {Precio}");
            }
        }

        public class Computadora : DispositivoELectronico
        {
            public string TipoProcesador { get; protected set; }

            public Computadora(string marca, string modelo, decimal precio, string tipoProcesador)
            {
                base.ConsDispositivo(marca, modelo, precio);
                TipoProcesador = (!string.IsNullOrEmpty(tipoProcesador)) ? tipoProcesador : throw new ArgumentNullException("Tipo de procesador nulo o invalido");
            }

            public override void MostrarInformacion()
            {
                base.MostrarInformacion();
                Console.WriteLine($"Tipo de procesador {TipoProcesador}\n");
            }

            public override void Encender()
            {
                Console.WriteLine("Encendiendo computadora...\n");
            }

            public override void Apagar()
            {
                Console.WriteLine("Apagando computadora...\n");
            }
            public static void Crear(string marca, string modelo, decimal precio, string tipoProcesador)
            {
                Computadora NuevaComputadora = new Computadora(marca, modelo, precio, tipoProcesador);
                lista.Add(NuevaComputadora);
            }

        }

        public class Telefono : DispositivoELectronico
        {
            public string SistemaOperativo { get; protected set; }

            public Telefono(string marca, string modelo, decimal precio, string sistemaOperativo)
            {
                base.ConsDispositivo(marca, modelo, precio);
                SistemaOperativo = (!string.IsNullOrEmpty(sistemaOperativo)) ? sistemaOperativo : throw new ArgumentNullException("Sistema operativo nulo o invalido");
            }

            public override void MostrarInformacion()
            {
                base.MostrarInformacion();
                Console.WriteLine($"Sistema Operativo: {SistemaOperativo}");
            }

            public override void Encender()
            {
                Console.WriteLine("Encendiendo telefono...");
            }

            public override void Apagar()
            {
                Console.WriteLine("Apagando telefono...\n");
            }

            public static void Crear(string marca, string modelo, decimal precio, string sistemaOperativo)
            {
                Telefono NuevoTelefono = new Telefono(marca, modelo, precio, sistemaOperativo);
                lista.Add(NuevoTelefono);
            }

        }

        public class Tablet : DispositivoELectronico
        {
            public int SizePantalla { get; protected set; }

            public Tablet(string marca, string modelo, decimal precio, int sizepantalla)
            {
                base.ConsDispositivo(marca, modelo, precio);
                SizePantalla = (!string.IsNullOrEmpty(sizepantalla.ToString()) || sizepantalla < 0 || sizepantalla > 50) ? sizepantalla : throw new ArgumentNullException("Volument de pantalla nulo o invalido");
            }

            public override void Apagar()
            {
                Console.WriteLine("Apagando tablet...\n");
            }

            public override void Encender()
            {
                Console.WriteLine("Encendiendo tablet...");
            }
            public static void Crear(string marca, string modelo, decimal precio, int sizepantalla)
            {
                Tablet NuevaTablet = new Tablet(marca, modelo, precio, sizepantalla);
                lista.Add(NuevaTablet);
            }

        }
        public static List<DispositivoELectronico> lista = new List<DispositivoELectronico>();
        static void Main(string[] args)
        {

            Tablet.Crear("Apple", "iPad 5", 2500, 10);
            Tablet.Crear("Samsung", "9 Plus", 1100, 9);

            Computadora.Crear("Dell", "Optiplex", 800, "INTEL CELERON");
            Computadora.Crear("Lenovo", "ThinkPad", 430, "AMD RYZEN");

            Telefono.Crear("Xiaomi", "Mi Pro 7", 600, "Android");
            Telefono.Crear("Samsung", "S22 Ultra", 1200, "Android");
            Telefono.Crear("Apple", "Iphone X", 450, "IOS");


            foreach (DispositivoELectronico c in lista)
            {
                c.Encender();
                c.MostrarInformacion();
                c.Apagar();
            }



        }

    }
}
