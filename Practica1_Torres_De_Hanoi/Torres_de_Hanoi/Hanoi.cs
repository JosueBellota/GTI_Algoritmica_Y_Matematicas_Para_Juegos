using System;
using System.Collections.Generic;
using System.Linq;

namespace Torres_de_Hanoi
{

    public class Hanoi
    {

        private int movimientos;


        public Hanoi()
        {
            movimientos = 0;
        }

        public void MoverDisco(Pila origen, Pila destino)
        {
            if (!origen.IsEmpty() && (destino.IsEmpty() || origen.Top().Tamaño < destino.Top().Tamaño))
            {
                Disco disco = origen.Pop();
                destino.Push(disco);
                movimientos++;
                Console.WriteLine($"Mover disco {disco.Tamaño} de {origen.Nombre} a {destino.Nombre}");
            }
            else if (!destino.IsEmpty() && (origen.IsEmpty() || destino.Top().Tamaño < origen.Top().Tamaño))
            {
                Disco disco = destino.Pop();
                origen.Push(disco);
                movimientos++;
                Console.WriteLine($"Mover disco {disco.Tamaño} de {destino.Nombre} a {origen.Nombre}");
            }
        }

        public void ResolverIterativo(int n, Pila ini, Pila fin, Pila aux)
        {
            if (n % 2 == 1)
            {
                while (fin.Count() != n)
                {
                    MoverDisco(ini, fin);
                    MostrarEstadoPilas(ini, aux, fin);

                    if (fin.Count() == n) break;

                    MoverDisco(ini, aux);
                    MostrarEstadoPilas(ini, aux, fin);

                    MoverDisco(aux, fin);
                    MostrarEstadoPilas(ini, aux, fin);
                }
            }
            else
            {
                while (fin.Count() != n)
                {
                    MoverDisco(ini, aux);
                    MostrarEstadoPilas(ini, aux, fin);

                    if (fin.Count() == n) break;

                    MoverDisco(ini, fin);
                    MostrarEstadoPilas(ini, aux, fin);

                    MoverDisco(aux, fin);
                    MostrarEstadoPilas(ini, aux, fin);
                }
            }
        }


        public void ResolverRecursivo(int n, Pila ini, Pila fin, Pila aux)
        {
            if (n == 1)
            {
                MoverDisco(ini, fin);
                MostrarEstadoPilas(ini, aux, fin);
            }
            else
            {
                ResolverRecursivo(n - 1, ini, aux, fin);
                MoverDisco(ini, fin);
                MostrarEstadoPilas(ini, aux, fin);
                ResolverRecursivo(n - 1, aux, fin, ini);
            }
        }

        private void MostrarEstadoPilas(Pila ini, Pila aux, Pila fin)
        {
            Console.WriteLine("Estado de las pilas:");
            Console.WriteLine($"{ini.Nombre}: {MostrarPila(ini)}");
            Console.WriteLine($"{aux.Nombre}: {MostrarPila(aux)}");
            Console.WriteLine($"{fin.Nombre}: {MostrarPila(fin)}");
            Console.WriteLine();
        }


        private string MostrarPila(Pila pila)
        {
            if (pila.IsEmpty())
            {
                return "Vacía";
            }
            else
            {
                var discos = pila.GetDiscos();
                return string.Join(" , ", discos.Select(d => $"Disco {d.Tamaño}"));
            }
        }

        public int ObtenerMovimientos()
        {
            return movimientos;
        }
    }
}
