﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UEFA.ChampionsLeague.Data;

namespace UEFA.ChampionsLeague.Data.Migrations
{
    [DbContext(typeof(UEFAChampionsLeagueDbContext))]
    partial class UEFAChampionsLeagueDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("UEFA.ChampionsLeague.Data.Models.Match", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AwayTeam")
                        .HasColumnType("nvarchar(250)");

                    b.Property<byte>("AwayTeamScore");

                    b.Property<string>("Group")
                        .HasColumnType("nvarchar(1)");

                    b.Property<string>("HomeTeam")
                        .HasColumnType("nvarchar(250)");

                    b.Property<byte>("HomeTeamScore");

                    b.Property<DateTime>("KickoffAt");

                    b.Property<string>("LeagueTitle")
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("MatchDay");

                    b.HasKey("Id");

                    b.ToTable("Matches");
                });
#pragma warning restore 612, 618
        }
    }
}