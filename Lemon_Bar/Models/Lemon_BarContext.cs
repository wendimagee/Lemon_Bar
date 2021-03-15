using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Lemon_Bar.Models
{
    public partial class Lemon_BarContext : DbContext
    {
        public Lemon_BarContext()
        {
        }

        public Lemon_BarContext(DbContextOptions<Lemon_BarContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<AgentInstance> AgentInstances { get; set; }
        public virtual DbSet<AgentVersion> AgentVersions { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }
        public virtual DbSet<EnumType> EnumTypes { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<MessageQueue> MessageQueues { get; set; }
        public virtual DbSet<MetaInformation> MetaInformations { get; set; }
        public virtual DbSet<MetaInformation1> MetaInformations1 { get; set; }
        public virtual DbSet<Scaleunitlimit> Scaleunitlimits { get; set; }
        public virtual DbSet<Schedule> Schedules { get; set; }
        public virtual DbSet<ScheduleTask> ScheduleTasks { get; set; }
        public virtual DbSet<ScheduleTask1> ScheduleTasks1 { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<SyncObjectDatum> SyncObjectData { get; set; }
        public virtual DbSet<Syncgroup> Syncgroups { get; set; }
        public virtual DbSet<Syncgroupmember> Syncgroupmembers { get; set; }
        public virtual DbSet<Task> Tasks { get; set; }
        public virtual DbSet<Taskdependency> Taskdependencies { get; set; }
        public virtual DbSet<Uihistory> Uihistories { get; set; }
        public virtual DbSet<Userdatabase> Userdatabases { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=tcp:wendiserver.database.windows.net,1433;Initial Catalog = Lemon_Bar; Persist Security Info=False;User ID = wendiserveradmin; Password= Admin123; MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Action>(entity =>
            {
                entity.ToTable("action", "dss");

                entity.HasIndex(e => new { e.State, e.Lastupdatetime }, "index_action_state_lastupdatetime");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Creationtime)
                    .HasColumnType("datetime")
                    .HasColumnName("creationtime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Lastupdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdatetime");

                entity.Property(e => e.State).HasColumnName("state");

                entity.Property(e => e.Syncgroupid).HasColumnName("syncgroupid");

                entity.Property(e => e.Type).HasColumnName("type");
            });

            modelBuilder.Entity<Agent>(entity =>
            {
                entity.ToTable("agent", "dss");

                entity.HasIndex(e => new { e.Subscriptionid, e.Name }, "IX_Agent_SubId_Name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.IsOnPremise).HasColumnName("is_on_premise");

                entity.Property(e => e.Lastalivetime)
                    .HasColumnType("datetime")
                    .HasColumnName("lastalivetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(140)
                    .HasColumnName("name");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(256)
                    .HasColumnName("password_hash");

                entity.Property(e => e.PasswordSalt)
                    .HasMaxLength(256)
                    .HasColumnName("password_salt");

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Subscriptionid).HasColumnName("subscriptionid");

                entity.Property(e => e.Version)
                    .HasMaxLength(40)
                    .HasColumnName("version");

                entity.HasOne(d => d.Subscription)
                    .WithMany(p => p.Agents)
                    .HasForeignKey(d => d.Subscriptionid)
                    .HasConstraintName("FK__agent__subscript");
            });

            modelBuilder.Entity<AgentInstance>(entity =>
            {
                entity.ToTable("agent_instance", "dss");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Agentid).HasColumnName("agentid");

                entity.Property(e => e.Lastalivetime)
                    .HasColumnType("datetime")
                    .HasColumnName("lastalivetime");

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasMaxLength(40)
                    .HasColumnName("version");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.AgentInstances)
                    .HasForeignKey(d => d.Agentid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__agent_ins__agent");
            });

            modelBuilder.Entity<AgentVersion>(entity =>
            {
                entity.ToTable("agent_version", "dss");

                entity.HasIndex(e => e.Version, "UQ__agent_ve__0F540134391AEE8D")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Comment).HasMaxLength(200);

                entity.Property(e => e.ExpiresOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("('9999-12-31 23:59:59.997')");

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.ProviderKey).HasMaxLength(128);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.Property(e => e.LoginProvider).HasMaxLength(128);

                entity.Property(e => e.Name).HasMaxLength(128);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.ToTable("configuration", "dss");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ConfigKey)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ConfigValue)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<EnumType>(entity =>
            {
                entity.ToTable("EnumType", "dss");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.ToTable("Item");

                entity.HasIndex(e => e.User, "UQ__Item__BD20C6F1341F0DAB")
                    .IsUnique();

                entity.Property(e => e.ItemName).HasMaxLength(30);

                entity.Property(e => e.TotalCost).HasColumnType("money");

                entity.Property(e => e.UnitCost).HasColumnType("money");

                entity.Property(e => e.Units).HasMaxLength(25);

                entity.HasOne(d => d.UserNavigation)
                    .WithOne(p => p.Item)
                    .HasForeignKey<Item>(d => d.User)
                    .HasConstraintName("FK__Item__User__7DCDAAA2");
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "TaskHosting");

                entity.HasIndex(e => e.IsCancelled, "index_job_iscancelled");

                entity.Property(e => e.JobId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.InitialInsertTimeUtc)
                    .HasColumnType("datetime")
                    .HasColumnName("InitialInsertTimeUTC")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<MessageQueue>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK__MessageQ__C87C0C9CEA1F910C");

                entity.ToTable("MessageQueue", "TaskHosting");

                entity.HasIndex(e => new { e.QueueId, e.UpdateTimeUtc, e.InsertTimeUtc, e.ExecTimes, e.Version }, "index_messagequeue_getnextmessage");

                entity.HasIndex(e => new { e.QueueId, e.MessageType, e.UpdateTimeUtc, e.InsertTimeUtc, e.ExecTimes, e.Version }, "index_messagequeue_getnextmessagebytype");

                entity.HasIndex(e => e.JobId, "index_messagequeue_jobid");

                entity.Property(e => e.MessageId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.InitialInsertTimeUtc)
                    .HasColumnType("datetime")
                    .HasColumnName("InitialInsertTimeUTC")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.InsertTimeUtc)
                    .HasColumnType("datetime")
                    .HasColumnName("InsertTimeUTC");

                entity.Property(e => e.UpdateTimeUtc)
                    .HasColumnType("datetime")
                    .HasColumnName("UpdateTimeUTC");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.MessageQueues)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK__MessageQu__JobId__2AD55B43");
            });

            modelBuilder.Entity<MetaInformation>(entity =>
            {
                entity.ToTable("MetaInformation", "dss");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.VersionString)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('1.0.0.0')");
            });

            modelBuilder.Entity<MetaInformation1>(entity =>
            {
                entity.ToTable("MetaInformation", "TaskHosting");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Timestamp)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.VersionString)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('1.0.0.0')");
            });

            modelBuilder.Entity<Scaleunitlimit>(entity =>
            {
                entity.ToTable("scaleunitlimits", "dss");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.LastModified)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.ToTable("Schedule", "TaskHosting");
            });

            modelBuilder.Entity<ScheduleTask>(entity =>
            {
                entity.HasKey(e => e.SyncGroupId);

                entity.ToTable("ScheduleTask", "dss");

                entity.Property(e => e.SyncGroupId).ValueGeneratedNever();

                entity.Property(e => e.ExpirationTime).HasColumnType("datetime");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

                entity.Property(e => e.LastUpdate).HasColumnType("datetime");

                entity.HasOne(d => d.SyncGroup)
                    .WithOne(p => p.ScheduleTask)
                    .HasForeignKey<ScheduleTask>(d => d.SyncGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ScheduleT__SyncG");
            });

            modelBuilder.Entity<ScheduleTask1>(entity =>
            {
                entity.HasKey(e => e.ScheduleTaskId)
                    .HasName("PK__Schedule__8DAD173A42A47FDE");

                entity.ToTable("ScheduleTask", "TaskHosting");

                entity.HasIndex(e => e.MessageId, "ScheduleTask_MessageId_Index");

                entity.Property(e => e.ScheduleTaskId).ValueGeneratedNever();

                entity.Property(e => e.NextRunTime).HasColumnType("datetime");

                entity.Property(e => e.TaskName)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.HasOne(d => d.ScheduleNavigation)
                    .WithMany(p => p.ScheduleTask1s)
                    .HasForeignKey(d => d.Schedule)
                    .HasConstraintName("FK__ScheduleT__Sched__3552E9B6");
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.ToTable("subscription", "dss");

                entity.HasIndex(e => e.Syncserveruniquename, "IX_SyncServerUniqueName")
                    .IsUnique()
                    .HasFilter("([syncserveruniquename] IS NOT NULL)");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Creationtime)
                    .HasColumnType("datetime")
                    .HasColumnName("creationtime");

                entity.Property(e => e.EnableDetailedProviderTracing).HasDefaultValueSql("((0))");

                entity.Property(e => e.Lastlogintime)
                    .HasColumnType("datetime")
                    .HasColumnName("lastlogintime");

                entity.Property(e => e.Name)
                    .HasMaxLength(140)
                    .HasColumnName("name");

                entity.Property(e => e.Policyid)
                    .HasColumnName("policyid")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Subscriptionstate).HasColumnName("subscriptionstate");

                entity.Property(e => e.Syncserveruniquename)
                    .HasMaxLength(256)
                    .HasColumnName("syncserveruniquename");

                entity.Property(e => e.Tombstoneretentionperiodindays).HasColumnName("tombstoneretentionperiodindays");

                entity.Property(e => e.Version)
                    .HasMaxLength(40)
                    .HasColumnName("version");
            });

            modelBuilder.Entity<SyncObjectDatum>(entity =>
            {
                entity.HasKey(e => new { e.ObjectId, e.DataType })
                    .HasName("PK_SyncObjectExtInfo");

                entity.ToTable("SyncObjectData", "dss");

                entity.Property(e => e.CreatedTime).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.LastModified)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.Property(e => e.ObjectData).IsRequired();
            });

            modelBuilder.Entity<Syncgroup>(entity =>
            {
                entity.ToTable("syncgroup", "dss");

                entity.HasIndex(e => e.HubMemberid, "index_syncgroup_hub_memberid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.ConflictResolutionPolicy).HasColumnName("conflict_resolution_policy");

                entity.Property(e => e.ConflictTableRetentionInDays).HasDefaultValueSql("((30))");

                entity.Property(e => e.HubMemberid).HasColumnName("hub_memberid");

                entity.Property(e => e.Hubhasdata).HasColumnName("hubhasdata");

                entity.Property(e => e.Lastupdatetime)
                    .HasColumnType("datetime")
                    .HasColumnName("lastupdatetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(140)
                    .HasColumnName("name");

                entity.Property(e => e.Ocsschemadefinition).HasColumnName("ocsschemadefinition");

                entity.Property(e => e.SchemaDescription)
                    .HasColumnType("xml")
                    .HasColumnName("schema_description");

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Subscriptionid).HasColumnName("subscriptionid");

                entity.Property(e => e.SyncEnabled)
                    .IsRequired()
                    .HasColumnName("sync_enabled")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.SyncInterval).HasColumnName("sync_interval");

                entity.HasOne(d => d.HubMember)
                    .WithMany(p => p.Syncgroups)
                    .HasForeignKey(d => d.HubMemberid)
                    .HasConstraintName("FK__syncgroup__hub_m");

                entity.HasOne(d => d.Subscription)
                    .WithMany(p => p.Syncgroups)
                    .HasForeignKey(d => d.Subscriptionid)
                    .HasConstraintName("FK__syncgroup__subsc");
            });

            modelBuilder.Entity<Syncgroupmember>(entity =>
            {
                entity.ToTable("syncgroupmember", "dss");

                entity.HasIndex(e => new { e.Syncgroupid, e.Databaseid }, "IX_SyncGroupMember_SyncGroupId_DatabaseId")
                    .IsUnique();

                entity.HasIndex(e => e.Databaseid, "index_syncgroupmember_databaseid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Databaseid).HasColumnName("databaseid");

                entity.Property(e => e.HubJobId).HasColumnName("hubJobId");

                entity.Property(e => e.Hubstate).HasColumnName("hubstate");

                entity.Property(e => e.HubstateLastupdated)
                    .HasColumnType("datetime")
                    .HasColumnName("hubstate_lastupdated")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.JobId).HasColumnName("jobId");

                entity.Property(e => e.Lastsynctime)
                    .HasColumnType("datetime")
                    .HasColumnName("lastsynctime");

                entity.Property(e => e.LastsynctimeZerofailuresHub)
                    .HasColumnType("datetime")
                    .HasColumnName("lastsynctime_zerofailures_hub");

                entity.Property(e => e.LastsynctimeZerofailuresMember)
                    .HasColumnType("datetime")
                    .HasColumnName("lastsynctime_zerofailures_member");

                entity.Property(e => e.Memberhasdata).HasColumnName("memberhasdata");

                entity.Property(e => e.Memberstate).HasColumnName("memberstate");

                entity.Property(e => e.MemberstateLastupdated)
                    .HasColumnType("datetime")
                    .HasColumnName("memberstate_lastupdated")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(140)
                    .HasColumnName("name");

                entity.Property(e => e.Noinitsync).HasColumnName("noinitsync");

                entity.Property(e => e.Scopename)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("scopename")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Syncdirection).HasColumnName("syncdirection");

                entity.Property(e => e.Syncgroupid).HasColumnName("syncgroupid");

                entity.HasOne(d => d.Database)
                    .WithMany(p => p.Syncgroupmembers)
                    .HasForeignKey(d => d.Databaseid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__syncmember__datab");

                entity.HasOne(d => d.Syncgroup)
                    .WithMany(p => p.Syncgroupmembers)
                    .HasForeignKey(d => d.Syncgroupid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__syncmember__syncg");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.ToTable("task", "dss");

                entity.HasIndex(e => e.Actionid, "index_task_actionid");

                entity.HasIndex(e => new { e.Agentid, e.State }, "index_task_agentid_state")
                    .HasFilter("([state]=(0))");

                entity.HasIndex(e => e.Completedtime, "index_task_completedtime");

                entity.HasIndex(e => new { e.State, e.Agentid, e.DependencyCount, e.Priority, e.Creationtime }, "index_task_gettask");

                entity.HasIndex(e => new { e.State, e.Completedtime }, "index_task_state")
                    .HasFilter("([state]=(2))");

                entity.HasIndex(e => new { e.Lastheartbeat, e.State }, "index_task_state_lastheartbeat")
                    .HasFilter("([state]<(0))");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Actionid).HasColumnName("actionid");

                entity.Property(e => e.Agentid).HasColumnName("agentid");

                entity.Property(e => e.Completedtime)
                    .HasColumnType("datetime")
                    .HasColumnName("completedtime");

                entity.Property(e => e.Creationtime)
                    .HasColumnType("datetime")
                    .HasColumnName("creationtime")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.DependencyCount).HasColumnName("dependency_count");

                entity.Property(e => e.Lastheartbeat)
                    .HasColumnType("datetime")
                    .HasColumnName("lastheartbeat");

                entity.Property(e => e.Lastresettime)
                    .HasColumnType("datetime")
                    .HasColumnName("lastresettime");

                entity.Property(e => e.OwningInstanceid).HasColumnName("owning_instanceid");

                entity.Property(e => e.Pickuptime)
                    .HasColumnType("datetime")
                    .HasColumnName("pickuptime");

                entity.Property(e => e.Priority)
                    .HasColumnName("priority")
                    .HasDefaultValueSql("((100))");

                entity.Property(e => e.Request).HasColumnName("request");

                entity.Property(e => e.Response).HasColumnName("response");

                entity.Property(e => e.RetryCount).HasColumnName("retry_count");

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.TaskNumber)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("taskNumber");

                entity.Property(e => e.Type).HasColumnName("type");

                entity.Property(e => e.Version).HasColumnName("version");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.Actionid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__task__actionid");
            });

            modelBuilder.Entity<Taskdependency>(entity =>
            {
                entity.HasKey(e => new { e.Nexttaskid, e.Prevtaskid })
                    .HasName("PK_TaskTask");

                entity.ToTable("taskdependency", "dss");

                entity.Property(e => e.Nexttaskid).HasColumnName("nexttaskid");

                entity.Property(e => e.Prevtaskid).HasColumnName("prevtaskid");

                entity.HasOne(d => d.Nexttask)
                    .WithMany(p => p.TaskdependencyNexttasks)
                    .HasForeignKey(d => d.Nexttaskid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__taskdepen__nextt");

                entity.HasOne(d => d.Prevtask)
                    .WithMany(p => p.TaskdependencyPrevtasks)
                    .HasForeignKey(d => d.Prevtaskid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__taskdepen__prevt");
            });

            modelBuilder.Entity<Uihistory>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("UIHistory", "dss");

                entity.HasIndex(e => e.Agentid, "Idx_UIHistory_AgentId");

                entity.HasIndex(e => e.CompletionTime, "Idx_UIHistory_CompletionTime");

                entity.HasIndex(e => e.Databaseid, "Idx_UIHistory_DatabaseId");

                entity.HasIndex(e => e.Id, "Idx_UIHistory_Id");

                entity.HasIndex(e => e.Serverid, "Idx_UIHistory_ServerId")
                    .IsClustered();

                entity.HasIndex(e => e.SyncgroupId, "Idx_UIHistory_SyncgroupId");

                entity.Property(e => e.Agentid).HasColumnName("agentid");

                entity.Property(e => e.CompletionTime).HasColumnName("completionTime");

                entity.Property(e => e.Databaseid).HasColumnName("databaseid");

                entity.Property(e => e.DetailEnumId)
                    .IsRequired()
                    .HasMaxLength(400)
                    .HasColumnName("detailEnumId");

                entity.Property(e => e.DetailStringParameters).HasColumnName("detailStringParameters");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.IsWritable)
                    .HasColumnName("isWritable")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.RecordType).HasColumnName("recordType");

                entity.Property(e => e.Serverid).HasColumnName("serverid");

                entity.Property(e => e.SyncgroupId).HasColumnName("syncgroupId");

                entity.Property(e => e.TaskType).HasColumnName("taskType");
            });

            modelBuilder.Entity<Userdatabase>(entity =>
            {
                entity.ToTable("userdatabase", "dss");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.Agentid).HasColumnName("agentid");

                entity.Property(e => e.ConnectionString).HasColumnName("connection_string");

                entity.Property(e => e.Database)
                    .HasMaxLength(256)
                    .HasColumnName("database");

                entity.Property(e => e.DbSchema).HasColumnName("db_schema");

                entity.Property(e => e.IsOnPremise).HasColumnName("is_on_premise");

                entity.Property(e => e.JobId).HasColumnName("jobId");

                entity.Property(e => e.LastSchemaUpdated)
                    .HasColumnType("datetime")
                    .HasColumnName("last_schema_updated");

                entity.Property(e => e.LastTombstonecleanup)
                    .HasColumnType("datetime")
                    .HasColumnName("last_tombstonecleanup");

                entity.Property(e => e.Region)
                    .HasMaxLength(256)
                    .HasColumnName("region");

                entity.Property(e => e.Server)
                    .HasMaxLength(256)
                    .HasColumnName("server");

                entity.Property(e => e.SqlazureInfo)
                    .HasColumnType("xml")
                    .HasColumnName("sqlazure_info");

                entity.Property(e => e.State).HasColumnName("state");

                entity.Property(e => e.Subscriptionid).HasColumnName("subscriptionid");

                entity.HasOne(d => d.Subscription)
                    .WithMany(p => p.Userdatabases)
                    .HasForeignKey(d => d.Subscriptionid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__userdatab__subsc");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
