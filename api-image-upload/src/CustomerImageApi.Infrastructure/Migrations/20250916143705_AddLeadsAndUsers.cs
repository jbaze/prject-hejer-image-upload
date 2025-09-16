using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerImageApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadsAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Customers (Name, Email, PhoneNumber, Address, Price, StartingDate, EstimatedTime, CreatedAt,UpdatedAt)
                SELECT TOP 10
                  Names.FirstName as Name,
                  CONCAT('cust.', FORMAT(ROW_NUMBER() OVER (ORDER BY NEWID()), '00'), '@techup.me') as Email,
                  CAST(ABS(CHECKSUM(NEWID())) % 900000000 + 100000000 AS VARCHAR(9)) as PhoneNumber,
                  Addresses.Address,
                  CAST(ABS(CHECKSUM(NEWID())) % 2000 + 50 AS INT) as Price,
                  DATEADD(day, -(ABS(CHECKSUM(NEWID())) % 30), GETDATE()) as StartingDate,
                  DATEADD(day, ABS(CHECKSUM(NEWID())) % 90 + 1, GETDATE()) as EstimatedTime,
                  GETDATE() as CreateAt,
                  null as UpdatedAt
                FROM (
                  SELECT 'Dean' as FirstName UNION ALL
                  SELECT 'Sarah' UNION ALL
                  SELECT 'Michael' UNION ALL
                  SELECT 'Emma' UNION ALL
                  SELECT 'James' UNION ALL
                  SELECT 'Olivia' UNION ALL
                  SELECT 'Robert' UNION ALL
                  SELECT 'Sophia' UNION ALL
                  SELECT 'Christopher' UNION ALL
                  SELECT 'Evelyn'
                ) Names
                CROSS JOIN (
                  SELECT 'Nordre Ringvej 32, København V' as Address UNION ALL
                  SELECT 'Vesterbrogade 15, København V' UNION ALL
                  SELECT 'Nørrebrogade 45, København N' UNION ALL
                  SELECT 'Østerbrogade 78, København Ø' UNION ALL
                  SELECT 'Amagerbrogade 123, København S' UNION ALL
                  SELECT 'Frederiksberg Allé 67, Frederiksberg' UNION ALL
                  SELECT 'Gothersgade 89, København K' UNION ALL
                  SELECT 'Strøget 12, København K' UNION ALL
                  SELECT 'Kongens Nytorv 5, København K' UNION ALL
                  SELECT 'Bredgade 34, København K'
                ) Addresses
                ORDER BY NEWID();

                INSERT INTO Leads (Name, Email, PhoneNumber, Address, Source, Price, FollowUpDays, StartingDate, EstimatedTime, CreatedAt, UpdatedAt)
                SELECT TOP 10
                  Names.FirstName as Name,
                  CONCAT('lead.', FORMAT(ROW_NUMBER() OVER (ORDER BY NEWID()), '00'), '@techup.me') as Email,
                  CAST(ABS(CHECKSUM(NEWID())) % 900000000 + 100000000 AS VARCHAR(9)) as PhoneNumber,
                  Addresses.Address,
                  Sources.Source,
                  CAST(ABS(CHECKSUM(NEWID())) % 200 + 10 AS INT) as Price,
                  CAST(ABS(CHECKSUM(NEWID())) % 90 + 7 AS INT) as FollowUpDays,
                  DATEADD(day, -(ABS(CHECKSUM(NEWID())) % 30), GETDATE()) as StartingDate,
                  DATEADD(day, ABS(CHECKSUM(NEWID())) % 60 + 1, GETDATE()) as EstimatedTime,
                  GETDATE() as CreatedAt,
                  null as UpdatedAt
                FROM (
                  SELECT 'Elene' as FirstName UNION ALL
                  SELECT 'Anna' UNION ALL
                  SELECT 'Lars' UNION ALL
                  SELECT 'Mette' UNION ALL
                  SELECT 'Niels' UNION ALL
                  SELECT 'Ida' UNION ALL
                  SELECT 'Peter' UNION ALL
                  SELECT 'Rikke' UNION ALL
                  SELECT 'Søren' UNION ALL
                  SELECT 'Kirsten'
                ) Names
                CROSS JOIN (
                  SELECT 'Region Midtjylland, Risskov Ø, Kamperhoug 81' as Address UNION ALL
                  SELECT 'Region Hovedstaden, Hellerup, Strandvejen 45' UNION ALL
                  SELECT 'Region Syddanmark, Odense C, Vestergade 12' UNION ALL
                  SELECT 'Region Nordjylland, Aalborg Ø, Kastetvej 67' UNION ALL
                  SELECT 'Region Sjælland, Roskilde, Sankt Jørgensbjerg 23' UNION ALL
                  SELECT 'Region Midtjylland, Aarhus C, Søndergade 89' UNION ALL
                  SELECT 'Region Hovedstaden, Frederiksberg, Gammel Kongevej 34' UNION ALL
                  SELECT 'Region Syddanmark, Vejle, Sankt Nikolaj Gade 45' UNION ALL
                  SELECT 'Region Nordjylland, Thisted, Storegade 67' UNION ALL
                  SELECT 'Region Sjælland, Slagelse, Adelgade 23'
                ) Addresses
                CROSS JOIN (
                  SELECT 'Facebook' as Source UNION ALL
                  SELECT 'Google Ads' UNION ALL
                  SELECT 'LinkedIn' UNION ALL
                  SELECT 'Instagram' UNION ALL
                  SELECT 'Website' UNION ALL
                  SELECT 'Referral' UNION ALL
                  SELECT 'Cold Call' UNION ALL
                  SELECT 'Email Campaign' UNION ALL
                  SELECT 'Trade Show' UNION ALL
                  SELECT 'YouTube'
                ) Sources
                ORDER BY NEWID();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
