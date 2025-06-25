using DigitalWorldOnline.Commons.Entities;
using DigitalWorldOnline.Commons.Models.Base;
using DigitalWorldOnline.Game.Services;
using Serilog;

namespace DigitalWorldOnline.Game.Services
{
    public class IntegrityValidationService
    {
        private readonly ILogger _logger;
        private readonly SecurityAuditService _securityAudit;

        public IntegrityValidationService(ILogger logger, SecurityAuditService securityAudit)
        {
            _logger = logger;
            _securityAudit = securityAudit;
        }

        public ValidationResult ValidateInventoryOperation(GameClient client, ItemModel item, int requestedAmount, string operation)
        {
            try
            {
                if (item == null)
                {
                    return ValidationResult.Failure("Item not found");
                }

                if (requestedAmount <= 0)
                {
                    return ValidationResult.Failure("Invalid amount requested");
                }

                var actualAmount = client.Tamer.Inventory.CountItensById(item.ItemId);
                if (actualAmount < requestedAmount)
                {
                    _securityAudit.LogSuspiciousActivity(client, 
                        "INVENTORY_INTEGRITY_VIOLATION", 
                        $"Attempted {operation} with {requestedAmount}x {item.ItemInfo.Name} but only has {actualAmount}x");
                    
                    return ValidationResult.Failure($"Insufficient items: has {actualAmount}, requested {requestedAmount}");
                }

                if (item.Amount < requestedAmount)
                {
                    _securityAudit.LogSuspiciousActivity(client, 
                        "ITEM_AMOUNT_MISMATCH", 
                        $"Item stack shows {item.Amount} but requested {requestedAmount}");
                    
                    return ValidationResult.Failure($"Item stack amount mismatch");
                }

                return ValidationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[INTEGRITY_ERROR] Error validating inventory operation for {client.Tamer.Name}");
                return ValidationResult.Failure("Validation error occurred");
            }
        }

        public ValidationResult ValidateTradeIntegrity(GameClient client, GameClient targetClient)
        {
            try
            {
                // Verificar se ambos os clientes ainda estão conectados e em troca
                if (!client.IsConnected || !targetClient.IsConnected)
                {
                    return ValidationResult.Failure("One or both players disconnected");
                }

                if (!client.Tamer.TradeCondition || !targetClient.Tamer.TradeCondition)
                {
                    return ValidationResult.Failure("One or both players not in trade state");
                }

                // Validar itens do cliente
                foreach (var tradeItem in client.Tamer.TradeInventory.EquippedItems)
                {
                    var validation = ValidateInventoryOperation(client, tradeItem, tradeItem.Amount, "TRADE");
                    if (!validation.IsValid)
                    {
                        return validation;
                    }
                }

                // Validar itens do cliente alvo
                foreach (var tradeItem in targetClient.Tamer.TradeInventory.EquippedItems)
                {
                    var validation = ValidateInventoryOperation(targetClient, tradeItem, tradeItem.Amount, "TRADE");
                    if (!validation.IsValid)
                    {
                        return validation;
                    }
                }

                // Validar bits
                if (client.Tamer.TradeInventory.Bits > client.Tamer.Inventory.Bits)
                {
                    _securityAudit.LogSuspiciousActivity(client, 
                        "BITS_INTEGRITY_VIOLATION", 
                        $"Attempted to trade {client.Tamer.TradeInventory.Bits} bits but only has {client.Tamer.Inventory.Bits}");
                    
                    return ValidationResult.Failure("Insufficient bits for trade");
                }

                if (targetClient.Tamer.TradeInventory.Bits > targetClient.Tamer.Inventory.Bits)
                {
                    _securityAudit.LogSuspiciousActivity(targetClient, 
                        "BITS_INTEGRITY_VIOLATION", 
                        $"Attempted to trade {targetClient.Tamer.TradeInventory.Bits} bits but only has {targetClient.Tamer.Inventory.Bits}");
                    
                    return ValidationResult.Failure("Target has insufficient bits for trade");
                }

                return ValidationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[INTEGRITY_ERROR] Error validating trade integrity between {client.Tamer.Name} and {targetClient.Tamer.Name}");
                return ValidationResult.Failure("Trade validation error occurred");
            }
        }

        public ValidationResult ValidateHatchIntegrity(GameClient client)
        {
            try
            {
                // Verificar se há um ovo na incubadora
                if (client.Tamer.Incubator.EggId == 0)
                {
                    return ValidationResult.Failure("No egg in incubator");
                }

                // Verificar se o ovo está pronto para chocar
                if (client.Tamer.Incubator.HatchLevel < 3)
                {
                    return ValidationResult.Failure("Egg not ready to hatch");
                }

                // Verificar se há slots disponíveis
                var availableSlots = 0;
                for (byte i = 0; i < client.Tamer.DigimonSlots; i++)
                {
                    if (client.Tamer.Digimons.FirstOrDefault(x => x.Slot == i) == null)
                    {
                        availableSlots++;
                    }
                }

                if (availableSlots == 0)
                {
                    return ValidationResult.Failure("No available Digimon slots");
                }

                return ValidationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[INTEGRITY_ERROR] Error validating hatch integrity for {client.Tamer.Name}");
                return ValidationResult.Failure("Hatch validation error occurred");
            }
        }

        public ValidationResult ValidatePlayerState(GameClient client, string operation)
        {
            try
            {
                if (!client.IsConnected)
                {
                    return ValidationResult.Failure("Player not connected");
                }

                if (client.Loading)
                {
                    return ValidationResult.Failure("Player is loading");
                }

                if (client.Tamer.State != Commons.Enums.Character.CharacterStateEnum.Ready)
                {
                    return ValidationResult.Failure("Player not in ready state");
                }

                // Verificar se o jogador não está fazendo muitas operações simultaneamente
                if (_securityAudit.IsSuspiciousActivity(client.TamerId, operation))
                {
                    _securityAudit.LogSuspiciousActivity(client, 
                        "RATE_LIMIT_EXCEEDED", 
                        $"Too many {operation} attempts in short time");
                    
                    return ValidationResult.Failure("Too many operations, please slow down");
                }

                return ValidationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"[INTEGRITY_ERROR] Error validating player state for {client.Tamer.Name}");
                return ValidationResult.Failure("Player state validation error occurred");
            }
        }

        public ValidationResult ValidateItemIntegrity(ItemModel item)
        {
            try
            {
                if (item == null)
                {
                    return ValidationResult.Failure("Item is null");
                }

                if (item.ItemId <= 0)
                {
                    return ValidationResult.Failure("Invalid item ID");
                }

                if (item.Amount <= 0)
                {
                    return ValidationResult.Failure("Invalid item amount");
                }

                if (item.ItemInfo == null)
                {
                    return ValidationResult.Failure("Item info is missing");
                }

                return ValidationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "[INTEGRITY_ERROR] Error validating item integrity");
                return ValidationResult.Failure("Item validation error occurred");
            }
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;

        private ValidationResult(bool isValid, string errorMessage = "")
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }

        public static ValidationResult Success() => new(true);
        public static ValidationResult Failure(string errorMessage) => new(false, errorMessage);
    }
}
