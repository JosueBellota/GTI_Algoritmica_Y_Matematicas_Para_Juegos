using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torres_de_Hanoi
{

    public class Disco
    {

        public int Tamaño { get; set; }

        public Disco(int tamaño)
        {
            Tamaño = tamaño;
        }

        public override string ToString()
        {
            return $"Disco {Tamaño}";
        }
    }
}
