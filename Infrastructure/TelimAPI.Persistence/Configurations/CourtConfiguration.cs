using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Persistence.Configurations
{
    internal class CourtConfiguration : IEntityTypeConfiguration<Court>
    {
        
            public void Configure(EntityTypeBuilder<Court> builder)
        {
            
            var fixedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            
            var aliMehkemeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var apellyasiyaMehkemesiId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var inzibatiMehkemeId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var kommersiyaMehkemesiId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var agirCinayetlerId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var herbiMehkemeId = Guid.Parse("66666666-6666-6666-6666-666666666666");
            var rayonSeherMehkemeleriId = Guid.Parse("77777777-7777-7777-7777-777777777777");

            
            builder.HasData(
                new Court { Id = aliMehkemeId, Name = "Ali Məhkəmə", CreatedDate = fixedDate },
                new Court { Id = apellyasiyaMehkemesiId, Name = "Apellyasiya Məhkəməsi", CreatedDate = fixedDate },
                new Court { Id = inzibatiMehkemeId, Name = "İnzibati Məhkəmə", CreatedDate = fixedDate },
                new Court { Id = kommersiyaMehkemesiId, Name = "Kommersiya Məhkəməsi", CreatedDate = fixedDate },
                new Court { Id = agirCinayetlerId, Name = "Ağır Cinayətlər Məhkəməsi", CreatedDate = fixedDate },
                new Court { Id = herbiMehkemeId, Name = "Hərbi Məhkəmə", CreatedDate = fixedDate },
                new Court { Id = rayonSeherMehkemeleriId, Name = "Rayon (Şəhər) Məhkəmələri", CreatedDate = fixedDate }
            );
        }
    }
    
}
