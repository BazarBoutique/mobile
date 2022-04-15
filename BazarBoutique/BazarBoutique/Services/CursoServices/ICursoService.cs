﻿

using BazarBoutique.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BazarBoutique.Services.CursoServices
{
    public interface ICursoService
    {
        Task<List<CursosModelo>> GetCurso(bool withDisabled);
    }
}