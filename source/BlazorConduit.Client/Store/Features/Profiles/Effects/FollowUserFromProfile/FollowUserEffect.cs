using BlazorConduit.Client.Models.Common;
using BlazorConduit.Client.Models.Profiles;
using BlazorConduit.Client.Services.Contracts;
using BlazorConduit.Client.Store.Features.Profiles.Actions.FollowUserFromProfile;
using Fluxor;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorConduit.Client.Store.Features.Profiles.Effects.FollowUserFromProfile
{
    public class FollowUserEffect : Effect<FollowUserFromProfileAction>
    {
        private readonly ILogger<FollowUserEffect> _logger;
        private readonly IConduitApiService _apiService;
        private readonly ITokenService _tokenService;

        public FollowUserEffect(ILogger<FollowUserEffect> logger, IConduitApiService apiService, ITokenService tokenService) =>
            (_logger, _apiService, _tokenService) = (logger, apiService, tokenService);

        protected override async Task HandleAsync(FollowUserFromProfileAction action, IDispatcher dispatcher)
        {
            try
            {
                // Call the profile user endpoint with the username
                var followResponse = await _apiService.PostNoContentAsync($"profiles/{action.Username}/follow", await _tokenService.GetTokenAsync());

                if (followResponse is null || !followResponse.IsSuccessStatusCode)
                {
                    // Throw the exception to issue the failure action
                    throw new ConduitApiException($"Could not follow user profile {action.Username}", HttpStatusCode.InternalServerError);
                }

                // Get the returned user profile
                var profile = await followResponse.Content.ReadFromJsonAsync<UserProfileResponse>();

                if (profile is null || profile.Profile is null)
                {
                    throw new ConduitApiException($"No profile returned in response to follow for user {action.Username}", HttpStatusCode.InternalServerError);
                }

                dispatcher.Dispatch(new FollowUserFromProfileSuccessAction(profile.Profile));
            }
            catch (ConduitApiException e)
            {
                _logger.LogError($"Validation error during profile follow for user {action.Username}");
                dispatcher.Dispatch(new FollowUserFromProfileFailureAction(e.Message, e.ApiErrors));
            }
            catch (Exception e)
            {
                _logger.LogError($"Error during profile follow for user {action.Username}");
                dispatcher.Dispatch(new FollowUserFromProfileFailureAction(e.Message));
            }
        }
    }
}
