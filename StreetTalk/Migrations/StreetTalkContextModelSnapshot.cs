﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StreetTalk;

namespace StreetTalk.Migrations
{
    [DbContext(typeof(StreetTalkContext))]
    partial class StreetTalkContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("StreetTalk.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AuthorId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasMaxLength(600)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("PostId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PostId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("StreetTalk.Models.Like", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PostId")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserId", "PostId");

                    b.HasIndex("PostId");

                    b.ToTable("Like");
                });

            modelBuilder.Entity("StreetTalk.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Filename")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Sensitive")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Photo");
                });

            modelBuilder.Entity("StreetTalk.Models.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<int?>("PhotoId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<int?>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PhotoId")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Post");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Post");
                });

            modelBuilder.Entity("StreetTalk.Models.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("City")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DateOfBirth")
                        .HasColumnType("Date");

                    b.Property<string>("FirstName")
                        .HasMaxLength(45)
                        .HasColumnType("TEXT");

                    b.Property<int?>("HouseNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("HouseNumberAddition")
                        .HasMaxLength(5)
                        .HasColumnType("TEXT");

                    b.Property<string>("LastName")
                        .HasMaxLength(45)
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Street")
                        .HasMaxLength(64)
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Profile");
                });

            modelBuilder.Entity("StreetTalk.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LockoutEndTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("StreetTalk.Models.PublicPost", b =>
                {
                    b.HasBaseType("StreetTalk.Models.Post");

                    b.Property<bool>("Closed")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ReportCount")
                        .HasColumnType("INTEGER");

                    b.HasDiscriminator().HasValue("PublicPost");
                });

            modelBuilder.Entity("StreetTalk.Models.Comment", b =>
                {
                    b.HasOne("StreetTalk.Models.User", "Author")
                        .WithMany("Comments")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StreetTalk.Models.PublicPost", "Post")
                        .WithMany("Comments")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Post");
                });

            modelBuilder.Entity("StreetTalk.Models.Like", b =>
                {
                    b.HasOne("StreetTalk.Models.PublicPost", "Post")
                        .WithMany("Likes")
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StreetTalk.Models.User", "User")
                        .WithMany("Likes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Post");

                    b.Navigation("User");
                });

            modelBuilder.Entity("StreetTalk.Models.Post", b =>
                {
                    b.HasOne("StreetTalk.Models.Photo", "Photo")
                        .WithOne("Post")
                        .HasForeignKey("StreetTalk.Models.Post", "PhotoId");

                    b.HasOne("StreetTalk.Models.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId");

                    b.Navigation("Photo");

                    b.Navigation("User");
                });

            modelBuilder.Entity("StreetTalk.Models.Profile", b =>
                {
                    b.HasOne("StreetTalk.Models.User", "User")
                        .WithOne("Profile")
                        .HasForeignKey("StreetTalk.Models.Profile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("StreetTalk.Models.Photo", b =>
                {
                    b.Navigation("Post");
                });

            modelBuilder.Entity("StreetTalk.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");

                    b.Navigation("Posts");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("StreetTalk.Models.PublicPost", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Likes");
                });
#pragma warning restore 612, 618
        }
    }
}
