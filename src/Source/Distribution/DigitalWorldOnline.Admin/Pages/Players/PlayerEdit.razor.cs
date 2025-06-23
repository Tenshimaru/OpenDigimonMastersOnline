using MediatR;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using DigitalWorldOnline.Commons.ViewModel.Players;
using DigitalWorldOnline.Application.Admin.Queries;
using DigitalWorldOnline.Application.Admin.Commands;
using DigitalWorldOnline.Commons.Enums.Admin;
using DigitalWorldOnline.Commons.Enums.Character;
using System;
using System.Threading.Tasks;

namespace DigitalWorldOnline.Admin.Pages.Players
{
    public partial class PlayerEdit
    {
        [Parameter] public long PlayerId { get; set; }
        
        [Inject] public ISender Sender { get; set; }
        [Inject] public ISnackbar Snackbar { get; set; }
        [Inject] public NavigationManager Navigation { get; set; }

        private PlayerViewModel? Player;
        private bool Loading = true;
        private bool Saving = false;
        private bool success;
        private MudForm form;

        protected override async Task OnInitializedAsync()
        {
            await LoadPlayer();
        }

        private async Task LoadPlayer()
        {
            try
            {
                Loading = true;
                
                // Get player details from database
                var result = await Sender.Send(new GetPlayerByIdQuery(PlayerId));

                if (result?.Register != null)
                {
                    Player = new PlayerViewModel
                    {
                        Id = result.Register.Id,
                        AccountId = result.Register.AccountId,
                        Name = result.Register.Name,
                        Level = result.Register.Level,
                        CurrentExperience = result.Register.CurrentExperience,
                        MapId = result.Register.Location?.MapId ?? 0,
                        State = result.Register.State,
                        EventState = result.Register.EventState,
                        Channel = result.Register.Channel,
                        Model = result.Register.Model,
                        Size = result.Register.Size,
                        CurrentHp = result.Register.CurrentHp,
                        CurrentDs = result.Register.CurrentDs,
                        XGauge = result.Register.Xai?.XGauge ?? 0,
                        XCrystals = result.Register.Xai?.XCrystals ?? 0,
                        CurrentTitle = result.Register.CurrentTitle,
                        DigimonSlots = result.Register.DigimonSlots,
                        Position = result.Register.Position,
                        CreateDate = result.Register.CreateDate
                    };
                }
                else
                {
                    Snackbar.Add("Player not found", Severity.Error);
                    Navigation.NavigateTo("/players");
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading player: {ex.Message}", Severity.Error);
                Navigation.NavigateTo("/players");
            }
            finally
            {
                Loading = false;
            }
        }

        private async Task SavePlayer()
        {
            if (Player == null) return;

            try
            {
                Saving = true;

                // Update player in database
                var command = new UpdatePlayerCommand(
                    Player.Id,
                    Player.Name,
                    Player.Level,
                    Player.CurrentExperience,
                    Player.MapId,
                    Player.State,
                    Player.EventState,
                    Player.Channel,
                    Player.Model,
                    Player.Size,
                    Player.CurrentHp,
                    Player.CurrentDs,
                    Player.XGauge,
                    Player.XCrystals,
                    Player.CurrentTitle,
                    Player.DigimonSlots
                );

                var result = await Sender.Send(command);

                if (result)
                {
                    Snackbar.Add("Player updated successfully!", Severity.Success);
                    Navigation.NavigateTo("/players");
                }
                else
                {
                    Snackbar.Add("Failed to update player", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error saving player: {ex.Message}", Severity.Error);
            }
            finally
            {
                Saving = false;
            }
        }
    }
}
