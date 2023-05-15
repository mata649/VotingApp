﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VotingApp.Context;

#nullable disable

namespace VotingApp.Migrations
{
    [DbContext(typeof(VotingAppContext))]
    partial class VotingAppContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("VotingApp.Option.Domain.Option", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("PoolID")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.HasKey("ID");

                    b.HasIndex("PoolID");

                    b.ToTable("Option", (string)null);
                });

            modelBuilder.Entity("VotingApp.Pool.Domain.Pool", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uuid");

                    b.HasKey("ID");

                    b.HasIndex("UserID");

                    b.ToTable("Pool", (string)null);
                });

            modelBuilder.Entity("VotingApp.User.Domain.User", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("ID");

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("VotingApp.Vote.Domain.Vote", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("OptionID")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserID")
                        .HasColumnType("uuid");

                    b.HasKey("ID");

                    b.HasIndex("OptionID");

                    b.HasIndex("UserID");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("VotingApp.Option.Domain.Option", b =>
                {
                    b.HasOne("VotingApp.Pool.Domain.Pool", "Pool")
                        .WithMany("Options")
                        .HasForeignKey("PoolID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pool");
                });

            modelBuilder.Entity("VotingApp.Pool.Domain.Pool", b =>
                {
                    b.HasOne("VotingApp.User.Domain.User", "User")
                        .WithMany("Pools")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("VotingApp.Vote.Domain.Vote", b =>
                {
                    b.HasOne("VotingApp.Option.Domain.Option", "Option")
                        .WithMany("Votes")
                        .HasForeignKey("OptionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("VotingApp.User.Domain.User", "User")
                        .WithMany("Votes")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Option");

                    b.Navigation("User");
                });

            modelBuilder.Entity("VotingApp.Option.Domain.Option", b =>
                {
                    b.Navigation("Votes");
                });

            modelBuilder.Entity("VotingApp.Pool.Domain.Pool", b =>
                {
                    b.Navigation("Options");
                });

            modelBuilder.Entity("VotingApp.User.Domain.User", b =>
                {
                    b.Navigation("Pools");

                    b.Navigation("Votes");
                });
#pragma warning restore 612, 618
        }
    }
}
