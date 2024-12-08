﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace functions.Migrations
{
    [DbContext(typeof(WydDbContext))]
    partial class WydDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Model.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Mail")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Mail")
                        .IsUnique();

                    b.HasIndex("Uid")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Model.Community", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Communities");
                });

            modelBuilder.Entity("Model.Event", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("EndTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<int?>("GroupId")
                        .HasColumnType("int");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.Property<DateTimeOffset>("StartTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.HasIndex("ParentId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Model.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CommunityId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("GeneralForCommunity")
                        .HasColumnType("bit");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CommunityId");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("Model.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Profiles");
                });

            modelBuilder.Entity("Model.ProfileEvent", b =>
                {
                    b.Property<int>("EventId")
                        .HasColumnType("int");

                    b.Property<int>("ProfileId")
                        .HasColumnType("int");

                    b.Property<bool>("Confirmed")
                        .HasColumnType("bit");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<bool>("Trusted")
                        .HasColumnType("bit");

                    b.HasKey("EventId", "ProfileId");

                    b.HasIndex("ProfileId");

                    b.ToTable("Profile_Event");
                });

            modelBuilder.Entity("Model.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MainMail")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("MainProfileId")
                        .HasColumnType("int");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Hash")
                        .IsUnique();

                    b.HasIndex("MainMail")
                        .IsUnique();

                    b.HasIndex("MainProfileId");

                    b.HasIndex("Tag")
                        .IsUnique()
                        .HasFilter("Tag <> ''");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Model.UserCommunity", b =>
                {
                    b.Property<int>("CommunityId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("CommunityId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("User_Community");
                });

            modelBuilder.Entity("Model.UserGroup", b =>
                {
                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<long>("Color")
                        .HasColumnType("bigint");

                    b.Property<bool>("Trusted")
                        .HasColumnType("bit");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("User_Group");
                });

            modelBuilder.Entity("Model.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndsAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("ProfileId")
                        .HasColumnType("int");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.Property<DateTime?>("StartsAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProfileId");

                    b.HasIndex("UserId");

                    b.ToTable("User_Role");
                });

            modelBuilder.Entity("Model.Account", b =>
                {
                    b.HasOne("Model.User", "User")
                        .WithMany("Accounts")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Model.Event", b =>
                {
                    b.HasOne("Model.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId");

                    b.HasOne("Model.Event", "Parent")
                        .WithMany()
                        .HasForeignKey("ParentId");

                    b.Navigation("Group");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("Model.Group", b =>
                {
                    b.HasOne("Model.Community", "Community")
                        .WithMany("Groups")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Community");
                });

            modelBuilder.Entity("Model.ProfileEvent", b =>
                {
                    b.HasOne("Model.Event", "Event")
                        .WithMany("ProfileEvents")
                        .HasForeignKey("EventId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.Profile", "Profile")
                        .WithMany("ProfileEvents")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Event");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("Model.User", b =>
                {
                    b.HasOne("Model.Profile", "MainProfile")
                        .WithMany()
                        .HasForeignKey("MainProfileId");

                    b.Navigation("MainProfile");
                });

            modelBuilder.Entity("Model.UserCommunity", b =>
                {
                    b.HasOne("Model.Community", "Community")
                        .WithMany("UserCommunities")
                        .HasForeignKey("CommunityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.User", "User")
                        .WithMany("UserCommunities")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Community");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Model.UserGroup", b =>
                {
                    b.HasOne("Model.Group", "Group")
                        .WithMany("UserGroups")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.User", "User")
                        .WithMany("UserGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Model.UserRole", b =>
                {
                    b.HasOne("Model.Profile", "Profile")
                        .WithMany("UserRoles")
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Model.User", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Profile");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Model.Community", b =>
                {
                    b.Navigation("Groups");

                    b.Navigation("UserCommunities");
                });

            modelBuilder.Entity("Model.Event", b =>
                {
                    b.Navigation("ProfileEvents");
                });

            modelBuilder.Entity("Model.Group", b =>
                {
                    b.Navigation("UserGroups");
                });

            modelBuilder.Entity("Model.Profile", b =>
                {
                    b.Navigation("ProfileEvents");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("Model.User", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("UserCommunities");

                    b.Navigation("UserGroups");

                    b.Navigation("UserRoles");
                });
#pragma warning restore 612, 618
        }
    }
}
