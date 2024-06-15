﻿// <auto-generated />
using System;
using EquinoxResourceBrowser.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EquinoxResourceBrowser.Data.Migrations
{
    [DbContext(typeof(ResourceContext))]
    [Migration("20240613185727_AddComputedResources")]
    partial class AddComputedResources
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Alliance", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AllianceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("CreatorCharacterId")
                        .HasColumnType("int");

                    b.Property<int>("CreatorCorporationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateFounded")
                        .HasColumnType("datetime2");

                    b.Property<int?>("ExecutorCorporationId")
                        .HasColumnType("int");

                    b.Property<int?>("FactionId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ticker")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id", "AllianceId");

                    b.ToTable("Alliances");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.ComputedResource", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SolarSystemId")
                        .HasColumnType("int");

                    b.Property<int>("Filter")
                        .HasColumnType("int");

                    b.Property<int?>("AllianceId")
                        .HasColumnType("int");

                    b.Property<int?>("CorporationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("TotalPower")
                        .HasColumnType("int");

                    b.Property<int>("TotalWorkforce")
                        .HasColumnType("int");

                    b.HasKey("Id", "SolarSystemId", "Filter");

                    b.ToTable("ComputedResources");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Constellation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ConstellationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.HasKey("Id", "ConstellationId");

                    b.ToTable("Constellations");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Corporation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CorporationId")
                        .HasColumnType("int");

                    b.Property<int?>("AllianceId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateFounded")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("FactionId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Ticker")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Id", "CorporationId");

                    b.ToTable("Corporations");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Faction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("FactionId")
                        .HasColumnType("int");

                    b.Property<int?>("CorporationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsUnique")
                        .HasColumnType("bit");

                    b.Property<int?>("MilitiaCorporationId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("SizeFactor")
                        .HasColumnType("real");

                    b.Property<int?>("SolarSystemId")
                        .HasColumnType("int");

                    b.Property<int>("StationCount")
                        .HasColumnType("int");

                    b.Property<int>("StationSystemCount")
                        .HasColumnType("int");

                    b.HasKey("Id", "FactionId");

                    b.ToTable("Factions");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Planet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("PlanetId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("MagmaticRate")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Power")
                        .HasColumnType("int");

                    b.Property<int>("SolarSystemId")
                        .HasColumnType("int");

                    b.Property<int?>("SuperionicRate")
                        .HasColumnType("int");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.Property<int?>("Workforce")
                        .HasColumnType("int");

                    b.HasKey("Id", "PlanetId");

                    b.ToTable("Planets");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Region", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("RegionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id", "RegionId");

                    b.ToTable("Regions");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.SolarSystem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SolarSystemId")
                        .HasColumnType("int");

                    b.Property<int>("ConstellationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SecurityClass")
                        .IsRequired()
                        .HasMaxLength(2)
                        .HasColumnType("nvarchar(2)");

                    b.Property<float>("SecurityStatus")
                        .HasColumnType("real");

                    b.Property<int?>("StarId")
                        .HasColumnType("int");

                    b.HasKey("Id", "SolarSystemId");

                    b.ToTable("SolarSystems");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Sovereignty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SolarSystemId")
                        .HasColumnType("int");

                    b.Property<int?>("AllianceId")
                        .HasColumnType("int");

                    b.Property<int?>("CorporationId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("FactionId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.HasKey("Id", "SolarSystemId");

                    b.ToTable("Sovereignties");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Star", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("StarId")
                        .HasColumnType("int");

                    b.Property<long>("Age")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<float>("Luminosity")
                        .HasColumnType("real");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Power")
                        .HasColumnType("int");

                    b.Property<long>("Radius")
                        .HasColumnType("bigint");

                    b.Property<int>("SolarSystemId")
                        .HasColumnType("int");

                    b.Property<string>("SpectralClass")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Temperature")
                        .HasColumnType("int");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.HasKey("Id", "StarId");

                    b.ToTable("Stars");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Stargate", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("StargateId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("DestinationStargateId")
                        .HasColumnType("int");

                    b.Property<int>("DestinationSystemId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SolarSystemId")
                        .HasColumnType("int");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.Property<double>("X")
                        .HasColumnType("float");

                    b.Property<double>("Y")
                        .HasColumnType("float");

                    b.Property<double>("Z")
                        .HasColumnType("float");

                    b.HasKey("Id", "StargateId");

                    b.ToTable("Stargates");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Type", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id", "TypeId");

                    b.ToTable("Types");
                });

            modelBuilder.Entity("EquinoxResourceBrowser.Data.Models.Upgrade", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("MagmaticRate")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("Power")
                        .HasColumnType("int");

                    b.Property<int>("SuperionicRate")
                        .HasColumnType("int");

                    b.Property<int>("Workforce")
                        .HasColumnType("int");

                    b.HasKey("Id", "TypeId");

                    b.ToTable("Upgrades");
                });
#pragma warning restore 612, 618
        }
    }
}
