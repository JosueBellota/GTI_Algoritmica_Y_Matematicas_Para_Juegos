using System;
using System.Linq;

namespace Torres_de_Hanoi
{

    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("El Gran Juego de las Torres de Hanoi\n");
            Console.WriteLine("3 torres\n");
            Console.WriteLine("Indica el número de discos...\n");

            int n;
            while (!int.TryParse(Console.ReadLine(), out n) || n <= 0)
            {
                Console.WriteLine("Por favor, introduce un número válido de discos (mayor a 0):");
            }

            Console.WriteLine($"\nHas seleccionado {n} discos\n");
            Console.WriteLine("Indica I para Iterativo o R para Recursivo\n");

            char metodo;
            while (!char.TryParse(Console.ReadLine().ToUpper(), out metodo) || (metodo != 'I' && metodo != 'R'))
            {
                Console.WriteLine("Por favor, introduce 'I' para Iterativo o 'R' para Recursivo:");
            }

            Pila ini = new Pila("INI");
            Pila aux = new Pila("AUX");
            Pila fin = new Pila("FIN");

            for (int i = n; i >= 1; i--)
            {
                ini.Push(new Disco(i));
            }

            Hanoi hanoi = new Hanoi();

            Console.WriteLine("\nSituación Inicial");
            MostrarEstado(ini, aux, fin);

            if (metodo == 'I')
            {
                hanoi.ResolverIterativo(n, ini, fin, aux);
            }
            else
            {
                hanoi.ResolverRecursivo(n, ini, fin, aux);
            }

            Console.WriteLine($"\nResuelto en {hanoi.ObtenerMovimientos()} movimientos\n");
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        static void MostrarEstado(Pila ini, Pila aux, Pila fin)
        {
            Console.WriteLine($"Torre {ini.Nombre}: {MostrarPila(ini)}");
            Console.WriteLine($"Torre {aux.Nombre}: {MostrarPila(aux)}");
            Console.WriteLine($"Torre {fin.Nombre}: {MostrarPila(fin)}\n");
        }

        static string MostrarPila(Pila pila)
        {
            if (pila.IsEmpty())
            {
                return "Vacía";
            }
            else
            {
                var discos = pila.GetDiscos();
                return string.Join(", ", discos.Select(d => $"{d.Tamaño}"));
            }
        }
    }
}
