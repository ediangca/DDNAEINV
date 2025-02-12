using DDNAEINV.Model.Details;
using DDNAEINV.Model.Entities;
using DDNAEINV.Model.Views;
using Microsoft.EntityFrameworkCore;

namespace DDNAEINV.Data
{
    public class ApplicationDBContext : DbContext
    {
        //public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        //{

        //}

        public ApplicationDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<BranchType> BranchTypes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<ItemGroup> ItemGroups { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Par> PARS { get; set; }
        public DbSet<ICS> ICSS { get; set; }
        public DbSet<OPR> OPRS { get; set; }
        public DbSet<RePAR> REPARS { get; set; }
        public DbSet<ITR> ITRS { get; set; }
        public DbSet<OPTR> OPTRS { get; set; }
        public DbSet<PRS> PRSS { get; set; }
        public DbSet<RRSEP> RRSEPS { get; set; }
        public DbSet<OPRR> OPRRS { get; set; }
        public DbSet<ParItem> PARItems { get; set; }
        public DbSet<ICSItem> ICSItems { get; set; }
        public DbSet<OPRItem> OPRItems { get; set; }
        public DbSet<PropertyCard> PropertyCards { get; set; }

        public DbSet<Module> Modules { get; set; }
        public DbSet<Privilege> Privileges { get; set; }

        //Views


        public DbSet<BranchesVw> ListOfBranch { get; set; } //List of Branch Details
        public DbSet<DepartmentsVw> ListOfDeparment { get; set; } //List of Department Details
        public DbSet<SectionsVw> ListOfSection { get; set; } //List of Section Details
        public DbSet<AccountID> GeneratedAccID { get; set; } //Generate User Account ID
        public DbSet<UserAccountsVw> ListOfUserAccount { get; set; } //List of User Account Details
        public DbSet<UserProfileVw> ListOfProfile { get; set; } //List of User Profile Details
        public DbSet<ItemVw> ListOfItem { get; set; } //List of Item


        public DbSet<ParVw> ListOfPar { get; set; } //List of PAR
        public DbSet<RePARVw> ListOfREPar { get; set; } //List of REPAR
        public DbSet<OPRVw> ListOfOPR { get; set; } //List of OPR
        public DbSet<OPTRVw> ListOfOPTR { get; set; } //List of REPAR
        public DbSet<PRSVw> ListOfPRS { get; set; } //List of PRS
        public DbSet<OPRRVw> ListOfOPRR { get; set; } //List of OPRR

        public DbSet<PropertyCardDetailsVw> PropertyCardDetails { get; set; } //PropertyCardDetails


        public DbSet<ICSItemVw> ListOfPostedICSItems { get; set; } //List of Posted ICS Items


        public DbSet<ICSVw> ListOfICS { get; set; } //List of ICS
        public DbSet<ITRVw> ListOfITR { get; set; } //List of ITR
        public DbSet<RRSEPVw> ListOfRRSEP { get; set; } //List of RRSEP

        public DbSet<PrivilegeVw> ListOfPrivilege { get; set; } //List of Privilege

        public DbSet<ActivityLogVw> ListOfActivity { get; set; } //List of Privilege

        //Offices Property Cencus
        public DbSet<CencusVw> Cencus { get; set; } //List of Census

        //List of offices and census of PAR, PTR, PRS, ICS, ITR AND RRSEP by each Transaction
        public DbSet<PAROfficesVw> ListPAROffices { get; set; }
        public DbSet<REPAROfficesVw> ListREPAROffices { get; set; }
        public DbSet<PRSOfficesVw> ListPRSOffices { get; set; }
        public DbSet<ICSOfficesVw> ListICSOffices { get; set; }
        public DbSet<ITROfficesVw> ListITROffices { get; set; }
        public DbSet<RRSEPOfficesVw> ListRRSEPOffices { get; set; }
        public DbSet<Above50KOffices> ListofAbove50KOffices { get; set; }
        public DbSet<Below50KOffices> ListofBelow50KOffices { get; set; }

        public DbSet<TotalAbove50ItemsByOfficeVw> TotalAbove50ItemsByOffice { get; set; } //List of Above 50k Items By Office

        //Reports View
        public DbSet<PARItemsDetailsVw> ListOfPARByOffice { get; set; } //List of PAR Items Details
        public DbSet<REPARItemsDetailsVw> ListOfREPARByOffice { get; set; } //List of REPAR Items Details
        public DbSet<PRSItemsDetailsVw> ListOfPRSByOffice { get; set; } //List of PRS Items Details


        public DbSet<ICSItemsDetailsVw> ListOfICSByOffice { get; set; } //List of ICS Items Details
        public DbSet<ITRItemsDetailsVw> ListOfITRByOffice { get; set; } //List of ITR Items Details
        public DbSet<RRSEPItemsDetailsVw> ListOfRRSEPByOffice { get; set; } //List of RRSEP Items Details
        public DbSet<SummaryItemsA50kDetailsVw> ListOfAbove50KByOffice { get; set; } //List of Above 50K Items Details
        public DbSet<SummaryItemsB50kDetailsVw> ListOfBelow50KByOffice { get; set; } //List of Below 50K Items Details

        //Function
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.HasKey(e => e.BranchID); // Configuring primary key
                                                // Other configurations...
            });
            modelBuilder.Entity<BranchType>(entity =>
            {
                entity.HasKey(e => e.BTID); // Configuring primary key
                                            // Other configurations...
                entity.Property(e => e.BTID)
                    .ValueGeneratedOnAdd(); // Configuring auto-increment
            });
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepID); // Configuring primary key
                                             // Other configurations...
                entity.Property(e => e.DepID)
                    .ValueGeneratedOnAdd(); // Configuring auto-increment
            });
            modelBuilder.Entity<Section>(entity =>
            {
                entity.HasKey(e => e.SecID); // Configuring primary key
                                             // Other configurations...
                entity.Property(e => e.SecID)
                    .ValueGeneratedOnAdd(); // Configuring auto-increment
            });
            modelBuilder.Entity<Position>(entity =>
            {
                entity.HasKey(e => e.PositionID); // Configuring primary key
                                                  // Other configurations...
                entity.Property(e => e.PositionID)
                    .ValueGeneratedOnAdd(); // Configuring auto-increment
            });
            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.HasKey(e => e.UserID); // Configuring primary key
                                              // Other configurations...
            });
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasKey(e => e.ProfileID); // Configuring primary key
                                                 // Other configurations...
            });
            modelBuilder.Entity<UserGroup>(entity =>
            {
                entity.HasKey(e => e.UGID); // Configuring primary key
                                            // Other configurations...
                entity.Property(e => e.UGID)
                    .ValueGeneratedOnAdd(); // Configuring auto-increment
            });
            modelBuilder.Entity<ItemGroup>(entity =>
            {
                entity.HasKey(e => e.IGID); // Configuring primary key
                                            // Other configurations...
                entity.Property(e => e.IGID)
                    .ValueGeneratedOnAdd(); // Configuring auto-increment
            });
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.IID); // Configuring primary key
                                           // Other configurations...
                entity.Property(e => e.IID)
                    .ValueGeneratedOnAdd(); // Configuring auto-increment
            });
            //PAR
            modelBuilder.Entity<Par>(entity =>
            {
                entity.HasKey(e => e.parNo); // Configuring primary key
                entity.Property(e => e.parNo).ValueGeneratedNever(); // Prevent OUTPUT clause
            });
            modelBuilder.Entity<ParItem>(entity =>
            {
                entity.HasKey(e => e.PARINO); // Configuring primary key
                                              // Other configurations...
            });
            modelBuilder.Entity<RePAR>(entity =>
            {
                entity.HasKey(e => e.REPARNo); // Configuring primary key
                entity.Property(e => e.REPARNo).ValueGeneratedNever(); // Prevent OUTPUT clause

                // Other configurations...
            });

            //OPTR
            modelBuilder.Entity<OPTR>(entity =>
            {
                entity.HasKey(e => e.OPTRNo); // Configuring primary key
            });
            //PRS
            modelBuilder.Entity<PRS>(entity =>
            {
                entity.HasKey(e => e.PRSNo); // Configuring primary key
                                             // Other configurations...
            });
            //OPR
            modelBuilder.Entity<OPR>(entity =>
            {
                entity.HasKey(e => e.oprNo); // Configuring primary key
            });
            modelBuilder.Entity<OPRItem>(entity =>
            {
                entity.HasKey(e => e.OPRINO); // Configuring primary key
                                              // Other configurations...
            });
            //ICS
            modelBuilder.Entity<ICS>(entity =>
            {
                entity.HasKey(e => e.ICSNo); // Configuring primary key
                                             // Other configurations...
            });
            modelBuilder.Entity<ICSItem>(entity =>
            {
                entity.HasKey(e => e.ICSItemNo); // Configuring primary key
                                                 // Other configurations...
            });
            modelBuilder.Entity<ITR>(entity =>
            {
                entity.HasKey(e => e.ITRNo); // Configuring primary key
                                             // Other configurations...
            });
            modelBuilder.Entity<RRSEP>(entity =>
            {
                entity.HasKey(e => e.RRSEPNo); // Configuring primary key
                                               // Other configurations...
            });
            modelBuilder.Entity<OPRR>(entity =>
            {
                entity.HasKey(e => e.OPRRNo); // Configuring primary key
                                               // Other configurations...
            });
            //PropertyCards
            modelBuilder.Entity<PropertyCard>(entity =>
            {
                entity.HasKey(e => e.PCNo); // Configuring primary key
                                           // Other configurations...
                entity.Property(e => e.PCNo)
                    .ValueGeneratedOnAdd(); // Configuring auto-increment
            });
            //PREVILLEGES
            modelBuilder.Entity<Module>(entity =>
            {
                entity.HasKey(e => e.MID); // Configuring primary key
                                           // Other configurations...
            });
            modelBuilder.Entity<Privilege>(entity =>
            {
                entity.HasKey(e => e.PID); // Configuring primary key
                                           // Other configurations...
            });
            modelBuilder.Entity<PropertyCard>(entity =>
            {
                entity.HasKey(e => e.PCNo); // Configuring primary key
                                           // Other configurations...
            });


            // ----------------- VIEWS -------------------
            modelBuilder.Entity<BranchesVw>().ToView("ListOfBranch");
            modelBuilder.Entity<BranchesVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<DepartmentsVw>().ToView("ListOfDeparment");
            modelBuilder.Entity<DepartmentsVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<SectionsVw>().ToView("ListOfSection");
            modelBuilder.Entity<SectionsVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<AccountID>().ToView("GetGenAccID");
            modelBuilder.Entity<AccountID>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<UserAccountsVw>().ToView("ListOfUserAccount");
            modelBuilder.Entity<UserAccountsVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<UserProfileVw>().ToView("ListOfProfile");
            modelBuilder.Entity<UserProfileVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ItemVw>().ToView("ListOfItem");
            modelBuilder.Entity<ItemVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ParVw>().ToView("ListOfPar");
            modelBuilder.Entity<ParVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<OPRVw>().ToView("ListOfOPR");
            modelBuilder.Entity<OPRVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<CencusVw>().ToView("Cencus");
            modelBuilder.Entity<CencusVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<TotalAbove50ItemsByOfficeVw>().ToView("TotalAbove50ItemsByOffice");
            modelBuilder.Entity<TotalAbove50ItemsByOfficeVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ActivityLogVw>().ToView("ListOfActivity");
            modelBuilder.Entity<ActivityLogVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<RePARVw>().ToView("ListOfREPAR");
            modelBuilder.Entity<RePARVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<OPRRVw>().ToView("ListOfREPAR");
            modelBuilder.Entity<OPRRVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<OPTRVw>().ToView("ListOfOPTR");
            modelBuilder.Entity<OPTRVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ICSVw>().ToView("ListOfICS");
            modelBuilder.Entity<ICSVw>(entity =>
            {
                entity.HasNoKey();
            });


            modelBuilder.Entity<ICSItemVw>().ToView("ListOfPostedICSItems");
            modelBuilder.Entity<ICSItemVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ITRVw>().ToView("ListOfITR");
            modelBuilder.Entity<ITRVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<PRSVw>().ToView("ListOfPRS");
            modelBuilder.Entity<PRSVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<RRSEPVw>().ToView("ListOfRRSEP");
            modelBuilder.Entity<RRSEPVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<OPRRVw>().ToView("ListOfOPRR");
            modelBuilder.Entity<OPRRVw>(entity =>
            {
                entity.HasNoKey();
            });


            modelBuilder.Entity<PrivilegeVw>().ToView("ListOfPrivilege");
            modelBuilder.Entity<PrivilegeVw>(entity =>
            {
                entity.HasNoKey();
            });

            //Offices by Module
            modelBuilder.Entity<PAROfficesVw>().ToView("ListPAROffices");
            modelBuilder.Entity<PAROfficesVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<REPAROfficesVw>().ToView("ListREPAROffices");
            modelBuilder.Entity<REPAROfficesVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<PRSOfficesVw>().ToView("ListPRSOffices");
            modelBuilder.Entity<PRSOfficesVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ICSOfficesVw>().ToView("ListICSOffices");
            modelBuilder.Entity<ICSOfficesVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ITROfficesVw>().ToView("ListITROffices");
            modelBuilder.Entity<ITROfficesVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<RRSEPOfficesVw>().ToView("ListRRSEPOffices");
            modelBuilder.Entity<RRSEPOfficesVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<PARItemsDetailsVw>().ToView("ListOfPARByOffice");
            modelBuilder.Entity<PARItemsDetailsVw>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<REPARItemsDetailsVw>().ToView("ListOfREPARByOffice");
            modelBuilder.Entity<REPARItemsDetailsVw>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<PRSItemsDetailsVw>().ToView("ListOfPRSByOffice");
            modelBuilder.Entity<PRSItemsDetailsVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<ICSItemsDetailsVw>().ToView("ListOfICSByOffice");
            modelBuilder.Entity<ICSItemsDetailsVw>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<ITRItemsDetailsVw>().ToView("ListOfITRByOffice");
            modelBuilder.Entity<ITRItemsDetailsVw>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<RRSEPItemsDetailsVw>().ToView("ListOfRRSEPByOffice");
            modelBuilder.Entity<RRSEPItemsDetailsVw>(entity =>
            {
                entity.HasNoKey();
            });


            modelBuilder.Entity<Above50KOffices>().ToView("ListofAbove50KOffices");
            modelBuilder.Entity<Above50KOffices>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<Below50KOffices>().ToView("ListofBelow50KOffices");
            modelBuilder.Entity<Below50KOffices>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<SummaryItemsA50kDetailsVw>().ToView("ListOfAbove50KByOffice");
            modelBuilder.Entity<SummaryItemsA50kDetailsVw>(entity =>
            {
                entity.HasNoKey();
            });
            modelBuilder.Entity<SummaryItemsB50kDetailsVw>().ToView("ListOfBelow50KByOffice");
            modelBuilder.Entity<SummaryItemsB50kDetailsVw>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<PropertyCardDetailsVw>().ToView("PropertyCardDetails");
            modelBuilder.Entity<PropertyCardDetailsVw>(entity =>
            {
                entity.HasNoKey();
            });


            base.OnModelCreating(modelBuilder);

        }


    }
}
