using AspNetCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCore.Infrastructure.EntityConfigurations
{
    public class ProductEntityTypeConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");

            builder.HasKey(x => x.Id).HasName("pk_products");

            builder.Property(x => x.Id).HasColumnName("id").HasField("_id").UsePropertyAccessMode(PropertyAccessMode.Field).ValueGeneratedOnAdd();

            builder.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(256);
            builder.Property(x => x.Description).HasColumnName("description");
            builder.Property(x => x.ImgUri).HasColumnName("img_uri").IsRequired().HasMaxLength(1024);
            builder.Property(x => x.Price).HasColumnName("price").IsRequired();
        }
    }
}
