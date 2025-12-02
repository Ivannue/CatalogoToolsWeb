using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CatalogoToolsWeb.Models
{
    [Table("tbl_Productos")]
    public class Producto
    {
        [Key]
        [Column("nId")]
        public int Id { get; set; }

        [Column("nIdProveedor")]
        public int ProveedorId { get; set; }

        [Column("nNombre")]
        [MaxLength(150)]
        public string Nombre { get; set; } = string.Empty;

        [Column("iImage")]
        public byte[]? Image { get; set; }

        [Column("dFechaInsercion")]
        public DateTime FechaInsercion { get; set; }

        [Column("bActivo")]
        public bool Activo { get; set; }
    }
}