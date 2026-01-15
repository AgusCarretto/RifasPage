using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public class Rifa
    {

        public int id { get; set; }
        public decimal prize { get; set; }

        public int? CompradorId { get; set; }
        public Comprador? comprador { get; set; }

        public EstadoRifa state { get; set; }


        public Rifa() { }
        public Rifa(int prize, EstadoRifa state)
        {
            this.prize = prize;
            this.state = state;
        }

        public enum EstadoRifa
        {
            Vendido,
            Reservado,
            Disponible
        }

    }
}
