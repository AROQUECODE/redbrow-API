using System;
using System.Collections.Generic;

namespace aroque.code.API.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; }

    public string Correo { get; set; }

    public int Edad { get; set; }

    public string Clave { get; set; }

    public string CreadoPor { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public virtual ICollection<TokenHistory> TokenHistories { get; set; } = new List<TokenHistory>();
}
