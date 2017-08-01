using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LPCloudAPI.Models
{
    public class TextItemContext : DbContext
    {
        public TextItemContext(DbContextOptions<TextItemContext> options) : base(options)
        {

        }

        public DbSet<TextItem> TextItems { get; set; }
    }
}
