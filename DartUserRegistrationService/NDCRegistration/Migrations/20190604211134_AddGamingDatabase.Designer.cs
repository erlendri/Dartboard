﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NDCRegistration;

namespace NDCRegistration.Migrations
{
    [DbContext(typeof(GamerContext))]
    [Migration("20190604211134_AddGamingDatabase")]
    partial class AddGamingDatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Dart.Messaging.Models.Game", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("GamerId");

                    b.Property<int>("Score");

                    b.Property<int>("State");

                    b.HasKey("Id");

                    b.HasIndex("GamerId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Dart.Messaging.Models.Gamer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DisplayName")
                        .IsRequired();

                    b.Property<string>("Email");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("QrCode");

                    b.HasKey("Id");

                    b.ToTable("Gamers");
                });

            modelBuilder.Entity("Dart.Messaging.Models.Game", b =>
                {
                    b.HasOne("Dart.Messaging.Models.Gamer")
                        .WithMany("Games")
                        .HasForeignKey("GamerId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
