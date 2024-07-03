using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Dapper;
using Microsoft.Extensions.Logging;
using MySqlConnector;

using static Equipments.Equipments;
using static EquipmentsAPI.Equipments;

namespace Equipments;

public static class Database
{
    public static string GlobalDatabaseConnectionString { get; set; } = string.Empty;

    public static async Task<MySqlConnection> ConnectAsync()
    {
        MySqlConnection connection = new(GlobalDatabaseConnectionString);
        await connection.OpenAsync();
        return connection;
    }

    public static void ExecuteAsync(string query, object? parameters)
    {
        Task.Run(async () => {
            using MySqlConnection connection = await ConnectAsync();
            await connection.ExecuteAsync(query, parameters);
        });
    }

    public static async Task CreateDatabaseAsync(EquipmentsConfig config)
    {
        MySqlConnectionStringBuilder builder = new()
        {
            Server = config.Database.Host,
            Database = config.Database.Name,
            UserID = config.Database.User,
            Password = config.Database.Password,
            Port = config.Database.Port,
            Pooling = true,
            MinimumPoolSize = 0,
            MaximumPoolSize = 640,
            ConnectionIdleTimeout = 30,
            AllowZeroDateTime = true
        };

        GlobalDatabaseConnectionString = builder.ConnectionString;

        using MySqlConnection connection = await ConnectAsync();
        using MySqlTransaction transaction = await connection.BeginTransactionAsync();

        try
        {
            string equipTable = config.Database.DBTable;

            await connection.ExecuteAsync($@"
                CREATE TABLE IF NOT EXISTS {equipTable} (
                id INT NOT NULL AUTO_INCREMENT,
                SteamID BIGINT UNSIGNED NOT NULL,
                Type varchar(16) NOT NULL,
                UniqueId varchar(256) NOT NULL,
                Slot INT,
                PRIMARY KEY (id)
            );", transaction: transaction);

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public static void LoadPlayer(CCSPlayerController player)
    {
        if (player == null || !player.IsValid || player.IsBot || player.IsHLTV || string.IsNullOrEmpty(player.IpAddress))
            return;

        ulong steamid = player.SteamID;
        string PlayerName = player.PlayerName;

        Task.Run(async () => {
            await LoadPlayerAsync(player, steamid, PlayerName);
        });
    }

    public static async Task LoadPlayerAsync(CCSPlayerController player, ulong SteamID, string PlayerName)
    {
        async Task LoadDataAsync(int attempt = 1)
        {
            try
            {
                using MySqlConnection connection = await ConnectAsync();

                SqlMapper.GridReader multiQuery = await connection.QueryMultipleAsync(
                    $@"SELECT * FROM {Instance.Config.Database.DBTable} WHERE SteamID = @SteamID",
                    new { SteamID, DateTime.Now });

                IEnumerable<Equipments_Items> equipments = await multiQuery.ReadAsync<Equipments_Items>();

                Server.NextFrame(() =>
                {
                    foreach (Equipments_Items newEquipment in equipments)
                    {
                        Equipments_Items? existingEquipment = Instance.GlobalEquipmentsItems.FirstOrDefault(e => e.SteamID == newEquipment.SteamID && e.UniqueId == newEquipment.UniqueId);
                        if (existingEquipment != null)
                        {
                            existingEquipment.Type = newEquipment.Type;
                            existingEquipment.Slot = newEquipment.Slot;
                        }
                        else
                        {
                            Instance.GlobalEquipmentsItems.Add(newEquipment);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Instance.Logger.LogError("Error load player {SteamID} attempt {attempt}: ex:{ErrorMessage}", SteamID, attempt, ex.Message);

                if (attempt < 3)
                {
                    Instance.Logger.LogInformation("Retrying to load player {SteamID} (attempt: {attempt})", SteamID, attempt + 1);
                    await Task.Delay(5000);
                    await LoadDataAsync(attempt + 1);
                }
            }
        }

        await LoadDataAsync();
    }

    public static void SavePlayerEquipment(CCSPlayerController player, Equipments_Items item)
    {
        ExecuteAsync($@"INSERT INTO {Instance.Config.Database.DBTable} (SteamID, Type, UniqueId, Slot) VALUES (@SteamID, @Type, @UniqueId, @Slot);",
        new {
            player.SteamID,
            item.Type,
            item.UniqueId,
            item.Slot
        });
    }
    public static void RemovePlayerEquipment(CCSPlayerController player, string UniqueId)
    {
        ExecuteAsync($@"DELETE FROM {Instance.Config.Database.DBTable} WHERE SteamID = @SteamID AND UniqueId = @UniqueId;",
        new {
            player.SteamID,
            UniqueId
        });
    }

    public static void ResetPlayer(CCSPlayerController player)
    {
        ExecuteAsync($@"DELETE FROM {Instance.Config.Database.DBTable} WHERE SteamID = @SteamID;",
        new { player.SteamID } );
    }

    public static void ResetDatabase()
    {
        Task.Run(async () => {
            using MySqlConnection connection = await ConnectAsync();
            connection.Query($@"DROP TABLE {Instance.Config.Database.DBTable}");
        });
    }
}