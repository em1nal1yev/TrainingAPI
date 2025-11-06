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
    internal class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            var fixedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Bütün Court ID-ləri (CourtConfiguration-dakı dəyərlərlə eyni olmalıdır)
            var aliMehkemeId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var apellyasiyaMehkemesiId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var inzibatiMehkemeId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var kommersiyaMehkemesiId = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var agirCinayetlerId = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var herbiMehkemeId = Guid.Parse("66666666-6666-6666-6666-666666666666");
            var rayonSeherMehkemeleriId = Guid.Parse("77777777-7777-7777-7777-777777777777");

            var departments = new List<Department>();

            // 1. Ali Məhkəmə (111...)
            departments.Add(new Department { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Ali Məhkəmə Aparatı", CourtId = aliMehkemeId, CreatedDate = fixedDate });
            departments.Add(new Department { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Analitik Şöbə", CourtId = aliMehkemeId, CreatedDate = fixedDate });

            // 2. Apellyasiya Məhkəməsi (222...)
            departments.Add(new Department { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Name = "Mülki Kollegiya Şöbəsi", CourtId = apellyasiyaMehkemesiId, CreatedDate = fixedDate });
            departments.Add(new Department { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Name = "İnzibati Heyət", CourtId = apellyasiyaMehkemesiId, CreatedDate = fixedDate });

            // 3. İnzibati Məhkəmə (333...)
            departments.Add(new Department { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), Name = "İnzibati İşlər Şöbəsi", CourtId = inzibatiMehkemeId, CreatedDate = fixedDate });
            departments.Add(new Department { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), Name = "Maliyyə və Təchizat", CourtId = inzibatiMehkemeId, CreatedDate = fixedDate });

            // 4. Kommersiya Məhkəməsi (444...)
            departments.Add(new Department { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), Name = "Kommersiya Mübahisələri Şöbəsi", CourtId = kommersiyaMehkemesiId, CreatedDate = fixedDate });
            departments.Add(new Department { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), Name = "Hüquqi Təminat", CourtId = kommersiyaMehkemesiId, CreatedDate = fixedDate });

            // 5. Ağır Cinayətlər Məhkəməsi (555...)
            departments.Add(new Department { Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), Name = "Cinayət İşlərinin Təminatı", CourtId = agirCinayetlerId, CreatedDate = fixedDate });
            departments.Add(new Department { Id = Guid.Parse("50000000-0000-0000-0000-000000000002"), Name = "Kadrlar və İşlərin İdarə Edilməsi", CourtId = agirCinayetlerId, CreatedDate = fixedDate });

            // 6. Hərbi Məhkəmə (666...)
            departments.Add(new Department { Id = Guid.Parse("60000000-0000-0000-0000-000000000001"), Name = "Hərbi Kollegiya Şöbəsi", CourtId = herbiMehkemeId, CreatedDate = fixedDate });
            departments.Add(new Department { Id = Guid.Parse("60000000-0000-0000-0000-000000000002"), Name = "Hərbi Sənədləşdirmə", CourtId = herbiMehkemeId, CreatedDate = fixedDate });

            // 7. Rayon (Şəhər) Məhkəmələri (777...)
            departments.Add(new Department { Id = Guid.Parse("70000000-0000-0000-0000-000000000001"), Name = "Məhkəmə Katibliyi", CourtId = rayonSeherMehkemeleriId, CreatedDate = fixedDate });
            departments.Add(new Department { Id = Guid.Parse("70000000-0000-0000-0000-000000000002"), Name = "Ərazi İşləri Şöbəsi", CourtId = rayonSeherMehkemeleriId, CreatedDate = fixedDate });


            // Department HasData-nı burada tətbiq edirik
            builder.HasData(departments);
        }
    }
}
