using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AdvertisementPrediction.Model
{
    public partial class AdvertisementPredictionContext : DbContext
    {
        public AdvertisementPredictionContext()
        {
        }

        public AdvertisementPredictionContext(DbContextOptions<AdvertisementPredictionContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Prediction> Predictions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;password=advertisement;database=AdvertisementPrediction;convert zero datetime=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Prediction>(entity =>
            {
                entity.HasKey(e => new { e.AdvertisementId, e.Model })
                    .HasName("PRIMARY");

                entity.ToTable("predictions");

                entity.Property(e => e.PredictionId)
                    .HasColumnName("prediction_id")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.AdvertisementId)
                    .HasColumnName("advertisement_id")
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Model)
                    .HasColumnName("model")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.GoodSentence)
                    .IsRequired()
                    .HasColumnName("good_sentence")
                    .HasMaxLength(45)
                    .IsUnicode(false);

                entity.Property(e => e.WrongSentence)
                    .IsRequired()
                    .HasColumnName("wrong_sentence")
                    .HasMaxLength(45)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
