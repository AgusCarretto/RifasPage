using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio
{
    public class Rifa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int id { get; set; }
        public decimal prize { get; set; }

        public int? CompradorId { get; set; }
        public Comprador? comprador { get; set; }

        public EstadoRifa state { get; set; }

        /// <summary>Último número de la 1ra tanda: los primeros 80 números, del 4001 al 4080.</summary>
        public const int UltimoNumeroPrimeraTanda = 4080;

        /// <summary>
        /// La 1ra tanda son los primeros 80 números (4001-4080) y la 2da es el resto (hoy 19321-19400).
        /// Se deduce del número, no se guarda en la base.
        /// </summary>
        [NotMapped]
        public int Tanda => id <= UltimoNumeroPrimeraTanda ? 1 : 2;


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
