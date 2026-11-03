using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Services.Exceptions;
using Npgsql;

namespace Services;

public static class ReservationService
{
    public static async Task WaitForAvailability(int projectID, CancellationToken ct)
    {
        using (var context = AppDbContextFactory.CreateContext())
        {
            while (await context.ProjectReservations
                .AnyAsync(pr => pr.ProjectID == projectID, ct))
            {
                await Task.Delay(1000, ct);
            }
        }
    }

    public static async Task<bool> LockReservedProject(AppDbContext context, int projectID, int userActionID, CancellationToken ct)
    {
        var project = await context.Projects
            .FromSqlInterpolated($"""
                SELECT *
                FROM "Projects"
                WHERE "ID" = {projectID}
                FOR UPDATE
                """)
            .SingleOrDefaultAsync(ct);

        if (project is null)
        {
            throw new InexistentProjectException();
        }

        return await context.ProjectReservations
            .AnyAsync(pr => pr.ProjectID == projectID && pr.UserActionID == userActionID, ct);
    }

    public static async Task<bool> TryAcquireProjectLock(AppDbContext context, int projectID, CancellationToken ct)
    {
        try
        {
            var project = await context.Projects
                .FromSqlInterpolated($"""
                    SELECT *
                    FROM "Projects"
                    WHERE "ID" = {projectID}
                    FOR UPDATE NOWAIT
                    """)
                .SingleOrDefaultAsync(ct);

            if (project is null)
            {
                throw new InexistentProjectException();
            }

            return true;
        }

        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.LockNotAvailable)
        {
            return false;
        }
    }

    public static async Task<bool> TryCreateReservation(AppDbContext context, int projectID, int userActionID, CancellationToken ct)
    {
        if (await context.ProjectReservations
            .AnyAsync(pr => pr.ProjectID == projectID, ct))
        {
            return false;
        }

        context.ProjectReservations.Add(new(projectID, userActionID));
        return true;
    }

    public static async Task ReleaseProjectReservation(AppDbContext context, int projectID, int userActionID, CancellationToken ct)
    {
        var reservation = await context.ProjectReservations
            .SingleOrDefaultAsync(pr => pr.ProjectID == projectID && pr.UserActionID == userActionID, ct);
        
        if (reservation is null)
        {
            throw new InvalidOperationException("The project isn't currently reserved by any actions");
        }

        context.ProjectReservations.Remove(reservation);
    }
}
