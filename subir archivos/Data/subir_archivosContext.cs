using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace subir_archivos.Models
{
    public class subir_archivosContext : DbContext
    {
        public subir_archivosContext (DbContextOptions<subir_archivosContext> options)
            : base(options)
        {
        }

        public DbSet<subir_archivos.Models.Usuario> Usuario { get; set; }
    }
}
