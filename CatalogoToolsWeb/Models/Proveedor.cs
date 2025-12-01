using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogoToolsWeb.Models
{
    [Table("tbl_Proveedores")]
    public class Proveedor
    {
        [Key]
        [Column("nId")]
        public int Id { get; set; }

        [Column("nNombre")]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Column("iLogo")]
        public byte[]? Logo { get; set; }

        [Column("dFechaInsercion")]
        public DateTime FechaInsercion { get; set; }

        [Column("bActivo")]
        public bool Activo { get; set; }
    }
}