using System;
using System.Collections.Generic;
using System.Linq;

namespace Torres_de_Hanoi
{

    public class Pila
    {

        private Stack<Disco> discos;

        public string Nombre { get; set; }

        public Pila(string nombre)
        {
            discos = new Stack<Disco>();
            Nombre = nombre;
        }

        public void Push(Disco disco)
        {
            discos.Push(disco);
        }

        public Disco Pop()
        {
            return discos.Pop();
        }

        public bool IsEmpty()
        {
            return discos.Count == 0;
        }

        public Disco Top()
        {
            return discos.Peek();
        }

        public int Count()
        {
            return discos.Count;
        }


        public IEnumerable<Disco> GetDiscos()
        {
            return discos.Reverse();
        }


        public override string ToString()
        {
            return Nombre;
        }
    }
}
