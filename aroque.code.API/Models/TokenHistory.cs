using System;
using System.Collections.Generic;

namespace aroque.code.API.Models;

public partial class TokenHistory
{
    public int IdToken { get; set; }

    public int? IdUsuario { get; set; }

    public string Token { get; set; }

    public string RefreshToken { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? FechaExpiracion { get; set; }

    public bool? Activo { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; }
}
