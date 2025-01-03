using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayrollAPI.Migrations.HRMDBConnectMigrations
{
    /// <inheritdoc />
    public partial class createReservationPaymentView_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE 
    ALGORITHM = UNDEFINED 
    DEFINER = `admin`@`%` 
    SQL SECURITY DEFINER
VIEW `hrmdb`.`Reservation_Payments_View` AS
    SELECT 
        `r`.`id` AS `reservationId`,
        `e`.`epf` AS `epf`,
        `r`.`checkInDate` AS `checkInDate`,
        `r`.`checkOutDate` AS `checkOutDate`,
        `rc`.`id` AS `categoryId`,
        `rc`.`categoryName` AS `categoryName`,
        `r`.`bookingStatus` AS `status`,
        IFNULL(`cc`.`cancellationFees`,
                `r`.`reservationCost`) AS `amount`,
        IFNULL(`cc`.`cancellation_Policy`,
                'Reservation Charge') AS `chargeType`
    FROM
        (((`hrmdb`.`reservation` `r`
        LEFT JOIN `hrmdb`.`reservationcategory` `rc` ON ((`r`.`reservationCategoryid` = `rc`.`id`)))
        LEFT JOIN `hrmdb`.`employee` `e` ON ((`r`.`employeeid` = `e`.`id`)))
        LEFT JOIN `hrmdb`.`cancellationcharges` `cc` ON ((`r`.`id` = `cc`.`reservationID`)))");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS Reservation_Payments_View;");
        }
    }
}
