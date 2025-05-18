using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Lab2_DB.Models;

public partial class LibraryContext : DbContext
{
    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Fund> Funds { get; set; }

    public virtual DbSet<IssuedBook> IssuedBooks { get; set; }

    public virtual DbSet<Librarian> Librarians { get; set; }

    public virtual DbSet<Library> Libraries { get; set; }

    public virtual DbSet<PublishingHouse> PublishingHouses { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<Request> Requests { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-MARYNA;Database=LibraryManagement;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.EmailAuthor).HasMaxLength(100);
            entity.Property(e => e.FullNameAuthor).HasMaxLength(100);
            entity.Property(e => e.PhoneNumberAuthor).HasMaxLength(20);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasIndex(e => e.AuthorBook, "IX_Books_AuthorBook");
            entity.HasIndex(e => e.FundBook, "IX_Books_FundBook");
            entity.HasIndex(e => e.PublisherBook, "IX_Books_PublisherBook");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.AvailabilityStatusBook).HasMaxLength(20);
            entity.Property(e => e.GenreBook).HasMaxLength(50);
            entity.Property(e => e.Isbn).HasColumnName("ISBN");
            entity.Property(e => e.TitleBook).HasMaxLength(150);

            entity.HasOne(d => d.AuthorBookNavigation).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorBook)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Authors");

            entity.HasOne(d => d.FundBookNavigation).WithMany(p => p.Books)
                .HasForeignKey(d => d.FundBook)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_Funds");

            entity.HasOne(d => d.PublisherBookNavigation).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherBook)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_PublishingHouses");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasIndex(e => e.Library, "IX_Departments_Library");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.LeaderDepartment).HasMaxLength(100);
            entity.Property(e => e.TitleDepartment).HasMaxLength(100);

            entity.HasOne(d => d.LibraryNavigation).WithMany(p => p.Departments)
                .HasForeignKey(d => d.Library)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Departments_Libraries");
        });

        modelBuilder.Entity<Fund>(entity =>
        {
            entity.HasIndex(e => e.DepartmentFund, "IX_Funds_DepartmentFund");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.TitleFund).HasMaxLength(100);
            entity.Property(e => e.TypeFund).HasMaxLength(50);

            entity.HasOne(d => d.DepartmentFundNavigation).WithMany(p => p.Funds)
                .HasForeignKey(d => d.DepartmentFund)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Funds_Departments");
        });

        modelBuilder.Entity<IssuedBook>(entity =>
        {
            entity.HasIndex(e => e.BookId, "IX_IssuedBooks_BookID");
            entity.HasIndex(e => e.RequestId, "IX_IssuedBooks_RequestID");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.RequestId).HasColumnName("RequestID");

            entity.HasOne(d => d.Book).WithMany(p => p.IssuedBooks)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IssuedBooks_Books1");

            entity.HasOne(d => d.Request).WithMany(p => p.IssuedBooks)
                .HasForeignKey(d => d.RequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IssuedBooks_Requests");
        });

        modelBuilder.Entity<Librarian>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.EmailLibrarian).HasMaxLength(100);
            entity.Property(e => e.FullNameLibrarian).HasMaxLength(100);
            entity.Property(e => e.PhoneNumberLibrarian).HasMaxLength(50);
        });

        modelBuilder.Entity<Library>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.EmailLibrary).HasMaxLength(100);
            entity.Property(e => e.LeaderLibrary).HasMaxLength(100);
            entity.Property(e => e.LibraryAddress).HasMaxLength(200);
            entity.Property(e => e.PhoneNumberLibrary).HasMaxLength(50);
            entity.Property(e => e.TitleLibrary).HasMaxLength(150);
        });

        modelBuilder.Entity<PublishingHouse>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.AddressPh).HasMaxLength(150).HasColumnName("AddressPH");
            entity.Property(e => e.EmailPh).HasMaxLength(100).HasColumnName("EmailPH");
            entity.Property(e => e.PhoneNumberPh).HasMaxLength(50).HasColumnName("PhoneNumberPH");
            entity.Property(e => e.TitlePh).HasMaxLength(100).HasColumnName("TitlePH");
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.EmailReader).HasMaxLength(100);
            entity.Property(e => e.FullNameReader).HasMaxLength(100);
            entity.Property(e => e.PhoneNumberReader).HasMaxLength(50);
            entity.Property(e => e.PlaceStudyOrWorkReader).HasMaxLength(150);
            entity.Property(e => e.RoleReader).HasMaxLength(50);
        });

        modelBuilder.Entity<Request>(entity =>
        {
            entity.HasIndex(e => e.CardNumberReader, "IX_Requests_CardNumberReader");
            entity.HasIndex(e => e.PassNumberLibrarian, "IX_Requests_PassNumberLibrarian");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.Isbn).HasColumnName("ISBN");
            entity.Property(e => e.RequestStatus).HasMaxLength(20);
            entity.Property(e => e.RequestType).HasMaxLength(20);

            entity.HasOne(d => d.CardNumberReaderNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.CardNumberReader)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Requests_Readers");

            entity.HasOne(d => d.PassNumberLibrarianNavigation).WithMany(p => p.Requests)
                .HasForeignKey(d => d.PassNumberLibrarian)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Requests_Librarians");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
