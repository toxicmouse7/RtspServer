﻿// <auto-generated />
using System;
using ManagementServer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ManagementServer.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("ManagementServer.Infrastructure.Persistence.Models.RtpFuzzingPreset", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ContentHeader")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("ContentHeaderType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Payload")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("RtpFuzzingPresets");
                });

            modelBuilder.Entity("ManagementServer.Infrastructure.Persistence.Models.RtpFuzzingPreset", b =>
                {
                    b.OwnsOne("ManagementServer.Settings.AppendSettings", "AppendSettings", b1 =>
                        {
                            b1.Property<Guid>("RtpFuzzingPresetId")
                                .HasColumnType("TEXT");

                            b1.Property<bool>("UseOriginalPayload")
                                .HasColumnType("INTEGER");

                            b1.Property<bool>("UseOriginalSequence")
                                .HasColumnType("INTEGER");

                            b1.Property<bool>("UseOriginalTimestamp")
                                .HasColumnType("INTEGER");

                            b1.HasKey("RtpFuzzingPresetId");

                            b1.ToTable("RtpFuzzingPresets");

                            b1.WithOwner()
                                .HasForeignKey("RtpFuzzingPresetId");
                        });

                    b.OwnsOne("RtspServer.Domain.Models.Rtp.RtpHeader", "Header", b1 =>
                        {
                            b1.Property<Guid>("RtpFuzzingPresetId")
                                .HasColumnType("TEXT");

                            b1.PrimitiveCollection<string>("CSRC")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<uint>("CSRCCount")
                                .HasColumnType("INTEGER");

                            b1.Property<bool>("Extension")
                                .HasColumnType("INTEGER");

                            b1.Property<ushort>("HeaderExtensionLength")
                                .HasColumnType("INTEGER");

                            b1.Property<bool>("Marker")
                                .HasColumnType("INTEGER");

                            b1.Property<bool>("Padding")
                                .HasColumnType("INTEGER");

                            b1.Property<uint>("PayloadType")
                                .HasColumnType("INTEGER");

                            b1.Property<uint>("SSRCIdentifier")
                                .HasColumnType("INTEGER");

                            b1.Property<ushort>("SequenceNumber")
                                .HasColumnType("INTEGER");

                            b1.Property<int>("Timestamp")
                                .HasColumnType("INTEGER");

                            b1.HasKey("RtpFuzzingPresetId");

                            b1.ToTable("RtpFuzzingPresets");

                            b1.WithOwner()
                                .HasForeignKey("RtpFuzzingPresetId");
                        });

                    b.Navigation("AppendSettings")
                        .IsRequired();

                    b.Navigation("Header")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
